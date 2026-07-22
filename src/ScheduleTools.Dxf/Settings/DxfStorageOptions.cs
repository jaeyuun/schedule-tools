namespace ScheduleTools.Dxf.Settings
{
    public sealed class DxfStorageOptions
    {
        public string SettingsDirectory { get; }
        public string TemplatesDirectory { get; }
        public string SettingExtension { get; }
        public string SettingFileHeader { get; }
        public string DefaultSettingName { get; }
        public string DefaultTemplateName { get; }
        public string DefaultTemplatePath { get; }

        public DxfStorageOptions(
            string settingsDirectory,
            string templatesDirectory,
            string settingExtension,
            string settingFileHeader,
            string defaultSettingName,
            string defaultTemplateName,
            string defaultTemplatePath)
        {
            SettingsDirectory = Require(settingsDirectory, nameof(settingsDirectory));
            TemplatesDirectory = Require(templatesDirectory, nameof(templatesDirectory));
            SettingExtension = NormalizeExtension(settingExtension);
            SettingFileHeader = Require(settingFileHeader, nameof(settingFileHeader));
            DefaultSettingName = Require(defaultSettingName, nameof(defaultSettingName));
            DefaultTemplateName = Require(defaultTemplateName, nameof(defaultTemplateName));
            DefaultTemplatePath = Require(defaultTemplatePath, nameof(defaultTemplatePath));
        }

        private static string Require(string value, string parameterName)
        {
            return string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("필수 저장소 옵션이 비어 있습니다.", parameterName)
                : value;
        }

        private static string NormalizeExtension(string extension)
        {
            extension = Require(extension, nameof(extension));
            return extension.StartsWith('.') ? extension : "." + extension;
        }
    }
}
