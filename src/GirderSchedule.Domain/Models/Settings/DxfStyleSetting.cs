namespace GirderSchedule.Domain.Models.Settings
{
    public sealed class DxfStyleSetting
    {
        public DxfStyleRole Role { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string StyleName { get; set; } = string.Empty;

        public DxfStyleSetting()
        {
        }

        public DxfStyleSetting(DxfStyleRole role, string displayName, string styleName)
        {
            Role = role;
            DisplayName = displayName;
            StyleName = styleName;
        }
    }
}