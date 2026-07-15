using GirderSchedule.App.Services.Settings;
using System;
using System.IO;

namespace GirderSchedule.App.Models
{
    public sealed class AppSetting
    {
        public string DefaultProjectFolder { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        public double DefaultFontSize { get; set; } = AppSettingService.DefaultFontSize;
    }
}