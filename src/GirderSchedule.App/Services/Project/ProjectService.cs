using GirderSchedule.App.Constants;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.Domain.Models;
using Microsoft.Win32;
using ScheduleTools.Core.RecentProjects;
using ScheduleTools.Core.Settings;
using ScheduleTools.Wpf.ProjectCreate.ViewModels;
using ScheduleTools.Wpf.ProjectCreate.Views;
using ScheduleTools.Wpf.Services;
using ScheduleTools.Wpf.Windows.Contracts;
using System;
using System.IO;

namespace GirderSchedule.App.Services.Project
{
    public sealed class ProjectService
    {
        private readonly ProjectFileService _fileService;
        private readonly RecentProjectService _recentProjectService;
        private readonly AppSettingService _appSettingService;
        private readonly ScheduleProjectFactory _projectFactory;
        private readonly IWindowService _windowService;

        public ProjectService(ProjectFileService fileService, RecentProjectService recentProjectService, AppSettingService appSettingService, ScheduleProjectFactory projectFactory, IWindowService windowService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _recentProjectService = recentProjectService ?? throw new ArgumentNullException(nameof(recentProjectService));
            _appSettingService = appSettingService ?? throw new ArgumentNullException(nameof(appSettingService));
            _projectFactory = projectFactory ?? throw new ArgumentNullException(nameof(projectFactory));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
        }

        public ProjectOpenResult CreateNewProject()
        {
            var viewModel = new ProjectCreateWindowViewModel(_appSettingService);

            var window = new ProjectCreateWindow
            {
                DataContext = viewModel
            };

            if (_windowService.ShowDialog(window) != true || window.Result == null)
                return ProjectOpenResult.Cancel();

            try
            {
                var result = window.Result;
                var projectName = result.ProjectName.Trim();
                var projectFolder = result.ProjectFolder.Trim();

                Directory.CreateDirectory(projectFolder);

                var fileName = projectName + FileConstants.ProjectExtension;
                var filePath = Path.Combine(projectFolder, fileName);

                if (File.Exists(filePath))
                {
                    DialogService.ShowNotice(
                        Properties.Resources.Title_ProjectSaveFailed,
                        "같은 이름의 프로젝트 파일이 이미 존재합니다.");

                    return ProjectOpenResult.Fail();
                }

                var project = _projectFactory.Create(projectName);

                _fileService.Save(filePath, project);
                _recentProjectService.AddOrUpdate(filePath, project.ProjectName);

                return ProjectOpenResult.Success(project, filePath);
            }
            catch (Exception ex)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ProjectSaveFailed,
                    $"{Properties.Resources.Content_FileSaveFailedWithError}\n\n{ex.Message}");

                return ProjectOpenResult.Fail();
            }
        }

        public ProjectOpenResult OpenProject()
        {
            var dialog = new OpenFileDialog
            {
                Title = FileConstants.ProjectDialogTitle,
                Filter = FileConstants.ProjectFilter,
                DefaultExt = FileConstants.ProjectExtension,
                CheckFileExists = true,
                Multiselect = false
            };

            var owner = _windowService.GetMainWindow();

            if (dialog.ShowDialog(owner) != true)
                return ProjectOpenResult.Cancel();

            try
            {
                var project = _fileService.Load(dialog.FileName);

                _recentProjectService.AddOrUpdate(
                    dialog.FileName,
                    project.ProjectName);

                return ProjectOpenResult.Success(
                    project,
                    dialog.FileName);
            }
            catch (Exception ex)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_OpenProjectFailed,
                    $"{Properties.Resources.Content_FileOpenFailedWithError}\n\n{ex.Message}");

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
            catch (Exception ex)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ProjectSaveFailed,
                    $"{Properties.Resources.Content_FileSaveFailedWithError}\n\n{ex.Message}");

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

            var owner = _windowService.GetMainWindow();

            if (dialog.ShowDialog(owner) != true)
                return ProjectSaveResult.Cancel();

            try
            {
                _fileService.Save(dialog.FileName, project);
                _recentProjectService.AddOrUpdate(dialog.FileName, project.ProjectName);

                return ProjectSaveResult.Success(dialog.FileName);
            }
            catch (Exception ex)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ProjectSaveFailed,
                    $"{Properties.Resources.Content_FileSaveFailedWithError}\n\n{ex.Message}");

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