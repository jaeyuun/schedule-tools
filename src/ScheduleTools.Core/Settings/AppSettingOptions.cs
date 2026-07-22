namespace ScheduleTools.Core.Settings
{
    public sealed class AppSettingOptions
    {
        public string SettingFilePath { get; }
        public string ProjectPath { get; }
        public double DefaultFontSize { get; }
        public double MinFontSize { get; }
        public double MaxFontSize { get; }

        public AppSettingOptions(
            string settingFilePath,
            string applicationPath,
            double defaultFontSize = 12,
            double minFontSize = 9,
            double maxFontSize = 16)
        {
            SettingFilePath = settingFilePath;
            ProjectPath = applicationPath;
            DefaultFontSize = defaultFontSize;
            MinFontSize = minFontSize;
            MaxFontSize = maxFontSize;
        }
    }
}