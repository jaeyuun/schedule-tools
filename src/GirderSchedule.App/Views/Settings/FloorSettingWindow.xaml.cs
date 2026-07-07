using GirderSchedule.App.Utils;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.App.ViewModels.Settings;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GirderSchedule.App.Views.Settings
{
    public partial class FloorSettingWindow : Window
    {
        private Point _dragStartPoint;
        private FloorNodeViewModel _draggedFloor;
        private DataGridRow _insertLineRow;

        public FloorSettingWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowSizeHelper.FitToWorkArea(this);
        }

        private void FloorGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            _draggedFloor = GetRowViewModel(e.OriginalSource as DependencyObject);
        }

        private void FloorGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (_draggedFloor == null)
            {
                return;
            }

            var currentPoint = e.GetPosition(null);
            var diff = _dragStartPoint - currentPoint;

            if (Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            DragDrop.DoDragDrop(FloorGrid, _draggedFloor, DragDropEffects.Move);

            ClearInsertLine();
            _draggedFloor = null;
        }

        private void FloorGrid_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(FloorNodeViewModel)))
            {
                e.Effects = DragDropEffects.None;
                ClearInsertLine();
                e.Handled = true;
                return;
            }

            var row = FindParent<DataGridRow>(e.OriginalSource as DependencyObject);

            if (row == null)
            {
                row = GetLastRow();
                ShowInsertLine(row, true);

                e.Effects = DragDropEffects.Move;
                e.Handled = true;
                return;
            }

            var rowPoint = e.GetPosition(row);
            var isAfter = rowPoint.Y >= row.ActualHeight / 2.0;

            ShowInsertLine(row, isAfter);

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void FloorGrid_DragLeave(object sender, DragEventArgs e)
        {
            var position = e.GetPosition(FloorGrid);

            if (position.X >= 0 &&
                position.Y >= 0 &&
                position.X <= FloorGrid.ActualWidth &&
                position.Y <= FloorGrid.ActualHeight)
            {
                return;
            }

            ClearInsertLine();
        }

        private void FloorGrid_Drop(object sender, DragEventArgs e)
        {
            ClearInsertLine();

            var source = e.Data.GetData(typeof(FloorNodeViewModel)) as FloorNodeViewModel;

            if (source == null)
            {
                return;
            }

            var viewModel = DataContext as FloorSettingWindowViewModel;

            if (viewModel == null)
            {
                return;
            }

            var oldIndex = viewModel.Floors.IndexOf(source);
            var insertIndex = GetInsertIndex(e.OriginalSource as DependencyObject, viewModel);

            if (oldIndex < 0)
            {
                return;
            }

            if (insertIndex > oldIndex)
            {
                insertIndex--;
            }

            if (insertIndex < 0)
            {
                insertIndex = 0;
            }

            if (insertIndex >= viewModel.Floors.Count)
            {
                insertIndex = viewModel.Floors.Count - 1;
            }

            if (oldIndex == insertIndex)
            {
                viewModel.SelectedFloor = source;
                return;
            }

            viewModel.Floors.Move(oldIndex, insertIndex);
            viewModel.SelectedFloor = source;
        }

        private int GetInsertIndex(DependencyObject source, FloorSettingWindowViewModel viewModel)
        {
            var row = FindParent<DataGridRow>(source);

            if (row == null)
            {
                return viewModel.Floors.Count;
            }

            var target = row.Item as FloorNodeViewModel;

            if (target == null)
            {
                return viewModel.Floors.Count;
            }

            var targetIndex = viewModel.Floors.IndexOf(target);

            if (targetIndex < 0)
            {
                return viewModel.Floors.Count;
            }

            var rowPoint = Mouse.GetPosition(row);
            var isUpperHalf = rowPoint.Y < row.ActualHeight / 2.0;

            if (isUpperHalf)
            {
                return targetIndex;
            }

            return targetIndex + 1;
        }

        private void ShowInsertLine(DataGridRow row, bool isAfter)
        {
            if (_insertLineRow != null && _insertLineRow != row)
            {
                ClearRowInsertLine(_insertLineRow);
            }

            if (row == null)
            {
                return;
            }

            _insertLineRow = row;
            row.BorderBrush = new SolidColorBrush(Color.FromRgb(38, 120, 255));

            if (isAfter)
            {
                row.BorderThickness = new Thickness(0, 0, 0, 2);
                return;
            }

            row.BorderThickness = new Thickness(0, 2, 0, 0);
        }

        private void ClearInsertLine()
        {
            if (_insertLineRow == null)
            {
                return;
            }

            ClearRowInsertLine(_insertLineRow);
            _insertLineRow = null;
        }

        private void ClearRowInsertLine(DataGridRow row)
        {
            if (row == null)
            {
                return;
            }

            row.ClearValue(BorderBrushProperty);
            row.ClearValue(BorderThicknessProperty);
        }

        private DataGridRow GetLastRow()
        {
            if (FloorGrid.Items.Count == 0)
            {
                return null;
            }

            var index = FloorGrid.Items.Count - 1;
            return FloorGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }

        private FloorNodeViewModel GetRowViewModel(DependencyObject source)
        {
            var row = FindParent<DataGridRow>(source);

            if (row == null)
            {
                return null;
            }

            return row.Item as FloorNodeViewModel;
        }

        private T FindParent<T>(DependencyObject source) where T : DependencyObject
        {
            while (source != null)
            {
                var parent = VisualTreeHelper.GetParent(source);

                if (parent is T)
                {
                    return parent as T;
                }

                source = parent;
            }

            return null;
        }
    }
}