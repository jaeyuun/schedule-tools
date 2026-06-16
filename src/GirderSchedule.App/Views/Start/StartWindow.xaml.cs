using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GirderSchedule.App.ViewModels.Start;

namespace GirderSchedule.App.Views.Start
{
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            DataContext = new StartWindowViewModel();
        }

        private void RecentProjectListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = DataContext as StartWindowViewModel;

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

            var viewModel = DataContext as StartWindowViewModel;

            if (viewModel == null)
            {
                return;
            }

            viewModel.StartSelectedProject();
            e.Handled = true;
        }
    }
}