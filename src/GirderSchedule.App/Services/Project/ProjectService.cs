using GirderSchedule.App.Constants;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.Domain.Models;
using Microsoft.Win32;
using ScheduleTools.Core.RecentProjects;
using ScheduleTools.Core.Settings;
using ScheduleTools.Wpf.ProjectCreate.ViewModels;
using ScheduleTools.Wpf.ProjectCreate.Views;
using ScheduleTools.Wpf.Services;
using System;
using System.IO;
using System.Windows;

namespace GirderSchedule.App.Services.Project
{
    public sealed class ProjectService
    {
        private readonly ProjectFileService _fileService;
        private readonly RecentProjectService _recentProjectService;
        private readonly AppSettingService _appSettingService;

        public ProjectService(
            ProjectFileService fileService,
            RecentProjectService recentProjectService,
            AppSettingService appSettingService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _recentProjectService = recentProjectService ?? throw new ArgumentNullException(nameof(recentProjectService));
            _appSettingService = appSettingService ?? throw new ArgumentNullException(nameof(appSettingService));
        }

        public ProjectOpenResult CreateNewProject()
        {
            var viewModel = new ProjectCreateWindowViewModel(_appSettingService);

            var window = new ProjectCreateWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = viewModel
            };

            if (window.ShowDialog() != true || window.Result == null)
                return ProjectOpenResult.Cancel();

            try
            {
                var result = window.Result;

                Directory.CreateDirectory(result.ProjectFolder);

                var project = new ScheduleProjectFactory().Create(result.ProjectName);

                var fileName = result.ProjectName + FileConstants.ProjectExtension;
                var filePath = Path.Combine(result.ProjectFolder, fileName);

                if (File.Exists(filePath))
                {
                    DialogService.ShowNotice(
                        Properties.Resources.Title_ProjectSaveFailed,
                        Properties.Resources.Content_FileSaveFailedWithError);

                    return ProjectOpenResult.Fail();
                }

                _fileService.Save(filePath, project);
                _recentProjectService.AddOrUpdate(filePath, project.ProjectName);

                return ProjectOpenResult.Success(project, filePath);
            }
            catch
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ProjectSaveFailed,
                    Properties.Resources.Content_FileSaveFailedWithError);

                return ProjectOpenResult.Fail();
            }
        }

        public ProjectOpenResult OpenProject()
        {
            var dialog = new OpenFileDialog
            {
                Title = FileConstants.ProjectDialogTitle,
                Filter = FileConstants.ProjectFilter,
                DefaultExt = FileConstants.ProjectExtension
            };

            if (dialog.ShowDialog() != true)
                return ProjectOpenResult.Cancel();

            try
            {
                var project = _fileService.Load(dialog.FileName);

                _recentProjectService.AddOrUpdate(dialog.FileName, project.ProjectName);

                return ProjectOpenResult.Success(project, dialog.FileName);
            }
            catch
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_OpenProjectFailed,
                    Properties.Resources.Content_FileOpenFailedWithError);

                return ProjectOpenResult.Fail();
            }
        }

        public ProjectSaveResult SaveProject(ScheduleProject project, string filePath)
        {
            ArgumentNullException.ThrowIfNull(project);

            if (string.IsNullOrWhiteSpace(filePath))
                return SaveProjectAs(project);

            try
            {
                _fileService.Save(filePath, project);
                _recentProjectService.AddOrUpdate(filePath, project.ProjectName);

                return ProjectSaveResult.Success(filePath);
            }
            catch
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ProjectSaveFailed,
                    Properties.Resources.Content_FileSaveFailedWithError);

                return ProjectSaveResult.Fail();
            }
        }

        public ProjectSaveResult SaveProjectAs(ScheduleProject project)
        {
            ArgumentNullException.ThrowIfNull(project);

            var dialog = new SaveFileDialog
            {
                Title = FileConstants.ProjectDialogTitle,
                Filter = FileConstants.ProjectFilter,
                DefaultExt = FileConstants.ProjectExtension,
                AddExtension = true,
                FileName = string.IsNullOrWhiteSpace(project.ProjectName)
                    ? FileConstants.DefaultProjectFileName
                    : project.ProjectName + FileConstants.ProjectExtension
            };

            if (dialog.ShowDialog() != true)
                return ProjectSaveResult.Cancel();

            try
            {
                _fileService.Save(dialog.FileName, project);
                _recentProjectService.AddOrUpdate(dialog.FileName, project.ProjectName);

                return ProjectSaveResult.Success(dialog.FileName);
            }
            catch
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ProjectSaveFailed,
                    Properties.Resources.Content_FileSaveFailedWithError);

                return ProjectSaveResult.Fail();
            }
        }
    }

    public sealed class ProjectOpenResult
    {
        public bool IsSuccess { get; private init; }
        public bool IsCanceled { get; private init; }
        public ScheduleProject? Project { get; private init; }
        public string FilePath { get; private init; } = string.Empty;

        private ProjectOpenResult()
        {
        }

        public static ProjectOpenResult Success(ScheduleProject project, string filePath)
        {
            ArgumentNullException.ThrowIfNull(project);

            return new ProjectOpenResult
            {
                IsSuccess = true,
                Project = project,
                FilePath = filePath ?? string.Empty
            };
        }

        public static ProjectOpenResult Cancel()
        {
            return new ProjectOpenResult
            {
                IsCanceled = true
            };
        }

        public static ProjectOpenResult Fail()
        {
            return new ProjectOpenResult();
        }
    }

    public sealed class ProjectSaveResult
    {
        public bool IsSuccess { get; private init; }
        public bool IsCanceled { get; private init; }
        public string FilePath { get; private init; } = string.Empty;

        private ProjectSaveResult()
        {
        }

        public static ProjectSaveResult Success(string filePath)
        {
            return new ProjectSaveResult
            {
                IsSuccess = true,
                FilePath = filePath ?? string.Empty
            };
        }

        public static ProjectSaveResult Cancel()
        {
            return new ProjectSaveResult
            {
                IsCanceled = true
            };
        }

        public static ProjectSaveResult Fail()
        {
            return new ProjectSaveResult();
        }
    }
}