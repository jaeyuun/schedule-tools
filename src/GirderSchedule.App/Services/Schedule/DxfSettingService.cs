using GirderSchedule.App.Common;
using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Constants;
using GirderSchedule.Dxf.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class DxfSettingService
    {
        private const string SettingHeader = "GIRDER_SCHEDULE_DXF_SETTING_V1";
        private const string DxfLayerFolderName = "DxfLayer";
        private const string SettingFolderName = "Setting";
        private const string TemplateFolderName = "Template";
        private const string SettingExtension = ".gds";
        private const string DefaultSettingName = "Default";
        private const string DefaultTemplateName = "GriderScheduleTemplate.dxf";

        public string DefaultSetting
        {
            get { return DefaultSettingName; }
        }

        public string GetRootDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DxfLayerFolderName);
        }

        public string GetSettingDirectory()
        {
            return Path.Combine(GetRootDirectory(), SettingFolderName);
        }

        public string GetTemplateDirectory()
        {
            return Path.Combine(GetRootDirectory(), TemplateFolderName);
        }

        public string GetDefaultTemplatePath()
        {
            return Path.Combine(GetTemplateDirectory(), DefaultTemplateName);
        }

        public void EnsureDefaultStorage()
        {
            Directory.CreateDirectory(GetSettingDirectory());
            Directory.CreateDirectory(GetTemplateDirectory());
            EnsureDefaultTemplate();
            EnsureDefaultSetting();
        }

        public List<string> GetSettingNames()
        {
            EnsureDefaultStorage();

            return Directory.GetFiles(GetSettingDirectory(), "*" + SettingExtension)
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .OrderBy(name => string.Equals(name, DefaultSettingName, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(name => name)
                .ToList();
        }

        public List<string> GetTemplateNames()
        {
            EnsureDefaultStorage();

            return Directory.GetFiles(GetTemplateDirectory(), "*.dxf")
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .OrderBy(name => string.Equals(name, DefaultTemplateName, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(name => name)
                .ToList();
        }

        public bool SettingExists(string settingName)
        {
            return File.Exists(GetSettingPath(settingName));
        }

        public string NormalizeSettingName(string settingName)
        {
            if (string.IsNullOrWhiteSpace(settingName) || !SettingExists(settingName))
            {
                return DefaultSettingName;
            }

            return settingName;
        }

        public DxfSettingProfile LoadSetting(string settingName)
        {
            EnsureDefaultStorage();

            settingName = NormalizeSettingName(settingName);
            var path = GetSettingPath(settingName);

            if (!File.Exists(path))
            {
                return CreateDefaultSettingProfile();
            }

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fileStream, Encoding.UTF8, true))
            {
                var header = reader.ReadString();

                if (header != SettingHeader)
                {
                    return CreateDefaultSettingProfile();
                }

                using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                using (var memoryStream = new MemoryStream())
                {
                    gzipStream.CopyTo(memoryStream);

                    var json = Encoding.UTF8.GetString(memoryStream.ToArray());
                    var profile = AppJsonSerializer.Deserialize<DxfSettingProfile>(json);

                    if (profile == null)
                    {
                        return CreateDefaultSettingProfile();
                    }

                    EnsureProfile(profile, settingName);
                    return profile;
                }
            }
        }

        public string AddTemplate(string sourceTemplatePath)
        {
            EnsureDefaultStorage();

            if (string.IsNullOrWhiteSpace(sourceTemplatePath) || !File.Exists(sourceTemplatePath))
            {
                return string.Empty;
            }

            var fileName = Path.GetFileName(sourceTemplatePath);
            var destinationPath = Path.Combine(GetTemplateDirectory(), fileName);
            File.Copy(sourceTemplatePath, destinationPath, true);
            return fileName;
        }

        public bool DeleteTemplate(string templateName)
        {
            if (string.Equals(templateName, DefaultTemplateName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var path = GetTemplatePath(templateName);

            if (!File.Exists(path))
            {
                return false;
            }

            File.Delete(path);
            ReassignDeletedTemplate(templateName);
            return true;
        }

        public bool DeleteSetting(string settingName)
        {
            if (string.Equals(settingName, DefaultSettingName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var path = GetSettingPath(settingName);

            if (!File.Exists(path))
            {
                return false;
            }

            File.Delete(path);
            return true;
        }

        public bool RenameSetting(string oldName, string newName)
        {
            if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName))
            {
                return false;
            }

            if (string.Equals(oldName, DefaultSettingName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var oldPath = GetSettingPath(oldName);
            var newPath = GetSettingPath(newName);

            if (!File.Exists(oldPath) || File.Exists(newPath))
            {
                return false;
            }

            File.Move(oldPath, newPath);

            var profile = LoadSetting(newName);
            profile.SettingName = newName;
            SaveSetting(profile);
            return true;
        }

        public string GetTemplatePath(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                templateName = DefaultTemplateName;
            }

            return Path.Combine(GetTemplateDirectory(), templateName);
        }

        public DxfStyleSetting CreateDefaultStyleSetting()
        {
            return new DxfStyleSetting
            {
                TextStyleName = DxfStyles.DefaultText,
                DrawingBlockName = DxfBlocks.TitleBlock,
                DimensionStyleName = DxfStyles.Dimension
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
                new DxfLayerSetting(DxfLayerRole.Text, "텍스트", DxfLayers.Text)
            };
        }

        public DxfSettingValidationResult Validate(DxfSettingProfile profile)
        {
            var result = new DxfSettingValidationResult();

            if (profile == null)
            {
                result.TemplateError = "설정이 비어 있습니다.";
                return result;
            }

            var templatePath = GetTemplatePath(profile.TemplateName);

            if (string.IsNullOrWhiteSpace(profile.TemplateName) || !File.Exists(templatePath))
            {
                result.TemplateError = "템플릿 파일을 찾을 수 없습니다.";
                return result;
            }

            try
            {
                var loader = new DxfDocumentLoader();
                var template = loader.LoadTemplate(templatePath);
                var document = template.Document;

                if (profile.StyleSetting == null)
                {
                    result.TemplateError = "스타일 설정이 비어 있습니다.";
                    return result;
                }

                if (!document.TextStyles.Contains(profile.StyleSetting.TextStyleName))
                {
                    result.InvalidStyles.Add("글꼴");
                }

                if (!document.Blocks.Contains(profile.StyleSetting.DrawingBlockName))
                {
                    result.InvalidStyles.Add("도면");
                }

                if (!document.DimensionStyles.Contains(profile.StyleSetting.DimensionStyleName))
                {
                    result.InvalidStyles.Add("치수선");
                }

                if (profile.LayerSettings == null)
                {
                    result.TemplateError = "레이어 설정이 비어 있습니다.";
                    return result;
                }

                foreach (var layer in profile.LayerSettings)
                {
                    if (string.IsNullOrWhiteSpace(layer.LayerName) || !document.Layers.Contains(layer.LayerName))
                    {
                        result.InvalidLayers.Add(layer.DisplayName);
                    }
                }
            }
            catch
            {
                result.TemplateError = "템플릿 파일을 읽을 수 없습니다.";
            }

            return result;
        }

        public void SaveSetting(DxfSettingProfile profile)
        {
            Directory.CreateDirectory(GetSettingDirectory());
            Directory.CreateDirectory(GetTemplateDirectory());
            SaveSettingCore(profile);
        }

        private void EnsureDefaultTemplate()
        {
            var defaultTemplatePath = GetDefaultTemplatePath();

            if (File.Exists(defaultTemplatePath))
            {
                return;
            }

            var resourceTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", DefaultTemplateName);
            var legacyTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "GirderTemplate.dxf");

            if (File.Exists(resourceTemplatePath))
            {
                File.Copy(resourceTemplatePath, defaultTemplatePath, true);
                return;
            }

            if (File.Exists(legacyTemplatePath))
            {
                File.Copy(legacyTemplatePath, defaultTemplatePath, true);
            }
        }

        private void EnsureDefaultSetting()
        {
            if (SettingExists(DefaultSettingName))
            {
                return;
            }

            SaveSettingCore(CreateDefaultSettingProfile());
        }

        private void SaveSettingCore(DxfSettingProfile profile)
        {
            if (profile == null)
            {
                profile = CreateDefaultSettingProfile();
            }

            EnsureProfile(profile, profile.SettingName);

            var path = GetSettingPath(profile.SettingName);
            var json = AppJsonSerializer.Serialize(profile);
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fileStream, Encoding.UTF8))
            {
                writer.Write(SettingHeader);

                using (var gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal, true))
                {
                    gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
            }
        }

        private DxfSettingProfile CreateDefaultSettingProfile()
        {
            return new DxfSettingProfile
            {
                SettingName = DefaultSettingName,
                TemplateName = DefaultTemplateName,
                StyleSetting = CreateDefaultStyleSetting(),
                LayerSettings = CreateDefaultLayerSettings()
            };
        }

        private void EnsureProfile(DxfSettingProfile profile, string fallbackSettingName)
        {
            if (profile == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(profile.SettingName))
            {
                profile.SettingName = string.IsNullOrWhiteSpace(fallbackSettingName) ? DefaultSettingName : fallbackSettingName;
            }

            if (string.IsNullOrWhiteSpace(profile.TemplateName))
            {
                profile.TemplateName = DefaultTemplateName;
            }

            if (profile.StyleSetting == null)
            {
                profile.StyleSetting = CreateDefaultStyleSetting();
            }

            EnsureStyleSetting(profile.StyleSetting);

            if (profile.LayerSettings == null)
            {
                profile.LayerSettings = new List<DxfLayerSetting>();
            }

            EnsureLayerSettings(profile);
        }

        private void EnsureStyleSetting(DxfStyleSetting styleSetting)
        {
            var defaultStyle = CreateDefaultStyleSetting();

            if (string.IsNullOrWhiteSpace(styleSetting.TextStyleName))
            {
                styleSetting.TextStyleName = defaultStyle.TextStyleName;
            }

            if (string.IsNullOrWhiteSpace(styleSetting.DrawingBlockName))
            {
                styleSetting.DrawingBlockName = defaultStyle.DrawingBlockName;
            }

            if (string.IsNullOrWhiteSpace(styleSetting.DimensionStyleName))
            {
                styleSetting.DimensionStyleName = defaultStyle.DimensionStyleName;
            }
        }

        private void EnsureLayerSettings(DxfSettingProfile profile)
        {
            var defaultLayers = CreateDefaultLayerSettings();

            foreach (var defaultLayer in defaultLayers)
            {
                var layer = profile.LayerSettings.FirstOrDefault(x => x.Role == defaultLayer.Role);

                if (layer == null)
                {
                    profile.LayerSettings.Add(defaultLayer);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(layer.DisplayName))
                {
                    layer.DisplayName = defaultLayer.DisplayName;
                }

                if (string.IsNullOrWhiteSpace(layer.LayerName))
                {
                    layer.LayerName = defaultLayer.LayerName;
                }
            }

            profile.LayerSettings = profile.LayerSettings
                .GroupBy(x => x.Role)
                .Select(x => x.First())
                .OrderBy(x => x.Role)
                .ToList();
        }

        private void ReassignDeletedTemplate(string deletedTemplateName)
        {
            foreach (var settingName in GetSettingNames())
            {
                var profile = LoadSetting(settingName);

                if (string.Equals(profile.TemplateName, deletedTemplateName, StringComparison.OrdinalIgnoreCase))
                {
                    profile.TemplateName = DefaultTemplateName;
                    SaveSetting(profile);
                }
            }
        }

        private string GetSettingPath(string settingName)
        {
            if (string.IsNullOrWhiteSpace(settingName))
            {
                settingName = DefaultSettingName;
            }

            return Path.Combine(GetSettingDirectory(), settingName + SettingExtension);
        }
    }

    public sealed class DxfSettingValidationResult
    {
        public string TemplateError { get; set; } = string.Empty;
        public List<string> InvalidStyles { get; private set; }
        public List<string> InvalidLayers { get; private set; }

        public bool IsValid
        {
            get { return string.IsNullOrWhiteSpace(TemplateError) && InvalidStyles.Count == 0 && InvalidLayers.Count == 0; }
        }

        public DxfSettingValidationResult()
        {
            InvalidStyles = new List<string>();
            InvalidLayers = new List<string>();
        }
    }
}