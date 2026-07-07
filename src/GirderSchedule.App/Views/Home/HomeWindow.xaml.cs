using GirderSchedule.App.Utils;
using GirderSchedule.App.ViewModels.Home;
using System.Windows;

namespace GirderSchedule.App.Views.Home
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
            DataContext = new HomeWindowViewModel();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowSizeHelper.FitToWorkArea(this);
        }
    }
}