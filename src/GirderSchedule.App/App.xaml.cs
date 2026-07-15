using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.Services.Settings;
using System.Windows;

namespace GirderSchedule.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ApplyAppSetting();

            base.OnStartup(e);
            new DxfSettingService().EnsureDefaultStorage();
        }

        private void ApplyAppSetting()
        {
            var settingService = new AppSettingService();
            var setting = settingService.Load();

            AppSettingService.ApplyFontSize(setting.DefaultFontSize);
        }
    }
}
