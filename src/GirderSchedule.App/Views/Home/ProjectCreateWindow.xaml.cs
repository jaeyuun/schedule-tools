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
        }
    }
}