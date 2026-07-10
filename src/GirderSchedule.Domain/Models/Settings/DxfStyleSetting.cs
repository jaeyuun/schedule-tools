namespace GirderSchedule.Domain.Models.Settings
{
    public sealed class DxfStyleSetting
    {
        public DxfStyleRole Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string StyleName { get; set; } = string.Empty;

        public DxfStyleSetting()
        {
        }

        public DxfStyleSetting(DxfStyleRole role, string roleName, string styleName)
        {
            Role = role;
            RoleName = roleName;
            StyleName = styleName;
        }
    }
}