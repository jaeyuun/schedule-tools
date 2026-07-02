using GirderSchedule.App.Constants;
using GirderSchedule.App.Services.Ui;
using GirderSchedule.App.ViewModels.Home;
using GirderSchedule.App.ViewModels.Settings;
using GirderSchedule.App.Views.Home;
using GirderSchedule.App.Views.Settings;
using GirderSchedule.Domain.Models;
using Microsoft.Win32;
using System.Windows;

namespace GirderSchedule.App.Services.Project
{
    public sealed class ProjectService
    {
        private readonly ProjectFileService _fileService;
        private readonly RecentProjectService _recentProjectService;

        public ProjectService(ProjectFileService fileService, RecentProjectService recentProjectService)
        {
            _fileService = fileService;
            _recentProjectService = recentProjectService;
        }

        public ProjectCreateResult CreateNewProject()
        {
            var window = new ProjectCreateWindow();
            window.Owner = Application.Current.MainWindow;

            if (window.ShowDialog() != true)
            {
                return ProjectCreateResult.Cancel();
            }

            var viewModel = window.DataContext as ProjectCreateWindowViewModel;

            if (viewModel == null || viewModel.CreatedProject == null)
            {
                return ProjectCreateResult.Cancel();
            }

            return ProjectCreateResult.Success(viewModel.CreatedProject, viewModel.CreatedFilePath);
        }

        public ProjectOpenResult OpenProject()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = FileConstants.ProjectDialogTitle;
            dialog.Filter = FileConstants.ProjectFilter;
            dialog.DefaultExt = FileConstants.ProjectExtension;

            if (dialog.ShowDialog() != true)
            {
                return ProjectOpenResult.Cancel();
            }

            try
            {
                var project = _fileService.Load(dialog.FileName);
                _recentProjectService.AddOrUpdate(dialog.FileName, project.ProjectName);
                return ProjectOpenResult.Success(project, dialog.FileName);
            }
            catch
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_OpenProjectFailed
                    , Properties.Resources.Content_FileOpenFailedWithError);
                return ProjectOpenResult.Fail();
            }
        }

        public ProjectSaveResult SaveProject(ScheduleProject project, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return SaveProjectAs(project);
            }

            try
            {
                _fileService.Save(filePath, project);
                _recentProjectService.AddOrUpdate(filePath, project.ProjectName);
                return ProjectSaveResult.Success(filePath);
            }
            catch
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ProjectSaveFailed,
                    Properties.Resources.Content_FileSaveFailedWithError);
                return ProjectSaveResult.Fail();
            }
        }

        public ProjectSaveResult SaveProjectAs(ScheduleProject project)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = FileConstants.ProjectDialogTitle;
            dialog.Filter = FileConstants.ProjectFilter;
            dialog.DefaultExt = FileConstants.ProjectExtension;
            dialog.AddExtension = true;
            dialog.FileName = string.IsNullOrWhiteSpace(project.ProjectName) ? FileConstants.DefaultProjectFileName : project.ProjectName + FileConstants.ProjectExtension;

            if (dialog.ShowDialog() != true)
            {
                return ProjectSaveResult.Cancel();
            }

            try
            {
                _fileService.Save(dialog.FileName, project);
                _recentProjectService.AddOrUpdate(dialog.FileName, project.ProjectName);
                return ProjectSaveResult.Success(dialog.FileName);
            }
            catch
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ProjectSaveFailed, 
                    Properties.Resources.Content_FileSaveFailedWithError);
                return ProjectSaveResult.Fail();
            }
        }
    }

    public sealed class ProjectCreateResult
    {
        public bool IsSuccess { get; private set; }
        public ScheduleProject Project { get; private set; }
        public string FilePath { get; private set; }

        private ProjectCreateResult()
        {
            FilePath = string.Empty;
        }

        public static ProjectCreateResult Success(ScheduleProject project, string filePath)
        {
            return new ProjectCreateResult
            {
                IsSuccess = true,
                Project = project,
                FilePath = filePath ?? string.Empty
            };
        }

        public static ProjectCreateResult Cancel()
        {
            return new ProjectCreateResult();
        }
    }

    public sealed class ProjectOpenResult
    {
        public bool IsSuccess { get; private set; }
        public ScheduleProject Project { get; private set; }
        public string FilePath { get; private set; }

        private ProjectOpenResult()
        {
            FilePath = string.Empty;
        }

        public static ProjectOpenResult Success(ScheduleProject project, string filePath)
        {
            return new ProjectOpenResult
            {
                IsSuccess = true,
                Project = project,
                FilePath = filePath ?? string.Empty
            };
        }

        public static ProjectOpenResult Cancel()
        {
            return new ProjectOpenResult();
        }

        public static ProjectOpenResult Fail()
        {
            return new ProjectOpenResult();
        }
    }

    public sealed class ProjectSaveResult
    {
        public bool IsSuccess { get; private set; }
        public string FilePath { get; private set; }

        private ProjectSaveResult()
        {
            FilePath = string.Empty;
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
            return new ProjectSaveResult();
        }

        public static ProjectSaveResult Fail()
        {
            return new ProjectSaveResult();
        }
    }
}