using GirderSchedule.App.Utils;
using GirderSchedule.App.ViewModels.Home;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GirderSchedule.App.Views.Home
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
            DataContext = new HomeStartViewModel();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowSizeHelper.FitToWorkArea(this);
        }

        private void RecentProjectListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HomeStartViewModel viewModel = DataContext as HomeStartViewModel;
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

            HomeStartViewModel viewModel = DataContext as HomeStartViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.StartSelectedProject();
            e.Handled = true;
        }

        private void RecentProjectListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = FindParent<ScrollViewer>(sender as DependencyObject);

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

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = child == null ? null : VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                T target = parent as T;
                if (target != null)
                {
                    return target;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}