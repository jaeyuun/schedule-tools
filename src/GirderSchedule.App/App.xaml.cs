using GirderSchedule.App.Services.Schedule;
using System.Windows;

namespace GirderSchedule.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            new DxfSettingService().EnsureDefaultStorage();
        }
    }
}
