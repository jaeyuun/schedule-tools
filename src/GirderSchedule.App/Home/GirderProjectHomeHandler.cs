using GirderSchedule.App.Constants;
using GirderSchedule.App.Infrastructure;
using GirderSchedule.App.Services.Project;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.Views.Main;
using ScheduleTools.Wpf.FileDialogs.Contracts;
using ScheduleTools.Wpf.FileDialogs.Models;
using ScheduleTools.Wpf.Home.Contracts;
using ScheduleTools.Wpf.Home.Models;
using ScheduleTools.Wpf.Home.Views;
using ScheduleTools.Wpf.Windows.Contracts;
using System;
using System.IO;

namespace GirderSchedule.App.Home
{
    public sealed class GirderProjectHomeHandler : IProjectHomeHandler
    {
        private readonly ProjectFileService _projectFileService;
        private readonly ScheduleProjectFactory _projectFactory;
        private readonly IFileDialogService _fileDialogService;
        private readonly IWindowService _windowService;

        public GirderProjectHomeHandler(ProjectFileService projectFileService, ScheduleProjectFactory projectFactory, IFileDialogService fileDialogService, IWindowService windowService)
        {
            _projectFileService = projectFileService ?? throw new ArgumentNullException(nameof(projectFileService));
            _projectFactory = projectFactory ?? throw new ArgumentNullException(nameof(projectFactory));
            _fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
        }

        public string ProjectDialogTitle => FileConstants.ProjectOpenDialogTitle;

        public string ProjectFilter => FileConstants.ProjectFilter;

        public string DefaultExtension => FileConstants.ProjectExtension;

        public HomeProjectLaunchResult CreateProject()
        {
            var window = new ProjectCreateWindow
            {
                DataContext = AppServices.CreateProjectCreateWindowViewModel()
            };

            if (_windowService.ShowDialog(window) != true || window.Result == null)
                return HomeProjectLaunchResult.Cancel();

            try
            {
                var projectName = window.Result.ProjectName.Trim();
                var projectFolder = window.Result.ProjectFolder.Trim();

                Directory.CreateDirectory(projectFolder);

                var fileName = projectName + FileConstants.ProjectExtension;
                var filePath = Path.Combine(projectFolder, fileName);

                if (File.Exists(filePath))
                {
                    return HomeProjectLaunchResult.Failure(
                        "같은 이름의 프로젝트 파일이 이미 존재합니다.");
                }

                var project = _projectFactory.Create(projectName);

                _projectFileService.Save(filePath, project);

                var mainWindow = new MainWindow(project, filePath);

                return HomeProjectLaunchResult.Success(
                    project.ProjectName,
                    filePath,
                    mainWindow);
            }
            catch (Exception ex)
            {
                return HomeProjectLaunchResult.Failure(
                    $"{Properties.Resources.Content_FileSaveFailedWithError}\n\n{ex.Message}");
            }
        }

        public HomeProjectLaunchResult OpenProject()
        {
            var filePath = _fileDialogService.OpenFile(
                new OpenFileDialogOptions
                {
                    Title = FileConstants.ProjectOpenDialogTitle,
                    Filter = FileConstants.ProjectFilter,
                    DefaultExtension = FileConstants.ProjectExtension,
                    CheckFileExists = true,
                    CheckPathExists = true
                });

            return string.IsNullOrWhiteSpace(filePath)
                ? HomeProjectLaunchResult.Cancel()
                : OpenProject(filePath);
        }

        public HomeProjectLaunchResult OpenProject(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return HomeProjectLaunchResult.Failure("프로젝트 파일 경로가 비어 있습니다.");

            if (!File.Exists(filePath))
                return HomeProjectLaunchResult.Failure("프로젝트 파일을 찾을 수 없습니다.");

            try
            {
                var project = _projectFileService.Load(filePath);
                var mainWindow = new MainWindow(project, filePath);

                return HomeProjectLaunchResult.Success(
                    project.ProjectName,
                    filePath,
                    mainWindow);
            }
            catch (Exception ex)
            {
                return HomeProjectLaunchResult.Failure(
                    $"{Properties.Resources.Content_FileOpenFailedWithError}\n\n{ex.Message}");
            }
        }

        public void ShowMainWindow(HomeProjectLaunchResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            if (!result.IsSuccess || result.MainWindow == null)
                return;

            _windowService.SwitchMainWindow(result.MainWindow);
        }
    }
}