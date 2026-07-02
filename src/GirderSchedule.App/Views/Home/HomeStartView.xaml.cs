using GirderSchedule.App.ViewModels.Home;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GirderSchedule.App.Views.Home
{
    public partial class HomeStartView : UserControl
    {
        public HomeStartView()
        {
            InitializeComponent();
        }

        private void RecentProjectListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not HomeStartViewModel viewModel)
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

            if (DataContext is not HomeStartViewModel viewModel)
            {
                return;
            }

            viewModel.StartSelectedProject();
            e.Handled = true;
        }

        private void RecentProjectListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = FindParent<ScrollViewer>(sender as DependencyObject);

            if (scrollViewer == null)
            {
                return;
            }

            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Clear();
            SearchTextBox.Focus();
        }

        private static T? FindParent<T>(DependencyObject? child) where T : DependencyObject
        {
            var parent = child == null ? null : VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                if (parent is T target)
                {
                    return target;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}