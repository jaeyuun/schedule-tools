using GirderSchedule.App.ViewModels.Main;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.App.Views.Schedule.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace GirderSchedule.App.Views.Schedule
{
    public partial class ProjectExplorer : UserControl
    {
        private ProjectTreeDragDropBehavior _dragDropBehavior;

        public ProjectExplorer()
        {
            InitializeComponent();

            Loaded += ProjectExplorer_Loaded;
            Unloaded += ProjectExplorer_Unloaded;
        }

        private void ProjectExplorer_Loaded(object sender, RoutedEventArgs e)
        {
            if (_dragDropBehavior != null)
            {
                return;
            }

            _dragDropBehavior = new ProjectTreeDragDropBehavior(ProjectTreeView);
            _dragDropBehavior.Attach();
        }

        private void ProjectExplorer_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_dragDropBehavior == null)
            {
                return;
            }

            _dragDropBehavior.Detach();
            _dragDropBehavior = null;
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