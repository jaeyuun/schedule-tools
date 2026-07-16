using GirderSchedule.App.Commands;
using GirderSchedule.App.Services.Settings;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace GirderSchedule.App.ViewModels.Settings
{
    public sealed class AppSettingWindowViewModel : ViewModelBase
    {
        private readonly AppSettingService _settingService;
        private readonly double _originalFontSize;
        private string _defaultProjectFolder;
        private double _defaultFontSize;

        public event EventHandler<bool> CloseRequested;

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
                var normalizedValue = AppSettingService.NormalizeFontSize(value);

                if (Math.Abs(_defaultFontSize - normalizedValue) < 0.001)
                {
                    return;
                }

                _defaultFontSize = normalizedValue;
                OnPropertyChanged(nameof(DefaultFontSize));
                OnPropertyChanged(nameof(DefaultFontSizeText));
                OnPropertyChanged(nameof(CanDecreaseFontSize));
                OnPropertyChanged(nameof(CanIncreaseFontSize));

                AppSettingService.ApplyFontSize(_defaultFontSize);
            }
        }

        public string DefaultFontSizeText => DefaultFontSize.ToString("0");
        public string VersionText { get; } = $"버전 {Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "정보 없음"}";
        public bool CanDecreaseFontSize => DefaultFontSize > AppSettingService.MinFontSize;
        public bool CanIncreaseFontSize => DefaultFontSize < AppSettingService.MaxFontSize;

        public ICommand BrowseProjectFolderCommand { get; }
        public ICommand DecreaseFontSizeCommand { get; }
        public ICommand IncreaseFontSizeCommand { get; }
        public ICommand ResetFontSizeCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public AppSettingWindowViewModel()
        {
            _settingService = new AppSettingService();

            var setting = _settingService.Load();

            _defaultProjectFolder = setting.DefaultProjectFolder;
            _defaultFontSize = AppSettingService.NormalizeFontSize(setting.DefaultFontSize);
            _originalFontSize = _defaultFontSize;

            BrowseProjectFolderCommand = new RelayCommand(p => BrowseProjectFolder());
            DecreaseFontSizeCommand = new RelayCommand(p => DecreaseFontSize());
            IncreaseFontSizeCommand = new RelayCommand(p => IncreaseFontSize());
            ResetFontSizeCommand = new RelayCommand(p => ResetFontSize());
            ConfirmCommand = new RelayCommand(p => Confirm());
            CancelCommand = new RelayCommand(p => Cancel());
        }

        private void BrowseProjectFolder()
        {
            using var dialog = new Forms.FolderBrowserDialog
            {
                Description = "새 프로젝트의 기본 저장 폴더를 선택하세요.",
                SelectedPath = Directory.Exists(DefaultProjectFolder) ? DefaultProjectFolder : string.Empty,
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() != Forms.DialogResult.OK)
            {
                return;
            }

            DefaultProjectFolder = dialog.SelectedPath;
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
            DefaultFontSize = AppSettingService.DefaultFontSize;
        }

        private void Confirm()
        {
            var folderPath = DefaultProjectFolder?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                folderPath = AppSettingService.GetDefaultProjectFolder();
            }

            var setting = _settingService.Load();
            setting.DefaultProjectFolder = folderPath;
            setting.DefaultFontSize = DefaultFontSize;

            _settingService.Save(setting);

            CloseRequested?.Invoke(this, true);
        }

        private void Cancel()
        {
            AppSettingService.ApplyFontSize(_originalFontSize);
            CloseRequested?.Invoke(this, false);
        }
    }
}