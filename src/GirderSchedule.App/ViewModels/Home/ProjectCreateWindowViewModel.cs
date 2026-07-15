using GirderSchedule.App.Commands;
using GirderSchedule.App.Constants;
using GirderSchedule.App.Services;
using GirderSchedule.App.Services.Project;
using GirderSchedule.App.Services.Settings;
using GirderSchedule.App.ViewModels.Main;
using GirderSchedule.App.Views.Main;
using GirderSchedule.Domain.Models;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace GirderSchedule.App.ViewModels.Home
{
    public sealed class ProjectCreateWindowViewModel : ViewModelBase
    {
        private readonly AppSettingService _settingService = new AppSettingService();
        private readonly ProjectFileService _projectFileService = new ProjectFileService();
        private readonly RecentProjectService _recentProjectService = new RecentProjectService();

        private string _projectName;
        private string _folderPath;

        public ScheduleProject CreatedProject { get; private set; }
        public string CreatedFilePath { get; private set; }

        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                if (_projectName == value)
                {
                    return;
                }

                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
            }
        }

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                if (_folderPath == value)
                {
                    return;
                }

                _folderPath = value;
                OnPropertyChanged(nameof(FolderPath));
            }
        }

        public ICommand BrowseCommand { get; private set; }
        public ICommand CreateCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public ProjectCreateWindowViewModel()
        {
            var setting = _settingService.Load();

            ProjectName = string.Empty;
            FolderPath = setting.DefaultProjectFolder;
            CreatedProject = null;
            CreatedFilePath = string.Empty;

            BrowseCommand = new RelayCommand(p => Browse());
            CreateCommand = new RelayCommand(p => CreateProject());
            BackCommand = new RelayCommand(p => CloseWindow());
        }

        private void Browse()
        {
            using var dialog = new Forms.FolderBrowserDialog
            {
                Description = Properties.Resources.Description_SelectProjectSaveFolder,
                SelectedPath = Directory.Exists(FolderPath) ? FolderPath : string.Empty,
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() != Forms.DialogResult.OK)
            {
                return;
            }

            FolderPath = dialog.SelectedPath;
        }

        private void CreateProject()
        {
            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_NewProject,
                    Properties.Resources.Content_InputProjectName);
                return;
            }

            if (string.IsNullOrWhiteSpace(FolderPath))
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_NewProject,
                    Properties.Resources.Content_SelectProjectFolder);
                return;
            }

            Directory.CreateDirectory(FolderPath);

            var fileName = SanitizeFileName(ProjectName) + FileConstants.ProjectExtension;
            var filePath = Path.Combine(FolderPath, fileName);

            if (File.Exists(filePath))
            {
                var result = AppDialogService.ShowConfirm(
                    Properties.Resources.Title_NewProject,
                    Properties.Resources.Content_ProjectAlreadyExistsOverwrite);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            var project = MainWindowViewModel.CreateNewProject(ProjectName);
            _projectFileService.Save(filePath, project);
            _recentProjectService.AddOrUpdate(filePath, project.ProjectName);

            CreatedProject = project;
            CreatedFilePath = filePath;

            var mainWindow = new MainWindow(project, filePath);
            ShowMainWindowOnly(mainWindow);
        }

        private void CloseWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);

            if (window == null)
            {
                return;
            }

            window.Close();
        }

        private void ShowMainWindowOnly(MainWindow mainWindow)
        {
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            var windows = Application.Current.Windows.Cast<Window>().Where(x => x != mainWindow).ToList();

            for (var i = 0; i < windows.Count; i++)
            {
                windows[i].Close();
            }
        }

        private string SanitizeFileName(string value)
        {
            var result = value.Trim();
            var invalidChars = Path.GetInvalidFileNameChars();

            for (var i = 0; i < invalidChars.Length; i++)
            {
                result = result.Replace(invalidChars[i].ToString(), string.Empty);
            }

            return string.IsNullOrWhiteSpace(result) ? FileConstants.DefaultProjectName : result;
        }
    }
}