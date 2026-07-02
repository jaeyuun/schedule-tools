namespace GirderSchedule.Domain.Models.Settings
{
    public sealed class DxfLayerSetting
    {
        public DxfLayerRole Role { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string LayerName { get; set; } = string.Empty;

        public DxfLayerSetting()
        {
        }

        public DxfLayerSetting(DxfLayerRole role, string displayName, string layerName)
        {
            Role = role;
            DisplayName = displayName;
            LayerName = layerName;
        }
    }
}