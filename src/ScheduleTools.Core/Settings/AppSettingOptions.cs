namespace ScheduleTools.Core.Settings
{
    public sealed class AppSettingOptions
    {
        public string SettingFilePath { get; }
        public string ApplicationFolderName { get; }
        public string ProjectFolderName { get; }
        public double DefaultFontSize { get; }
        public double MinFontSize { get; }
        public double MaxFontSize { get; }

        public AppSettingOptions(
            string settingFilePath,
            string applicationFolderName,
            string projectFolderName,
            double defaultFontSize = 12,
            double minFontSize = 9,
            double maxFontSize = 16)
        {
            SettingFilePath = settingFilePath;
            ApplicationFolderName = applicationFolderName;
            ProjectFolderName = projectFolderName;
            DefaultFontSize = defaultFontSize;
            MinFontSize = minFontSize;
            MaxFontSize = maxFontSize;
        }
    }
}