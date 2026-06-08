using System.Windows;
using System.Windows.Controls;
using GirderSchedule.App.ViewModels;

namespace GirderSchedule.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;

            if (item == null)
            {
                return;
            }

            var viewModel = DataContext as MainWindowViewModel;

            if (viewModel == null)
            {
                return;
            }

            var floor = item.DataContext as FloorNodeViewModel;
            if (floor != null)
            {
                viewModel.SelectedFloor = floor;
                e.Handled = true;
                return;
            }

            var set = item.DataContext as ScheduleSetNodeViewModel;
            if (set != null)
            {
                viewModel.SelectedSet = set;
                e.Handled = true;
            }
        }
    }
}