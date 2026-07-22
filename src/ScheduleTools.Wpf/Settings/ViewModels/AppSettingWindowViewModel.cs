using Microsoft.Win32;
using ScheduleTools.Core.Settings;
using ScheduleTools.Wpf.Commands;
using ScheduleTools.Wpf.Mvvm;
using System.IO;
using System.Windows.Input;

namespace ScheduleTools.Wpf.Settings.ViewModels
{
    public sealed class AppSettingWindowViewModel : ViewModelBase
    {
        private readonly AppSettingService _settingService;
        private readonly double _originalFontSize;
        private string _defaultProjectFolder;
        private double _defaultFontSize;

        public event EventHandler<bool>? CloseRequested;

        public string VersionText { get; }

        public string DefaultProjectFolder
        {
            get => _defaultProjectFolder;
            set
            {
                if (_defaultProjectFolder == value)
                {
                    return;
                }

                _defaultProjectFolder = value;
                OnPropertyChanged(nameof(DefaultProjectFolder));
            }
        }

        public double DefaultFontSize
        {
            get => _defaultFontSize;
            private set
            {
                var normalizedValue = _settingService.NormalizeFontSize(value);

                if (Math.Abs(_defaultFontSize - normalizedValue) < 0.001)
                {
                    return;
                }

                _defaultFontSize = normalizedValue;

                OnPropertyChanged(nameof(DefaultFontSize));
                OnPropertyChanged(nameof(DefaultFontSizeValueText));
                OnPropertyChanged(nameof(CanDecreaseFontSize));
                OnPropertyChanged(nameof(CanIncreaseFontSize));

                AppFontSizeService.Apply(_defaultFontSize, _settingService.NormalizeFontSize);
            }
        }

        public string DefaultFontSizeValueText => DefaultFontSize.ToString("0");
        public bool CanDecreaseFontSize => DefaultFontSize > _settingService.MinFontSize;
        public bool CanIncreaseFontSize => DefaultFontSize < _settingService.MaxFontSize;

        public ICommand BrowseProjectFolderCommand { get; }
        public ICommand DecreaseFontSizeCommand { get; }
        public ICommand IncreaseFontSizeCommand { get; }
        public ICommand ResetFontSizeCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public AppSettingWindowViewModel(AppSettingService settingService, string versionText)
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            VersionText = versionText ?? string.Empty;

            var setting = _settingService.Load();

            _defaultProjectFolder = setting.DefaultProjectFolder;
            _defaultFontSize = _settingService.NormalizeFontSize(setting.DefaultFontSize);
            _originalFontSize = _defaultFontSize;

            BrowseProjectFolderCommand = new RelayCommand(_ => BrowseProjectFolder());
            DecreaseFontSizeCommand = new RelayCommand(_ => DecreaseFontSize());
            IncreaseFontSizeCommand = new RelayCommand(_ => IncreaseFontSize());
            ResetFontSizeCommand = new RelayCommand(_ => ResetFontSize());
            ConfirmCommand = new RelayCommand(_ => Confirm());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void BrowseProjectFolder()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "기본 저장 폴더 선택",
                InitialDirectory = Directory.Exists(DefaultProjectFolder) ? DefaultProjectFolder : string.Empty,
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            DefaultProjectFolder = dialog.FolderName;
        }

        private void DecreaseFontSize()
        {
            if (!CanDecreaseFontSize)
            {
                return;
            }

            DefaultFontSize--;
        }

        private void IncreaseFontSize()
        {
            if (!CanIncreaseFontSize)
            {
                return;
            }

            DefaultFontSize++;
        }

        private void ResetFontSize()
        {
            DefaultFontSize = _settingService.DefaultFontSize;
        }

        private void Confirm()
        {
            var folderPath = DefaultProjectFolder?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                folderPath = _settingService.ProjectPath;
            }

            var setting = _settingService.Load();

            setting.DefaultProjectFolder = folderPath;
            setting.DefaultFontSize = DefaultFontSize;

            _settingService.Save(setting);

            CloseRequested?.Invoke(this, true);
        }

        private void Cancel()
        {
            RestoreOriginalFontSize();
            CloseRequested?.Invoke(this, false);
        }

        public void RestoreOriginalFontSize()
        {
            AppFontSizeService.Apply(_originalFontSize, _settingService.NormalizeFontSize);
        }
    }
}