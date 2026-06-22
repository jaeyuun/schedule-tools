using GirderSchedule.App.ViewModels.Main;
using GirderSchedule.App.ViewModels.Schedule;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GirderSchedule.App.Views.Schedule
{
    public partial class ProjectExplorer : UserControl
    {
        private Point _treeDragStartPoint;
        private object _treeDragItem;
        private TreeViewItem _insertLineItem;
        private bool _insertLineAfter;
        private FloorNodeViewModel _emptyAreaDropFloor;

        public ProjectExplorer()
        {
            InitializeComponent();
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

        private void ProjectTreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as DependencyObject;

            if (IsDragIgnoredSource(source))
            {
                _treeDragItem = null;
                return;
            }

            _treeDragStartPoint = e.GetPosition(null);
            _treeDragItem = GetTreeViewItemData(source);
        }

        private void ProjectTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _treeDragItem == null)
            {
                return;
            }

            var currentPoint = e.GetPosition(null);
            var diff = _treeDragStartPoint - currentPoint;

            if (Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance && Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            var data = new DataObject(_treeDragItem.GetType(), _treeDragItem);
            DragDrop.DoDragDrop(ProjectTreeView, data, DragDropEffects.Move);

            ClearInsertLine();
            _treeDragItem = null;
        }

        private void ProjectTreeView_DragOver(object sender, DragEventArgs e)
        {
            var source = GetDragData(e);
            var targetItem = FindVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);
            var target = targetItem == null ? null : targetItem.DataContext;
            var sourceSet = source as ScheduleSetNodeViewModel;

            if (targetItem == null && sourceSet != null)
            {
                var lastClosedFloor = GetLastClosedFloor();

                if (lastClosedFloor != null)
                {
                    _emptyAreaDropFloor = lastClosedFloor;
                    ClearInsertLine();

                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
                    return;
                }
            }

            _emptyAreaDropFloor = null;

            if (!CanDropTreeItem(source, target))
            {
                e.Effects = DragDropEffects.None;
                ClearInsertLine();
                e.Handled = true;
                return;
            }

            var isAfter = GetIsAfter(targetItem, e);
            ShowInsertLine(targetItem, isAfter);

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void ProjectTreeView_DragLeave(object sender, DragEventArgs e)
        {
            var position = e.GetPosition(ProjectTreeView);

            if (position.X >= 0 && position.Y >= 0 && position.X <= ProjectTreeView.ActualWidth && position.Y <= ProjectTreeView.ActualHeight)
            {
                return;
            }

            ClearInsertLine();
        }

        private void ProjectTreeView_Drop(object sender, DragEventArgs e)
        {
            var source = GetDragData(e);
            var targetItem = FindVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);
            var target = targetItem == null ? null : targetItem.DataContext;
            var isAfter = _insertLineAfter;
            var emptyAreaDropFloor = _emptyAreaDropFloor;

            ClearInsertLine();

            var viewModel = DataContext as MainWindowViewModel;

            if (viewModel == null)
            {
                return;
            }

            var sourceSet = source as ScheduleSetNodeViewModel;

            if (sourceSet != null && emptyAreaDropFloor != null)
            {
                viewModel.MoveSetToFloor(sourceSet, emptyAreaDropFloor);
                e.Handled = true;
                return;
            }

            if (!CanDropTreeItem(source, target))
            {
                return;
            }

            var sourceFloor = source as FloorNodeViewModel;
            var targetFloor = target as FloorNodeViewModel;

            if (sourceFloor != null && targetFloor != null)
            {
                viewModel.MoveFloor(sourceFloor, targetFloor, isAfter);
                e.Handled = true;
                return;
            }

            var targetSet = target as ScheduleSetNodeViewModel;

            if (sourceSet != null && targetSet != null)
            {
                viewModel.MoveSet(sourceSet, targetSet, isAfter);
                e.Handled = true;
                return;
            }

            if (sourceSet != null && targetFloor != null)
            {
                viewModel.MoveSetToFloor(sourceSet, targetFloor);
                e.Handled = true;
            }
        }

        private object GetDragData(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(FloorNodeViewModel)))
            {
                return e.Data.GetData(typeof(FloorNodeViewModel));
            }

            if (e.Data.GetDataPresent(typeof(ScheduleSetNodeViewModel)))
            {
                return e.Data.GetData(typeof(ScheduleSetNodeViewModel));
            }

            return null;
        }

        private bool CanDropTreeItem(object source, object target)
        {
            if (source == null || target == null || ReferenceEquals(source, target))
            {
                return false;
            }

            if (source is FloorNodeViewModel && target is FloorNodeViewModel)
            {
                return true;
            }

            if (source is ScheduleSetNodeViewModel && target is ScheduleSetNodeViewModel)
            {
                return true;
            }

            if (source is ScheduleSetNodeViewModel && target is FloorNodeViewModel)
            {
                return true;
            }

            return false;
        }

        private bool GetIsAfter(TreeViewItem item, DragEventArgs e)
        {
            if (item == null)
            {
                return true;
            }

            var point = e.GetPosition(item);
            return point.Y >= item.ActualHeight / 2.0;
        }

        private void ShowInsertLine(TreeViewItem item, bool isAfter)
        {
            if (_insertLineItem != null && _insertLineItem != item)
            {
                ClearTreeViewItemInsertLine(_insertLineItem);
            }

            if (item == null)
            {
                return;
            }

            _insertLineItem = item;
            _insertLineAfter = isAfter;

            item.BorderBrush = new SolidColorBrush(Color.FromRgb(38, 120, 255));

            if (isAfter)
            {
                item.BorderThickness = new Thickness(0, 0, 0, 2);
            }
            else
            {
                item.BorderThickness = new Thickness(0, 2, 0, 0);
            }
        }

        private void ClearInsertLine()
        {
            if (_insertLineItem != null)
            {
                ClearTreeViewItemInsertLine(_insertLineItem);
            }

            _insertLineItem = null;
            _insertLineAfter = false;
            _emptyAreaDropFloor = null;
        }

        private void ClearTreeViewItemInsertLine(TreeViewItem item)
        {
            if (item == null)
            {
                return;
            }

            item.ClearValue(BorderBrushProperty);
            item.ClearValue(BorderThicknessProperty);
        }

        private FloorNodeViewModel GetLastClosedFloor()
        {
            var viewModel = DataContext as MainWindowViewModel;

            if (viewModel == null || viewModel.Floors == null || viewModel.Floors.Count == 0)
            {
                return null;
            }

            var lastFloor = viewModel.Floors[viewModel.Floors.Count - 1];
            var lastFloorItem = GetFloorTreeViewItem(lastFloor);

            if (lastFloorItem == null || lastFloorItem.IsExpanded)
            {
                return null;
            }

            return lastFloor;
        }

        private TreeViewItem GetFloorTreeViewItem(FloorNodeViewModel floor)
        {
            return GetTreeViewItem(ProjectTreeView, floor);
        }

        private TreeViewItem GetTreeViewItem(ItemsControl parent, object data)
        {
            if (parent == null || data == null)
            {
                return null;
            }

            var item = parent.ItemContainerGenerator.ContainerFromItem(data) as TreeViewItem;

            if (item != null)
            {
                return item;
            }

            for (var i = 0; i < parent.Items.Count; i++)
            {
                var child = parent.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;

                if (child == null)
                {
                    continue;
                }

                item = GetTreeViewItem(child, data);

                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        private bool IsDragIgnoredSource(DependencyObject source)
        {
            if (source == null)
            {
                return true;
            }

            if (FindVisualParent<CheckBox>(source) != null)
            {
                return true;
            }

            if (FindVisualParent<ToggleButton>(source) != null)
            {
                return true;
            }

            return false;
        }

        private object GetTreeViewItemData(DependencyObject source)
        {
            var item = FindVisualParent<TreeViewItem>(source);
            return item == null ? null : item.DataContext;
        }

        private T FindVisualParent<T>(DependencyObject source) where T : DependencyObject
        {
            while (source != null)
            {
                var typed = source as T;

                if (typed != null)
                {
                    return typed;
                }

                source = VisualTreeHelper.GetParent(source);
            }

            return null;
        }
    }
}