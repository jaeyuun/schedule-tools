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
        }
    }
}