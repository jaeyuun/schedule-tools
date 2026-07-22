using GirderSchedule.App.Infrastructure;
using ScheduleTools.Wpf.Home.Views;
using ScheduleTools.Wpf.Settings;
using System.Windows;

namespace GirderSchedule.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ApplyAppSetting();

            var viewModel = AppServices.CreateHomeStartViewModel();
            var homeWindow = new HomeWindow(viewModel);

            MainWindow = homeWindow;
            homeWindow.Show();
        }

        private static void ApplyAppSetting()
        {
            var setting = AppServices.AppSetting.Load();
            AppFontSizeService.Apply(setting.DefaultFontSize, AppServices.AppSetting.NormalizeFontSize);
        }
    }
}