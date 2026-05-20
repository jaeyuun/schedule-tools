using System.Windows;
using GirderSchedule.App.ViewModels.Girder;

namespace GirderSchedule.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ScheduleViewModel();
        }
    }
}