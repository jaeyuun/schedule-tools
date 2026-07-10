namespace GirderSchedule.Domain.Models.Settings
{
    public sealed class DxfLayerSetting
    {
        public DxfLayerRole Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string LayerName { get; set; } = string.Empty;

        public DxfLayerSetting()
        {
        }

        public DxfLayerSetting(DxfLayerRole role, string roleName, string layerName)
        {
            Role = role;
            RoleName = roleName;
            LayerName = layerName;
        }
    }
}