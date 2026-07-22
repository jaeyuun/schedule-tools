namespace ScheduleTools.Core.Settings
{
    public sealed class AppSetting
    {
        public string DefaultProjectFolder { get; set; } = string.Empty;
        public double DefaultFontSize { get; set; }

        public AppSetting()
        {
        }

        public AppSetting(string defaultProjectFolder, double defaultFontSize)
        {
            DefaultProjectFolder = defaultProjectFolder;
            DefaultFontSize = defaultFontSize;
        }
    }
}