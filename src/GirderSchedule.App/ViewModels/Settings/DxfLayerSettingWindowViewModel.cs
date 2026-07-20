using GirderSchedule.App.Commands;
using GirderSchedule.App.Constants;
using GirderSchedule.App.Services;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Constants;
using Microsoft.Win32;
using netDxf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        private bool _isApplyingProfile;

        public event EventHandler Accepted;

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

                if (!_isApplyingProfile)
                {
                    ApplyBlockStyleFromSelectedTemplate();
                }
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
            profile.StyleSettings = CreateStyleSettings();
            profile.LayerSettings = CreateLayerSettings();
            return profile;
        }

        public List<DxfStyleSetting> CreateStyleSettings()
        {
            var settings = new List<DxfStyleSetting>();

            foreach (var style in Styles)
            {
                settings.Add(new DxfStyleSetting(style.Role, style.DisplayName, style.StyleName));
            }

            return settings;
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

            var settingName = _project == null ? FileConstants.DxfSettingName : _project.DxfSettingName;
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

            _isApplyingProfile = true;

            try
            {
                SelectedTemplateName = profile.TemplateName;

                Styles.Clear();

                if (profile.StyleSettings != null)
                {
                    foreach (var style in profile.StyleSettings)
                    {
                        Styles.Add(new DxfStyleItemViewModel(style.Role, style.RoleName, style.StyleName));
                    }
                }

                Layers.Clear();

                if (profile.LayerSettings != null)
                {
                    foreach (var layer in profile.LayerSettings)
                    {
                        Layers.Add(new DxfLayerItemViewModel(layer.Role, layer.RoleName, layer.LayerName));
                    }
                }

                _loadedProfile = CreateProfileSnapshot(profile);
            }
            finally
            {
                _isApplyingProfile = false;
            }
        }

        private void AddTemplate()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = FileConstants.DxfOpenDialogTitle;
            dialog.Filter = FileConstants.DxfFilter;
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            var templateName = _settingService.AddTemplate(dialog.FileName);

            if (string.IsNullOrWhiteSpace(templateName))
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_DxfSetting,
                    Properties.Resources.Content_DxfTemplateAddFailed);
                return;
            }

            RefreshTemplateNames();
            SelectedTemplateName = templateName;
        }

        private void ApplyBlockStyleFromSelectedTemplate()
        {
            if (string.IsNullOrWhiteSpace(SelectedTemplateName))
            {
                return;
            }

            var templatePath = _settingService.GetTemplatePath(SelectedTemplateName);
            ApplyBlockStyleFromTemplate(templatePath);
        }

        private void ApplyBlockStyleFromTemplate(string templatePath)
        {
            if (string.IsNullOrWhiteSpace(templatePath) || !File.Exists(templatePath))
            {
                return;
            }

            try
            {
                var document = DxfDocument.Load(templatePath);

                if (document == null)
                {
                    return;
                }

                var blockName = GetFirstInsertedBlockName(document);
                var blockStyleItem = Styles.FirstOrDefault(x => x.Role == DxfStyleRole.Block);

                if (blockStyleItem != null)
                {
                    blockStyleItem.StyleName = blockName;
                }
            }
            catch
            {
                // 템플릿 선택은 유지하고 블록명 읽기 실패만 무시합니다.
            }
        }

        private static string GetFirstInsertedBlockName(DxfDocument document)
        {
            if (document == null)
            {
                return DxfBlocks.TitleBlock;
            }

            return document.Entities.Inserts
                .Where(x => x?.Block != null && !string.IsNullOrWhiteSpace(x.Block.Name))
                .Select(x => x.Block.Name)
                .FirstOrDefault() ?? DxfBlocks.TitleBlock;
        }

        private void DeleteTemplate()
        {
            if (!_settingService.DeleteTemplate(SelectedTemplateName))
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_DxfSetting,
                    Properties.Resources.Content_DxfTemplateDeleteFailed);
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
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_DxfSetting,
                    Properties.Resources.Content_DxfSettingNameSet);
                return;
            }

            var profile = CreateProfile();
            profile.SettingName = settingName;

            if (!_settingService.Validate(profile).IsValid)
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_DxfSetting,
                    Properties.Resources.Content_DxfSettingNameError);
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
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_DxfSetting,
                    Properties.Resources.Content_DxfSettingDeleteFailed);
                return;
            }

            RefreshSettingNames();
            SelectedSettingName = FileConstants.DxfSettingName;
        }

        private void Reset()
        {
            var profile = _settingService.CreateDefaultSettingProfile();
            ApplyProfile(profile);
        }

        private void Close()
        {
            if (!ValidateCurrentSetting())
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_DxfSetting,
                    Properties.Resources.Content_DxfSettingNameError);
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

            if (HasStyleChanges(_loadedProfile.StyleSettings, currentProfile.StyleSettings))
            {
                return true;
            }

            if (HasLayerChanges(_loadedProfile.LayerSettings, currentProfile.LayerSettings))
            {
                return true;
            }

            return false;
        }

        private bool HasStyleChanges(List<DxfStyleSetting> original, List<DxfStyleSetting> current)
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

                if (!string.Equals(original[i].RoleName, current[i].RoleName, StringComparison.Ordinal))
                {
                    return true;
                }

                if (!string.Equals(original[i].StyleName, current[i].StyleName, StringComparison.Ordinal))
                {
                    return true;
                }
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

                if (!string.Equals(original[i].RoleName, current[i].RoleName, StringComparison.Ordinal))
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

            snapshot.StyleSettings = new List<DxfStyleSetting>();

            if (profile.StyleSettings != null)
            {
                foreach (var style in profile.StyleSettings)
                {
                    snapshot.StyleSettings.Add(new DxfStyleSetting(style.Role, style.RoleName, style.StyleName));
                }
            }

            snapshot.LayerSettings = new List<DxfLayerSetting>();

            if (profile.LayerSettings != null)
            {
                foreach (var layer in profile.LayerSettings)
                {
                    snapshot.LayerSettings.Add(new DxfLayerSetting(layer.Role, layer.RoleName, layer.LayerName));
                }
            }

            return snapshot;
        }
    }
}