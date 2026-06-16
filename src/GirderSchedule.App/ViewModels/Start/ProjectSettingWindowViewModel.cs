using GirderSchedule.App.Commands;
using GirderSchedule.App.Services;
using GirderSchedule.App.Views;
using GirderSchedule.Domain.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace GirderSchedule.App.ViewModels.Start
{
    public sealed class ProjectSettingWindowViewModel : ViewModelBase
    {
        private readonly AppSettingService _settingService = new AppSettingService();
        private readonly ProjectFileService _projectFileService = new ProjectFileService();
        private readonly RecentProjectService _recentProjectService = new RecentProjectService();

        private string _projectName;
        private string _folderPath;

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

        public ProjectSettingWindowViewModel()
        {
            var setting = _settingService.Load();

            ProjectName = string.Empty;
            FolderPath = setting.DefaultProjectFolder;

            BrowseCommand = new RelayCommand(p => Browse());
            CreateCommand = new RelayCommand(p => CreateProject());
            BackCommand = new RelayCommand(p => CloseWindow(p));
        }

        private void Browse()
        {
            var dialog = new Forms.FolderBrowserDialog();
            dialog.Description = "프로젝트를 저장할 폴더를 선택하세요.";
            dialog.SelectedPath = FolderPath;

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
                MessageBox.Show("프로젝트 이름을 입력하세요.", "새 프로젝트", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(FolderPath))
            {
                MessageBox.Show("프로젝트 위치를 선택하세요.", "새 프로젝트", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Directory.CreateDirectory(FolderPath);

            var fileName = SanitizeFileName(ProjectName) + ".gsp";
            var filePath = Path.Combine(FolderPath, fileName);

            if (File.Exists(filePath))
            {
                var result = MessageBox.Show("같은 이름의 프로젝트 파일이 이미 있습니다.\r\n덮어쓰시겠습니까?", "새 프로젝트", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            var project = MainWindowViewModel.CreateNewProject(ProjectName);
            _projectFileService.Save(filePath, project);
            _recentProjectService.AddOrUpdate(filePath, project.ProjectName);

            var setting = _settingService.Load();
            setting.DefaultProjectFolder = FolderPath;
            _settingService.Save(setting);

            var mainWindow = new MainWindow(project, filePath);
            ShowMainWindowOnly(mainWindow);
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

        private void CloseWindow(object parameter)
        {
            CloseCurrentWindow();
        }

        private void CloseCurrentWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);

            if (window != null)
            {
                window.Close();
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

            return string.IsNullOrWhiteSpace(result) ? "새 프로젝트" : result;
        }
    }
}