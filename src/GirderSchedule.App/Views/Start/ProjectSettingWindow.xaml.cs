using System.Windows;
using GirderSchedule.App.ViewModels.Start;

namespace GirderSchedule.App.Views.Start
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