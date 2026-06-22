using GirderSchedule.App.ViewModels.Home;
using System.Windows;

namespace GirderSchedule.App.Views.Home
{
    public partial class ProjectSettingWindow : Window
    {
        public ProjectSettingWindow()
        {
            InitializeComponent();
            DataContext = new ProjectSettingWindowViewModel();
        }
    }
}