using ScheduleTools.Wpf.Home.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScheduleTools.Wpf.Home.Views
{
    public partial class HomeWindow : Window
    {
        public HomeWindow(HomeStartViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            InitializeComponent();
            DataContext = viewModel;
        }

        private void RecentProjectListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                RecentProjectScrollViewer.LineUp();
            else
                RecentProjectScrollViewer.LineDown();

            e.Handled = true;
        }

        private void RecentProjectListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (sender is not ListBox { SelectedItem: RecentProjectViewModel recentProject })
                return;

            if (DataContext is not HomeStartViewModel viewModel)
                return;

            e.Handled = true;
            viewModel.OpenRecentProject(recentProject);
        }

        private void RecentProjectListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBox { SelectedItem: RecentProjectViewModel recentProject })
                return;

            if (DataContext is not HomeStartViewModel viewModel)
                return;

            e.Handled = true;
            viewModel.OpenRecentProject(recentProject);
        }
    }
}