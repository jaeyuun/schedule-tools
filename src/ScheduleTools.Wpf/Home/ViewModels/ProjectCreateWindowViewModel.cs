using ScheduleTools.Core.Settings;
using ScheduleTools.Wpf.Commands;
using ScheduleTools.Wpf.Mvvm;
using ScheduleTools.Wpf.ProjectCreate.Models;
using System.IO;
using System.Windows.Input;

namespace ScheduleTools.Wpf.ProjectCreate.ViewModels
{
    public sealed class ProjectCreateWindowViewModel : ViewModelBase
    {
        private readonly AppSettingService _appSettingService;

        private string _projectName = string.Empty;
        private string _projectFolder = string.Empty;

        public event EventHandler<bool>? CloseRequested;
        public event EventHandler? BrowseFolderRequested;

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

        public ProjectCreateWindowViewModel(AppSettingService appSettingService)
        {
            _appSettingService = appSettingService ?? throw new ArgumentNullException(nameof(appSettingService));

            ProjectFolder = _appSettingService.GetDefaultProjectFolder();

            BrowseFolderCommand = new RelayCommand(_ => BrowseFolderRequested?.Invoke(this, EventArgs.Empty));
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

        private void Confirm()
        {
            ProjectName = ProjectName.Trim();
            ProjectFolder = ProjectFolder.Trim();

            if (!CanCreate)
                return;

            Directory.CreateDirectory(ProjectFolder);
            CloseRequested?.Invoke(this, true);
        }
    }
}