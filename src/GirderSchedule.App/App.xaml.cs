using GirderSchedule.App.Infrastructure;
using ScheduleTools.Wpf.Home.Views;
using System.Windows;

namespace GirderSchedule.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var viewModel = AppServices.CreateHomeStartViewModel();
            var homeWindow = new HomeWindow(viewModel);

            MainWindow = homeWindow;
            homeWindow.Show();
        }
    }
}