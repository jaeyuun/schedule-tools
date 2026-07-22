using ScheduleTools.Core.Settings;
using ScheduleTools.Wpf.Commands;
using ScheduleTools.Wpf.FileDialogs.Contracts;
using ScheduleTools.Wpf.FileDialogs.Models;
using ScheduleTools.Wpf.Mvvm;
using ScheduleTools.Wpf.Home.Models;
using System.Windows.Input;

namespace ScheduleTools.Wpf.Home.ViewModels
{
    public sealed class ProjectCreateWindowViewModel : ViewModelBase
    {
        private readonly AppSettingService _appSettingService;
        private readonly IFolderDialogService _folderDialogService;

        private string _projectName = "NewProject";
        private string _projectFolder = string.Empty;

        public event EventHandler<bool>? CloseRequested;

        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (!SetProperty(ref _projectName, value))
                    return;

                OnPropertyChanged(nameof(CanCreate));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string ProjectFolder
        {
            get => _projectFolder;
            set
            {
                if (!SetProperty(ref _projectFolder, value))
                    return;

                OnPropertyChanged(nameof(CanCreate));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool CanCreate =>
            !string.IsNullOrWhiteSpace(ProjectName) &&
            !string.IsNullOrWhiteSpace(ProjectFolder);

        public ICommand BrowseFolderCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ConfirmCommand { get; }

        public ProjectCreateWindowViewModel(AppSettingService appSettingService, IFolderDialogService folderDialogService)
        {
            _appSettingService = appSettingService ?? throw new ArgumentNullException(nameof(appSettingService));
            _folderDialogService = folderDialogService ?? throw new ArgumentNullException(nameof(folderDialogService));

            ProjectFolder = _appSettingService.ProjectPath;

            BrowseFolderCommand = new RelayCommand(_ => BrowseFolder());
            CancelCommand = new RelayCommand(_ => CloseRequested?.Invoke(this, false));
            ConfirmCommand = new RelayCommand(_ => Confirm(), _ => CanCreate);
        }

        public ProjectCreateResult CreateResult()
        {
            return new ProjectCreateResult
            {
                ProjectName = ProjectName.Trim(),
                ProjectFolder = ProjectFolder.Trim()
            };
        }

        private void BrowseFolder()
        {
            var selectedFolder = _folderDialogService.SelectFolder(
                new FolderDialogOptions
                {
                    Title = "프로젝트 폴더 선택",
                    InitialDirectory = ProjectFolder
                });

            if (!string.IsNullOrWhiteSpace(selectedFolder))
                ProjectFolder = selectedFolder;
        }

        private void Confirm()
        {
            ProjectName = ProjectName.Trim();
            ProjectFolder = ProjectFolder.Trim();

            if (!CanCreate)
                return;

            CloseRequested?.Invoke(this, true);
        }
    }
}