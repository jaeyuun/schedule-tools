using System.Collections.Generic;

namespace GirderSchedule.Domain.Models.Settings
{
    public sealed class DxfSettingProfile
    {
        public string SettingName { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public DxfStyleSetting StyleSetting { get; set; } = new DxfStyleSetting();
        public List<DxfLayerSetting> LayerSettings { get; set; } = new List<DxfLayerSetting>();
    }
}
