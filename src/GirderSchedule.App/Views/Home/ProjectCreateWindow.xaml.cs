using GirderSchedule.App.Utils;
using GirderSchedule.App.ViewModels.Home;
using System.Windows;

namespace GirderSchedule.App.Views.Home
{
    public partial class ProjectCreateWindow : Window
    {
        public ProjectCreateWindow()
        {
            InitializeComponent();
            DataContext = new ProjectCreateWindowViewModel();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowSizeHelper.FitToWorkArea(this);
        }
    }
}