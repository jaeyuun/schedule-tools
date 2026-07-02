using GirderSchedule.App.Commands;
using GirderSchedule.App.Services;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Settings;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels.Settings
{
    public sealed class DxfLayerSettingWindowViewModel : ViewModelBase
    {
        private readonly DxfSettingService _settingService;
        private readonly ScheduleProject _project;
        private string _selectedSettingName = string.Empty;
        private string _settingNameText = string.Empty;
        private string _selectedTemplateName = string.Empty;
        private bool _isTemplateInvalid;
        private DxfSettingProfile _loadedProfile;

        public event EventHandler Accepted;
        public event EventHandler Cancelled;

        public ObservableCollection<string> SettingNames { get; private set; }
        public ObservableCollection<string> TemplateNames { get; private set; }
        public ObservableCollection<DxfStyleItemViewModel> Styles { get; private set; }
        public ObservableCollection<DxfLayerItemViewModel> Layers { get; private set; }

        public ICommand AddTemplateCommand { get; private set; }
        public ICommand DeleteTemplateCommand { get; private set; }
        public ICommand SaveSettingCommand { get; private set; }
        public ICommand DeleteSettingCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        public string SelectedSettingName
        {
            get { return _selectedSettingName; }
            set
            {
                if (_selectedSettingName == value)
                {
                    return;
                }

                _selectedSettingName = value;
                SettingNameText = value;
                OnPropertyChanged(nameof(SelectedSettingName));
                LoadSelectedSetting();
            }
        }

        public string SettingNameText
        {
            get { return _settingNameText; }
            set
            {
                if (_settingNameText == value)
                {
                    return;
                }

                _settingNameText = value;
                OnPropertyChanged(nameof(SettingNameText));
            }
        }

        public string SelectedTemplateName
        {
            get { return _selectedTemplateName; }
            set
            {
                if (_selectedTemplateName == value)
                {
                    return;
                }

                _selectedTemplateName = value;
                IsTemplateInvalid = false;
                OnPropertyChanged(nameof(SelectedTemplateName));
            }
        }

        public bool IsTemplateInvalid
        {
            get { return _isTemplateInvalid; }
            set
            {
                if (_isTemplateInvalid == value)
                {
                    return;
                }

                _isTemplateInvalid = value;
                OnPropertyChanged(nameof(IsTemplateInvalid));
            }
        }

        public DxfLayerSettingWindowViewModel()
            : this(null, new DxfSettingService())
        {
        }

        public DxfLayerSettingWindowViewModel(ScheduleProject project)
            : this(project, new DxfSettingService())
        {
        }

        public DxfLayerSettingWindowViewModel(ScheduleProject project, DxfSettingService settingService)
        {
            _project = project;
            _settingService = settingService;

            SettingNames = new ObservableCollection<string>();
            TemplateNames = new ObservableCollection<string>();
            Styles = new ObservableCollection<DxfStyleItemViewModel>();
            Layers = new ObservableCollection<DxfLayerItemViewModel>();

            AddTemplateCommand = new RelayCommand(p => AddTemplate());
            DeleteTemplateCommand = new RelayCommand(p => DeleteTemplate());
            SaveSettingCommand = new RelayCommand(p => SaveSetting());
            DeleteSettingCommand = new RelayCommand(p => DeleteSetting());
            ResetCommand = new RelayCommand(p => Reset());
            CloseCommand = new RelayCommand(p => Close());

            Initialize();
        }

        public DxfSettingProfile CreateProfile()
        {
            var profile = new DxfSettingProfile();
            profile.SettingName = SelectedSettingName;
            profile.TemplateName = SelectedTemplateName;
            profile.StyleSetting = CreateStyleSetting();
            profile.LayerSettings = CreateLayerSettings();
            return profile;
        }

        public DxfStyleSetting CreateStyleSetting()
        {
            var setting = new DxfStyleSetting();

            foreach (var style in Styles)
            {
                if (style.Role == DxfStyleRole.TextStyle)
                {
                    setting.TextStyleName = style.StyleName;
                }
                else if (style.Role == DxfStyleRole.DrawingBlock)
                {
                    setting.DrawingBlockName = style.StyleName;
                }
                else if (style.Role == DxfStyleRole.DimensionStyle)
                {
                    setting.DimensionStyleName = style.StyleName;
                }
            }

            return setting;
        }

        public List<DxfLayerSetting> CreateLayerSettings()
        {
            var settings = new List<DxfLayerSetting>();

            foreach (var layer in Layers)
            {
                settings.Add(new DxfLayerSetting(layer.Role, layer.DisplayName, layer.LayerName));
            }

            return settings;
        }

        private void Initialize()
        {
            _settingService.EnsureDefaultStorage();
            RefreshTemplateNames();
            RefreshSettingNames();

            var settingName = _project == null ? _settingService.DefaultSetting : _project.DxfSettingName;
            settingName = _settingService.NormalizeSettingName(settingName);

            if (_project != null && _project.DxfSettingName != settingName)
            {
                _project.DxfSettingName = settingName;
            }

            SelectedSettingName = settingName;
        }

        private void RefreshSettingNames()
        {
            SettingNames.Clear();

            foreach (var name in _settingService.GetSettingNames())
            {
                SettingNames.Add(name);
            }
        }

        private void RefreshTemplateNames()
        {
            TemplateNames.Clear();

            foreach (var name in _settingService.GetTemplateNames())
            {
                TemplateNames.Add(name);
            }
        }

        private void LoadSelectedSetting()
        {
            if (string.IsNullOrWhiteSpace(SelectedSettingName))
            {
                return;
            }

            var profile = _settingService.LoadSetting(SelectedSettingName);
            ApplyProfile(profile);
        }

        private void ApplyProfile(DxfSettingProfile profile)
        {
            if (profile == null)
            {
                return;
            }

            SelectedTemplateName = profile.TemplateName;

            Styles.Clear();

            if (profile.StyleSetting != null)
            {
                Styles.Add(new DxfStyleItemViewModel(DxfStyleRole.TextStyle, "글꼴", profile.StyleSetting.TextStyleName));
                Styles.Add(new DxfStyleItemViewModel(DxfStyleRole.DrawingBlock, "도면", profile.StyleSetting.DrawingBlockName));
                Styles.Add(new DxfStyleItemViewModel(DxfStyleRole.DimensionStyle, "치수선", profile.StyleSetting.DimensionStyleName));
            }

            Layers.Clear();

            if (profile.LayerSettings != null)
            {
                foreach (var layer in profile.LayerSettings)
                {
                    Layers.Add(new DxfLayerItemViewModel(layer.Role, layer.DisplayName, layer.LayerName));
                }
            }

            _loadedProfile = CreateProfileSnapshot(profile);
        }

        private void AddTemplate()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "DXF 템플릿 선택";
            dialog.Filter = "DXF 파일 (*.dxf)|*.dxf";
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            var templateName = _settingService.AddTemplate(dialog.FileName);

            if (string.IsNullOrWhiteSpace(templateName))
            {
                AppDialogService.ShowNotice("DXF 설정", "템플릿을 추가하지 못했습니다.");
                return;
            }

            RefreshTemplateNames();
            SelectedTemplateName = templateName;
        }

        private void DeleteTemplate()
        {
            if (!_settingService.DeleteTemplate(SelectedTemplateName))
            {
                AppDialogService.ShowNotice("DXF 설정", "기본 템플릿은 삭제할 수 없습니다.");
                return;
            }

            RefreshTemplateNames();
            SelectedTemplateName = _settingService.GetTemplateNames()[0];
        }

        private void SaveSetting()
        {
            var settingName = NormalizeSettingNameText(SettingNameText);

            if (string.IsNullOrWhiteSpace(settingName))
            {
                AppDialogService.ShowNotice("DXF 설정", "설정 이름을 입력해 주세요.");
                return;
            }

            var profile = CreateProfile();
            profile.SettingName = settingName;

            if (!_settingService.Validate(profile).IsValid)
            {
                AppDialogService.ShowNotice("DXF 설정", "유효하지 않은 템플릿, 스타일명, 레이어명이 있습니다.");
                return;
            }

            _settingService.SaveSetting(profile);
            _loadedProfile = CreateProfileSnapshot(profile);

            RefreshSettingNames();
            SelectedSettingName = settingName;
            SettingNameText = settingName;
        }

        private string NormalizeSettingNameText(string settingName)
        {
            if (string.IsNullOrWhiteSpace(settingName))
            {
                return string.Empty;
            }

            return settingName.Trim();
        }

        private void DeleteSetting()
        {
            if (!_settingService.DeleteSetting(SelectedSettingName))
            {
                AppDialogService.ShowNotice("DXF 설정", "설정을 삭제하지 못했습니다.");
                return;
            }

            RefreshSettingNames();
            SelectedSettingName = _settingService.DefaultSetting;
        }

        private void Reset()
        {
            var profile = _settingService.LoadSetting(_settingService.DefaultSetting);
            ApplyProfile(profile);
        }

        private void Close()
        {
            if (!ValidateCurrentSetting())
            {
                AppDialogService.ShowNotice("DXF 설정", "유효하지 않은 템플릿, 스타일명, 레이어명이 있습니다.");
                return;
            }

            var profile = CreateProfile();

            if (HasChanges(profile))
            {
                _settingService.SaveSetting(profile);
                _loadedProfile = CreateProfileSnapshot(profile);
            }

            if (_project != null)
            {
                _project.DxfSettingName = SelectedSettingName;
            }

            if (Accepted != null)
            {
                Accepted(this, EventArgs.Empty);
            }
        }

        private bool HasChanges(DxfSettingProfile currentProfile)
        {
            if (_loadedProfile == null || currentProfile == null)
            {
                return true;
            }

            if (!string.Equals(_loadedProfile.SettingName, currentProfile.SettingName, StringComparison.Ordinal))
            {
                return true;
            }

            if (!string.Equals(_loadedProfile.TemplateName, currentProfile.TemplateName, StringComparison.Ordinal))
            {
                return true;
            }

            if (HasStyleChanges(_loadedProfile.StyleSetting, currentProfile.StyleSetting))
            {
                return true;
            }

            if (HasLayerChanges(_loadedProfile.LayerSettings, currentProfile.LayerSettings))
            {
                return true;
            }

            return false;
        }

        private bool HasStyleChanges(DxfStyleSetting original, DxfStyleSetting current)
        {
            if (original == null || current == null)
            {
                return original != current;
            }

            if (!string.Equals(original.TextStyleName, current.TextStyleName, StringComparison.Ordinal))
            {
                return true;
            }

            if (!string.Equals(original.DrawingBlockName, current.DrawingBlockName, StringComparison.Ordinal))
            {
                return true;
            }

            if (!string.Equals(original.DimensionStyleName, current.DimensionStyleName, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        private bool HasLayerChanges(List<DxfLayerSetting> original, List<DxfLayerSetting> current)
        {
            if (original == null || current == null)
            {
                return original != current;
            }

            if (original.Count != current.Count)
            {
                return true;
            }

            for (var i = 0; i < original.Count; i++)
            {
                if (original[i].Role != current[i].Role)
                {
                    return true;
                }

                if (!string.Equals(original[i].DisplayName, current[i].DisplayName, StringComparison.Ordinal))
                {
                    return true;
                }

                if (!string.Equals(original[i].LayerName, current[i].LayerName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ValidateCurrentSetting()
        {
            ClearInvalidMarks();

            var profile = CreateProfile();
            var result = _settingService.Validate(profile);

            if (!string.IsNullOrWhiteSpace(result.TemplateError))
            {
                IsTemplateInvalid = true;
            }

            foreach (var invalidStyle in result.InvalidStyles)
            {
                foreach (var style in Styles)
                {
                    if (style.DisplayName == invalidStyle)
                    {
                        style.IsInvalid = true;
                    }
                }
            }

            foreach (var invalidLayer in result.InvalidLayers)
            {
                foreach (var layer in Layers)
                {
                    if (layer.DisplayName == invalidLayer)
                    {
                        layer.IsInvalid = true;
                    }
                }
            }

            return result.IsValid;
        }

        private void ClearInvalidMarks()
        {
            IsTemplateInvalid = false;

            foreach (var style in Styles)
            {
                style.IsInvalid = false;
            }

            foreach (var layer in Layers)
            {
                layer.IsInvalid = false;
            }
        }

        private DxfSettingProfile CreateProfileSnapshot(DxfSettingProfile profile)
        {
            if (profile == null)
            {
                return null;
            }

            var snapshot = new DxfSettingProfile();
            snapshot.SettingName = profile.SettingName;
            snapshot.TemplateName = profile.TemplateName;

            if (profile.StyleSetting != null)
            {
                snapshot.StyleSetting = new DxfStyleSetting();
                snapshot.StyleSetting.TextStyleName = profile.StyleSetting.TextStyleName;
                snapshot.StyleSetting.DrawingBlockName = profile.StyleSetting.DrawingBlockName;
                snapshot.StyleSetting.DimensionStyleName = profile.StyleSetting.DimensionStyleName;
            }

            snapshot.LayerSettings = new List<DxfLayerSetting>();

            if (profile.LayerSettings != null)
            {
                foreach (var layer in profile.LayerSettings)
                {
                    snapshot.LayerSettings.Add(new DxfLayerSetting(layer.Role, layer.DisplayName, layer.LayerName));
                }
            }

            return snapshot;
        }
    }
}