using ScheduleTools.Core.Serialization;

namespace ScheduleTools.Core.Settings
{
    public sealed class AppSettingService
    {
        private readonly AppSettingOptions _options;

        public string ProjectPath => _options.ProjectPath;
        public double DefaultFontSize => _options.DefaultFontSize;
        public double MinFontSize => _options.MinFontSize;
        public double MaxFontSize => _options.MaxFontSize;

        public AppSettingService(AppSettingOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public AppSetting Load()
        {
            var setting = JsonFileSerializer.Load(
                _options.SettingFilePath,
                CreateDefault);

            Normalize(setting);
            return setting;
        }

        public void Save(AppSetting setting)
        {
            if (setting == null)
            {
                return;
            }

            Normalize(setting);
            JsonFileSerializer.Save(_options.SettingFilePath, setting);
        }

        public AppSetting CreateDefault()
        {
            return new AppSetting(
                _options.ProjectPath,
                _options.DefaultFontSize);
        }

        public double NormalizeFontSize(double fontSize)
        {
            if (double.IsNaN(fontSize) || double.IsInfinity(fontSize) || fontSize <= 0)
            {
                return _options.DefaultFontSize;
            }

            return Math.Clamp(
                fontSize,
                _options.MinFontSize,
                _options.MaxFontSize);
        }

        private void Normalize(AppSetting setting)
        {
            if (string.IsNullOrWhiteSpace(setting.DefaultProjectFolder))
            {
                setting.DefaultProjectFolder = _options.ProjectPath;
            }

            setting.DefaultFontSize = NormalizeFontSize(setting.DefaultFontSize);
        }
    }
}