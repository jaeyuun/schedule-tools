using GirderSchedule.App.Constants;
using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Constants;
using GirderSchedule.Dxf.Documents;
using netDxf;
using ScheduleTools.Core.Serialization;
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
        private const string Header = "GIRDER_SCHEDULE_DXF_SETTING_V1";

        public void EnsureDefaultStorage()
        {
            Directory.CreateDirectory(FileConstants.SettingPath);
            Directory.CreateDirectory(FileConstants.TemplatePath);
            EnsureDefaultSetting();
        }

        public List<string> GetSettingNames()
        {
            EnsureDefaultStorage();

            return Directory.GetFiles(FileConstants.SettingPath, $"*{FileConstants.DxfSettingExtension}")
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .OrderBy(name => IsDefaultSettingName(name) ? 0 : 1)
                .ThenBy(name => name)
                .ToList();
        }

        public List<string> GetTemplateNames()
        {
            EnsureDefaultStorage();

            var names = Directory.GetFiles(FileConstants.TemplatePath, $"*{FileConstants.DxfExtension}")
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Where(name => !IsDefaultTemplateName(name))
                .OrderBy(name => name)
                .ToList();

            if (File.Exists(FileConstants.DefaultTemplateFilePath))
            {
                names.Insert(0, FileConstants.TemplateFileName);
            }

            return names;
        }

        public bool SettingExists(string settingName)
        {
            return File.Exists(GetSettingPath(settingName));
        }

        public string NormalizeSettingName(string settingName)
        {
            if (string.IsNullOrWhiteSpace(settingName) || !SettingExists(settingName))
            {
                return FileConstants.DxfSettingName;
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

            return ReadSettingFile(path, settingName);
        }

        public void SaveSetting(DxfSettingProfile profile)
        {
            Directory.CreateDirectory(FileConstants.SettingPath);
            Directory.CreateDirectory(FileConstants.TemplatePath);
            SaveSettingCore(profile);
        }

        public string AddTemplate(string sourceTemplatePath)
        {
            EnsureDefaultStorage();

            if (string.IsNullOrWhiteSpace(sourceTemplatePath) || !File.Exists(sourceTemplatePath))
            {
                return string.Empty;
            }

            var fileName = Path.GetFileName(sourceTemplatePath);
            var destinationPath = Path.Combine(FileConstants.TemplatePath, fileName);

            File.Copy(sourceTemplatePath, destinationPath, true);

            return fileName;
        }

        public bool DeleteTemplate(string templateName)
        {
            if (IsDefaultTemplateName(templateName))
            {
                return false;
            }

            var path = Path.Combine(FileConstants.TemplatePath, templateName);

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
            if (IsDefaultSettingName(settingName))
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

        public string GetTemplatePath(string templateName)
        {
            if (IsDefaultTemplateName(templateName))
            {
                return FileConstants.DefaultTemplateFilePath;
            }

            return Path.Combine(FileConstants.TemplatePath, templateName);
        }

        public DxfSettingProfile CreateDefaultSettingProfile()
        {
            return new DxfSettingProfile
            {
                SettingName = FileConstants.DxfSettingName,
                TemplateName = FileConstants.TemplateFileName,
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

        private DxfSettingProfile ReadSettingFile(string path, string fallbackSettingName)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fileStream, Encoding.UTF8, true))
            {
                var header = reader.ReadString();

                if (header != Header)
                {
                    return CreateDefaultSettingProfile();
                }

                var json = ReadCompressedJson(fileStream);
                var profile = JsonFileSerializer.Deserialize<DxfSettingProfile>(json);

                if (profile == null)
                {
                    return CreateDefaultSettingProfile();
                }

                EnsureProfile(profile, fallbackSettingName);

                return profile;
            }
        }

        private static string ReadCompressedJson(Stream stream)
        {
            using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
            using (var memoryStream = new MemoryStream())
            {
                gzipStream.CopyTo(memoryStream);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        private void SaveSettingCore(DxfSettingProfile profile)
        {
            if (profile == null)
            {
                profile = CreateDefaultSettingProfile();
            }

            EnsureProfile(profile, profile.SettingName);

            var path = GetSettingPath(profile.SettingName);
            var json = JsonFileSerializer.Serialize(profile);

            WriteSettingFile(path, json);
        }

        private static void WriteSettingFile(string path, string json)
        {
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fileStream, Encoding.UTF8))
            {
                writer.Write(Header);

                using (var gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal, true))
                {
                    gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
            }
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
            if (SettingExists(FileConstants.DxfSettingName))
            {
                return;
            }

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

        private static void EnsureProfileNames(DxfSettingProfile profile, string fallbackSettingName)
        {
            if (string.IsNullOrWhiteSpace(profile.SettingName))
            {
                profile.SettingName = string.IsNullOrWhiteSpace(fallbackSettingName) ? FileConstants.DxfSettingName : fallbackSettingName;
            }

            if (string.IsNullOrWhiteSpace(profile.TemplateName))
            {
                profile.TemplateName = FileConstants.TemplateFileName;
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
                    profile.TemplateName = FileConstants.TemplateFileName;
                    SaveSetting(profile);
                }
            }
        }

        private void RenameSettingProfile(string newName)
        {
            var profile = LoadSetting(newName);
            profile.SettingName = newName;
            SaveSetting(profile);
        }

        private string GetSettingPath(string settingName)
        {
            if (string.IsNullOrWhiteSpace(settingName))
            {
                settingName = FileConstants.DxfSettingName;
            }

            return Path.Combine(FileConstants.SettingPath, settingName + FileConstants.DxfSettingExtension);
        }

        private static bool IsDefaultSettingName(string settingName)
        {
            return string.Equals(settingName, FileConstants.DxfSettingName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDefaultTemplateName(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                return true;
            }

            return string.Equals(templateName, FileConstants.TemplateFileName, StringComparison.OrdinalIgnoreCase);
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