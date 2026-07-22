using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Constants;
using GirderSchedule.Dxf.Documents;
using netDxf;
using ScheduleTools.Dxf.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class DxfSettingService
    {
        private readonly DxfSettingFileStore<DxfSettingProfile> _settingStore;
        private readonly DxfTemplateStore _templateStore;

        public DxfSettingService(DxfStorageOptions options)
        {
            _templateStore = new DxfTemplateStore(options);
            _settingStore = new DxfSettingFileStore<DxfSettingProfile>(options, CreateDefaultSettingProfile);
        }

        public void EnsureDefaultStorage()
        {
            _settingStore.EnsureDirectory();
            _templateStore.EnsureDirectory();
            EnsureDefaultSetting();
        }

        public List<string> GetSettingNames()
        {
            EnsureDefaultStorage();
            return _settingStore.GetNames();
        }

        public List<string> GetTemplateNames()
        {
            EnsureDefaultStorage();
            return _templateStore.GetNames();
        }

        public bool SettingExists(string settingName)
        {
            return _settingStore.Exists(settingName);
        }

        public string NormalizeSettingName(string settingName)
        {
            return _settingStore.NormalizeName(settingName);
        }

        public DxfSettingProfile LoadSetting(string settingName)
        {
            EnsureDefaultStorage();

            settingName = NormalizeSettingName(settingName);
            var profile = _settingStore.Load(settingName);
            EnsureProfile(profile, settingName);
            return profile;
        }

        public void SaveSetting(DxfSettingProfile profile)
        {
            EnsureDefaultStorage();
            SaveSettingCore(profile);
        }

        public string AddTemplate(string sourceTemplatePath)
        {
            EnsureDefaultStorage();
            return _templateStore.Add(sourceTemplatePath);
        }

        public bool DeleteTemplate(string templateName)
        {
            if (!_templateStore.Delete(templateName)) return false;

            ReassignDeletedTemplate(templateName);
            return true;
        }

        public bool DeleteSetting(string settingName)
        {
            return _settingStore.Delete(settingName);
        }

        public string GetTemplatePath(string templateName)
        {
            return _templateStore.GetPath(templateName);
        }

        public DxfSettingProfile CreateDefaultSettingProfile()
        {
            return new DxfSettingProfile
            {
                SettingName = _settingStore.DefaultSettingName,
                TemplateName = _templateStore.DefaultTemplateName,
                StyleSettings = CreateDefaultStyleSettings(),
                LayerSettings = CreateDefaultLayerSettings()
            };
        }

        public List<DxfStyleSetting> CreateDefaultStyleSettings()
        {
            return new List<DxfStyleSetting>
            {
                new DxfStyleSetting(DxfStyleRole.Text, "글꼴", DxfStyles.DefaultText),
                new DxfStyleSetting(DxfStyleRole.Block, "도면", DxfBlocks.TitleBlock),
                new DxfStyleSetting(DxfStyleRole.Dimension, "치수선", DxfStyles.Dimension),
            };
        }

        public List<DxfLayerSetting> CreateDefaultLayerSettings()
        {
            return new List<DxfLayerSetting>
            {
                new DxfLayerSetting(DxfLayerRole.FormLine, "표", DxfLayers.FormLine),
                new DxfLayerSetting(DxfLayerRole.FormText, "표 텍스트", DxfLayers.FormText),
                new DxfLayerSetting(DxfLayerRole.MemberForce, "부재력", DxfLayers.Defpoint),
                new DxfLayerSetting(DxfLayerRole.Girder, "보 외곽", DxfLayers.RcGir),
                new DxfLayerSetting(DxfLayerRole.Rebar, "주근", DxfLayers.Rebar),
                new DxfLayerSetting(DxfLayerRole.Stirrup, "스터럽", DxfLayers.Rebar),
                new DxfLayerSetting(DxfLayerRole.Text, "텍스트", DxfLayers.Text),
                new DxfLayerSetting(DxfLayerRole.Dimension, "치수선", DxfLayers.Dim)
            };
        }

        public DxfSettingValidationResult Validate(DxfSettingProfile profile)
        {
            var result = new DxfSettingValidationResult();

            if (!ValidateProfileBase(profile, result))
            {
                return result;
            }

            try
            {
                var document = LoadTemplateDocument(profile.TemplateName);

                ValidateStyleSettings(document, profile.StyleSettings, result);
                ValidateLayerSettings(document, profile.LayerSettings, result);
            }
            catch
            {
                result.TemplateError = "템플릿 파일을 읽을 수 없습니다.";
            }

            return result;
        }

        private void SaveSettingCore(DxfSettingProfile profile)
        {
            profile ??= CreateDefaultSettingProfile();

            EnsureProfile(profile, profile.SettingName);
            _settingStore.Save(profile.SettingName, profile);
        }

        private bool ValidateProfileBase(DxfSettingProfile profile, DxfSettingValidationResult result)
        {
            if (profile == null)
            {
                result.TemplateError = "설정이 비어 있습니다.";
                return false;
            }

            var templatePath = GetTemplatePath(profile.TemplateName);

            if (string.IsNullOrWhiteSpace(profile.TemplateName) || !File.Exists(templatePath))
            {
                result.TemplateError = "템플릿 파일을 찾을 수 없습니다.";
                return false;
            }

            if (profile.StyleSettings == null)
            {
                result.TemplateError = "스타일 설정이 비어 있습니다.";
                return false;
            }

            if (profile.LayerSettings == null)
            {
                result.TemplateError = "레이어 설정이 비어 있습니다.";
                return false;
            }

            return true;
        }

        private DxfDocument LoadTemplateDocument(string templateName)
        {
            var templatePath = GetTemplatePath(templateName);
            var loader = new DxfDocumentLoader();
            var template = loader.LoadTemplate(templatePath);

            return template.Document;
        }

        private static void ValidateStyleSettings(DxfDocument document, List<DxfStyleSetting> styles, DxfSettingValidationResult result)
        {
            foreach (var style in styles)
            {
                if (!ContainsDxfStyle(document, style))
                {
                    result.InvalidStyles.Add(style.RoleName);
                }
            }
        }

        private static void ValidateLayerSettings(DxfDocument document, List<DxfLayerSetting> layers, DxfSettingValidationResult result)
        {
            foreach (var layer in layers)
            {
                if (string.IsNullOrWhiteSpace(layer.LayerName) || !document.Layers.Contains(layer.LayerName))
                {
                    result.InvalidLayers.Add(layer.RoleName);
                }
            }
        }

        private static bool ContainsDxfStyle(DxfDocument document, DxfStyleSetting style)
        {
            if (document == null || style == null || string.IsNullOrWhiteSpace(style.StyleName))
            {
                return false;
            }

            switch (style.Role)
            {
                case DxfStyleRole.Text:
                    return document.TextStyles.Contains(style.StyleName);

                case DxfStyleRole.Block:
                    return document.Blocks.Contains(style.StyleName);

                case DxfStyleRole.Dimension:
                    return document.DimensionStyles.Contains(style.StyleName);

                default:
                    return false;
            }
        }

        private void EnsureDefaultSetting()
        {
            if (SettingExists(_settingStore.DefaultSettingName)) return;

            SaveSettingCore(CreateDefaultSettingProfile());
        }

        private void EnsureProfile(DxfSettingProfile profile, string fallbackSettingName)
        {
            if (profile == null)
            {
                return;
            }

            EnsureProfileNames(profile, fallbackSettingName);
            EnsureStyleSettings(profile);
            EnsureLayerSettings(profile);
        }

        private void EnsureProfileNames(DxfSettingProfile profile, string fallbackSettingName)
        {
            if (string.IsNullOrWhiteSpace(profile.SettingName))
            {
                profile.SettingName = string.IsNullOrWhiteSpace(fallbackSettingName) ? _settingStore.DefaultSettingName : fallbackSettingName;
            }

            if (string.IsNullOrWhiteSpace(profile.TemplateName))
            {
                profile.TemplateName = _templateStore.DefaultTemplateName;
            }
        }

        private void EnsureStyleSettings(DxfSettingProfile profile)
        {
            if (profile.StyleSettings == null)
            {
                profile.StyleSettings = new List<DxfStyleSetting>();
            }

            var defaultStyles = CreateDefaultStyleSettings();

            foreach (var defaultStyle in defaultStyles)
            {
                EnsureStyleSetting(profile.StyleSettings, defaultStyle);
            }

            profile.StyleSettings = profile.StyleSettings
                .GroupBy(x => x.Role)
                .Select(x => x.First())
                .OrderBy(x => x.Role)
                .ToList();
        }

        private static void EnsureStyleSetting(List<DxfStyleSetting> styles, DxfStyleSetting defaultStyle)
        {
            var style = styles.FirstOrDefault(x => x.Role == defaultStyle.Role);

            if (style == null)
            {
                styles.Add(defaultStyle);
                return;
            }

            if (string.IsNullOrWhiteSpace(style.RoleName))
            {
                style.RoleName = defaultStyle.RoleName;
            }

            if (string.IsNullOrWhiteSpace(style.StyleName))
            {
                style.StyleName = defaultStyle.StyleName;
            }
        }

        private void EnsureLayerSettings(DxfSettingProfile profile)
        {
            if (profile.LayerSettings == null)
            {
                profile.LayerSettings = new List<DxfLayerSetting>();
            }

            var defaultLayers = CreateDefaultLayerSettings();

            foreach (var defaultLayer in defaultLayers)
            {
                EnsureLayerSetting(profile.LayerSettings, defaultLayer);
            }

            profile.LayerSettings = profile.LayerSettings
                .GroupBy(x => x.Role)
                .Select(x => x.First())
                .OrderBy(x => x.Role)
                .ToList();
        }

        private static void EnsureLayerSetting(List<DxfLayerSetting> layers, DxfLayerSetting defaultLayer)
        {
            var layer = layers.FirstOrDefault(x => x.Role == defaultLayer.Role);

            if (layer == null)
            {
                layers.Add(defaultLayer);
                return;
            }

            if (string.IsNullOrWhiteSpace(layer.RoleName))
            {
                layer.RoleName = defaultLayer.RoleName;
            }

            if (string.IsNullOrWhiteSpace(layer.LayerName))
            {
                layer.LayerName = defaultLayer.LayerName;
            }
        }

        private void ReassignDeletedTemplate(string deletedTemplateName)
        {
            foreach (var settingName in GetSettingNames())
            {
                var profile = LoadSetting(settingName);

                if (string.Equals(profile.TemplateName, deletedTemplateName, StringComparison.OrdinalIgnoreCase))
                {
                    profile.TemplateName = _templateStore.DefaultTemplateName;
                    SaveSetting(profile);
                }
            }
        }

    }
}
