using GirderSchedule.App.ViewModels.Home;
using System.Windows;
using System.Windows.Input;

namespace GirderSchedule.App.Views.Home
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
            DataContext = new HomeWindowViewModel();
        }

        private void RecentProjectListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = DataContext as HomeWindowViewModel;

            if (viewModel == null)
            {
                return;
            }

            viewModel.StartSelectedProject();
        }

        private void RecentProjectListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            var viewModel = DataContext as HomeWindowViewModel;

            if (viewModel == null)
            {
                return;
            }

            viewModel.StartSelectedProject();
            e.Handled = true;
        }
    }
}