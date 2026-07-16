using GirderSchedule.App.Services.Settings;
using GirderSchedule.App.Utils;

namespace GirderSchedule.App.Models
{
    public sealed class AppSetting
    {
        public string DefaultProjectFolder { get; set; } = PathUtil.GetDefaultProjectDirectory();
        public double DefaultFontSize { get; set; } = AppSettingService.DefaultFontSize;
    }
}