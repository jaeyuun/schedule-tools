using GirderSchedule.App.ViewModels.Main;
using GirderSchedule.App.ViewModels.Schedule;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GirderSchedule.App.Views.Schedule.Behaviors
{
    public sealed class ProjectTreeDragDropBehavior
    {
        private const string DropTopTag = "DropTop";
        private const string DropBottomTag = "DropBottom";

        private readonly TreeView _treeView;

        private Point _dragStartPoint;
        private object _dragItem;
        private TreeViewItem _insertLineItem;
        private bool _insertLineAfter;
        private FloorNodeViewModel _emptyAreaDropFloor;
        private bool _isAttached;

        public ProjectTreeDragDropBehavior(TreeView treeView)
        {
            _treeView = treeView;
        }

        public void Attach()
        {
            if (_isAttached || _treeView == null)
            {
                return;
            }

            _treeView.PreviewMouseLeftButtonDown += TreeView_PreviewMouseLeftButtonDown;
            _treeView.PreviewMouseRightButtonDown += TreeView_PreviewMouseRightButtonDown;
            _treeView.MouseMove += TreeView_MouseMove;
            _treeView.DragOver += TreeView_DragOver;
            _treeView.DragLeave += TreeView_DragLeave;
            _treeView.Drop += TreeView_Drop;

            _isAttached = true;
        }

        public void Detach()
        {
            if (!_isAttached || _treeView == null)
            {
                return;
            }

            ClearInsertLine();

            _treeView.PreviewMouseLeftButtonDown -= TreeView_PreviewMouseLeftButtonDown;
            _treeView.PreviewMouseRightButtonDown -= TreeView_PreviewMouseRightButtonDown;
            _treeView.MouseMove -= TreeView_MouseMove;
            _treeView.DragOver -= TreeView_DragOver;
            _treeView.DragLeave -= TreeView_DragLeave;
            _treeView.Drop -= TreeView_Drop;

            _dragItem = null;
            _isAttached = false;
        }

        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FocusTreeView();

            var source = e.OriginalSource as DependencyObject;

            if (IsDragIgnoredSource(source))
            {
                _dragItem = null;
                return;
            }

            _dragStartPoint = e.GetPosition(null);
            _dragItem = GetTreeViewItemData(source);
        }

        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            FocusTreeView();

            var item = FindVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);

            if (item == null)
            {
                return;
            }

            item.Focus();
            item.IsSelected = true;
        }

        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (!CanStartDrag(e))
            {
                return;
            }

            var data = new DataObject(_dragItem.GetType(), _dragItem);
            DragDrop.DoDragDrop(_treeView, data, DragDropEffects.Move);

            ClearInsertLine();
            _dragItem = null;
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            var context = CreateDropContext(e);

            if (HandleEmptyAreaDragOver(context, e))
            {
                return;
            }

            _emptyAreaDropFloor = null;

            if (!CanDropTreeItem(context.Source, context.Target))
            {
                RejectDrop(e);
                return;
            }

            var isAfter = GetIsAfter(context.TargetItem, e);
            ShowInsertLine(context.TargetItem, isAfter);

            AcceptDrop(e);
        }

        private void TreeView_DragLeave(object sender, DragEventArgs e)
        {
            if (IsPointerInsideTreeView(e))
            {
                return;
            }

            ClearInsertLine();
        }

        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            var context = CreateDropContext(e);
            var viewModel = _treeView.DataContext as MainWindowViewModel;
            var isAfter = _insertLineAfter;
            var emptyAreaDropFloor = _emptyAreaDropFloor;

            ClearInsertLine();

            if (viewModel == null)
            {
                return;
            }

            if (DropToEmptyArea(viewModel, context.Source, emptyAreaDropFloor))
            {
                e.Handled = true;
                return;
            }

            if (!CanDropTreeItem(context.Source, context.Target))
            {
                return;
            }

            if (DropToTreeItem(viewModel, context.Source, context.Target, isAfter))
            {
                e.Handled = true;
            }
        }

        private void FocusTreeView()
        {
            _treeView.Focus();
            Keyboard.Focus(_treeView);
        }

        private bool CanStartDrag(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _dragItem == null)
            {
                return false;
            }

            var currentPoint = e.GetPosition(null);
            var diff = _dragStartPoint - currentPoint;

            return Math.Abs(diff.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                   Math.Abs(diff.Y) >= SystemParameters.MinimumVerticalDragDistance;
        }

        private DropContext CreateDropContext(DragEventArgs e)
        {
            var source = GetDragData(e);
            var targetItem = FindVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);
            var target = targetItem == null ? null : targetItem.DataContext;

            return new DropContext(source, targetItem, target);
        }

        private bool HandleEmptyAreaDragOver(DropContext context, DragEventArgs e)
        {
            var sourceSet = context.Source as ScheduleSetNodeViewModel;

            if (context.TargetItem != null || sourceSet == null)
            {
                return false;
            }

            var lastClosedFloor = GetLastClosedFloor();

            if (lastClosedFloor == null)
            {
                return false;
            }

            _emptyAreaDropFloor = lastClosedFloor;
            ClearInsertLine();
            AcceptDrop(e);

            return true;
        }

        private bool DropToEmptyArea(MainWindowViewModel viewModel, object source, FloorNodeViewModel emptyAreaDropFloor)
        {
            var sourceSet = source as ScheduleSetNodeViewModel;

            if (sourceSet == null || emptyAreaDropFloor == null)
            {
                return false;
            }

            viewModel.MoveSetToFloor(sourceSet, emptyAreaDropFloor);
            return true;
        }

        private bool DropToTreeItem(MainWindowViewModel viewModel, object source, object target, bool isAfter)
        {
            var sourceFloor = source as FloorNodeViewModel;
            var targetFloor = target as FloorNodeViewModel;

            if (sourceFloor != null && targetFloor != null)
            {
                viewModel.MoveFloor(sourceFloor, targetFloor, isAfter);
                return true;
            }

            var sourceSet = source as ScheduleSetNodeViewModel;
            var targetSet = target as ScheduleSetNodeViewModel;

            if (sourceSet != null && targetSet != null)
            {
                viewModel.MoveSet(sourceSet, targetSet, isAfter);
                return true;
            }

            if (sourceSet != null && targetFloor != null)
            {
                viewModel.MoveSetToFloor(sourceSet, targetFloor);
                return true;
            }

            return false;
        }

        private void AcceptDrop(DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void RejectDrop(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            ClearInsertLine();
            e.Handled = true;
        }

        private bool IsPointerInsideTreeView(DragEventArgs e)
        {
            var position = e.GetPosition(_treeView);

            return position.X >= 0 &&
                   position.Y >= 0 &&
                   position.X <= _treeView.ActualWidth &&
                   position.Y <= _treeView.ActualHeight;
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
            item.Tag = isAfter ? DropBottomTag : DropTopTag;
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

            item.ClearValue(FrameworkElement.TagProperty);
        }

        private FloorNodeViewModel GetLastClosedFloor()
        {
            var viewModel = _treeView.DataContext as MainWindowViewModel;

            if (viewModel == null || viewModel.Floors == null || viewModel.Floors.Count == 0)
            {
                return null;
            }

            var lastFloor = viewModel.Floors[viewModel.Floors.Count - 1];
            var lastFloorItem = GetTreeViewItem(_treeView, lastFloor);

            if (lastFloorItem == null || lastFloorItem.IsExpanded)
            {
                return null;
            }

            return lastFloor;
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

        private sealed class DropContext
        {
            public object Source { get; private set; }
            public TreeViewItem TargetItem { get; private set; }
            public object Target { get; private set; }

            public DropContext(object source, TreeViewItem targetItem, object target)
            {
                Source = source;
                TargetItem = targetItem;
                Target = target;
            }
        }
    }
}