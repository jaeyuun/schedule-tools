using GirderSchedule.App.Preview;
using GirderSchedule.App.ViewModels;
using GirderSchedule.Domain.Girder.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GirderSchedule.App.Views.Controls
{
    public partial class ScheduleExportPreview : UserControl
    {
        public static readonly DependencyProperty FloorsProperty = DependencyProperty.Register(nameof(Floors), typeof(ObservableCollection<FloorNodeViewModel>), typeof(ScheduleExportPreview), new PropertyMetadata(null, OnFloorsChanged));

        private const int PageSize = 9;

        private readonly ScheduleExportPreviewDrawer _drawer = new ScheduleExportPreviewDrawer();
        private readonly List<ScheduleSet> _allSets = new List<ScheduleSet>();

        private bool _isFitMode = true;
        private bool _isRefreshQueued;
        private bool _isPanning;
        private double _zoomScale = 1.0;
        private int _currentPageIndex;
        private Point _panStartPoint;
        private Point _panStartOffset;

        public ObservableCollection<FloorNodeViewModel> Floors
        {
            get { return (ObservableCollection<FloorNodeViewModel>)GetValue(FloorsProperty); }
            set { SetValue(FloorsProperty, value); }
        }

        public ScheduleExportPreview()
        {
            InitializeComponent();

            Loaded += ScheduleExportPreview_Loaded;
            Unloaded += ScheduleExportPreview_Unloaded;
            SizeChanged += ScheduleExportPreview_SizeChanged;
        }

        private static void OnFloorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ScheduleExportPreview;

            if (control == null)
            {
                return;
            }

            var oldFloors = e.OldValue as ObservableCollection<FloorNodeViewModel>;
            var newFloors = e.NewValue as ObservableCollection<FloorNodeViewModel>;

            control.DetachFloors(oldFloors);
            control.AttachFloors(newFloors);
            control.QueueRefresh(true);
        }

        private void ScheduleExportPreview_Loaded(object sender, RoutedEventArgs e)
        {
            AttachFloors(Floors);
            QueueRefresh(true);
        }

        private void ScheduleExportPreview_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachFloors(Floors);
            EndPan();
        }

        private void ScheduleExportPreview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isFitMode)
            {
                return;
            }

            Dispatcher.BeginInvoke(new Action(FitToScreen));
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex <= 0)
            {
                return;
            }

            _currentPageIndex--;
            Redraw();

            if (_isFitMode)
            {
                FitToScreen();
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex >= GetPageCount() - 1)
            {
                return;
            }

            _currentPageIndex++;
            Redraw();

            if (_isFitMode)
            {
                FitToScreen();
            }
        }

        private void Fit_Click(object sender, RoutedEventArgs e)
        {
            FitToScreen();
        }

        private void PreviewScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (PreviewScrollViewer == null)
            {
                return;
            }

            var mousePosition = e.GetPosition(PreviewScrollViewer);
            var scale = e.Delta > 0 ? _zoomScale * 1.1 : _zoomScale / 1.1;

            SetZoomAtPoint(scale, mousePosition);

            e.Handled = true;
        }

        private void PreviewScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PreviewScrollViewer == null || e.ChangedButton != MouseButton.Middle)
            {
                return;
            }

            _isPanning = true;
            _panStartPoint = e.GetPosition(PreviewScrollViewer);
            _panStartOffset = new Point(PreviewScrollViewer.HorizontalOffset, PreviewScrollViewer.VerticalOffset);

            PreviewScrollViewer.CaptureMouse();
            PreviewScrollViewer.Cursor = Cursors.ScrollAll;

            e.Handled = true;
        }

        private void PreviewScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (PreviewScrollViewer == null || !_isPanning)
            {
                return;
            }

            var currentPoint = e.GetPosition(PreviewScrollViewer);
            var deltaX = currentPoint.X - _panStartPoint.X;
            var deltaY = currentPoint.Y - _panStartPoint.Y;

            PreviewScrollViewer.ScrollToHorizontalOffset(_panStartOffset.X - deltaX);
            PreviewScrollViewer.ScrollToVerticalOffset(_panStartOffset.Y - deltaY);

            e.Handled = true;
        }

        private void PreviewScrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle)
            {
                return;
            }

            EndPan();
            e.Handled = true;
        }

        private void PreviewScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isPanning)
            {
                return;
            }

            EndPan();
        }

        private void EndPan()
        {
            _isPanning = false;

            if (PreviewScrollViewer == null)
            {
                return;
            }

            PreviewScrollViewer.ReleaseMouseCapture();
            PreviewScrollViewer.Cursor = Cursors.Arrow;
        }

        private void QueueRefresh(bool fitAfterRefresh)
        {
            if (_isRefreshQueued)
            {
                return;
            }

            _isRefreshQueued = true;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                _isRefreshQueued = false;
                RebuildPages();
                Redraw();

                if (fitAfterRefresh)
                {
                    FitToScreen();
                }
            }));
        }

        private void RebuildPages()
        {
            _allSets.Clear();

            if (Floors != null)
            {
                for (var i = 0; i < Floors.Count; i++)
                {
                    var floor = Floors[i];

                    if (floor == null || !floor.IsChecked)
                    {
                        continue;
                    }

                    for (var j = 0; j < floor.Sets.Count; j++)
                    {
                        var setNode = floor.Sets[j];

                        if (setNode == null || !setNode.IsChecked || setNode.Model == null)
                        {
                            continue;
                        }

                        _allSets.Add(setNode.Model);
                    }
                }
            }

            var pageCount = GetPageCount();

            if (_currentPageIndex >= pageCount)
            {
                _currentPageIndex = pageCount - 1;
            }

            if (_currentPageIndex < 0)
            {
                _currentPageIndex = 0;
            }

            UpdatePageState();
        }

        private void Redraw()
        {
            if (PreviewCanvas == null)
            {
                return;
            }

            PreviewCanvas.Children.Clear();

            var pageSets = GetCurrentPageSets();
            _drawer.Draw(PreviewCanvas, pageSets, _currentPageIndex + 1, GetPageCount());

            UpdatePageState();
        }

        private List<ScheduleSet> GetCurrentPageSets()
        {
            var result = new List<ScheduleSet>();

            if (_allSets.Count == 0)
            {
                return result;
            }

            var start = _currentPageIndex * PageSize;
            var end = Math.Min(start + PageSize, _allSets.Count);

            for (var i = start; i < end; i++)
            {
                result.Add(_allSets[i]);
            }

            return result;
        }

        private int GetPageCount()
        {
            if (_allSets.Count == 0)
            {
                return 1;
            }

            return (_allSets.Count + PageSize - 1) / PageSize;
        }

        private void UpdatePageState()
        {
            var pageCount = GetPageCount();

            if (PageTextBlock != null)
            {
                PageTextBlock.Text = (_currentPageIndex + 1) + " / " + pageCount;
            }

            if (PrevPageButton != null)
            {
                PrevPageButton.IsEnabled = _currentPageIndex > 0;
            }

            if (NextPageButton != null)
            {
                NextPageButton.IsEnabled = _currentPageIndex < pageCount - 1;
            }
        }

        private void FitToScreen()
        {
            if (PreviewScrollViewer == null || PreviewCanvas == null)
            {
                return;
            }

            var viewportWidth = PreviewScrollViewer.ActualWidth;
            var viewportHeight = PreviewScrollViewer.ActualHeight;

            if (viewportWidth <= 0 || viewportHeight <= 0)
            {
                return;
            }

            var canvasWidth = PreviewCanvas.Width;
            var canvasHeight = PreviewCanvas.Height;

            if (canvasWidth <= 0 || canvasHeight <= 0)
            {
                return;
            }

            var scaleX = viewportWidth / canvasWidth;
            var scaleY = viewportHeight / canvasHeight;
            var scale = Math.Min(scaleX, scaleY);

            if (scale > 1.0)
            {
                scale = 1.0;
            }

            SetZoom(scale, true);

            PreviewScrollViewer.ScrollToHorizontalOffset(0);
            PreviewScrollViewer.ScrollToVerticalOffset(0);
        }

        private void SetZoom(double scale, bool fitMode)
        {
            if (scale < 0.2)
            {
                scale = 0.2;
            }

            if (scale > 4.0)
            {
                scale = 4.0;
            }

            _isFitMode = fitMode;
            _zoomScale = scale;

            PreviewScaleTransform.ScaleX = scale;
            PreviewScaleTransform.ScaleY = scale;

            if (ZoomTextBlock != null)
            {
                ZoomTextBlock.Text = ((int)Math.Round(scale * 100.0)) + "%";
            }
        }

        private void SetZoomAtPoint(double scale, Point mousePosition)
        {
            if (PreviewScrollViewer == null)
            {
                SetZoom(scale, false);
                return;
            }

            var oldScale = _zoomScale;

            if (oldScale <= 0)
            {
                oldScale = 1.0;
            }

            if (scale < 0.2)
            {
                scale = 0.2;
            }

            if (scale > 4.0)
            {
                scale = 4.0;
            }

            var contentX = (PreviewScrollViewer.HorizontalOffset + mousePosition.X) / oldScale;
            var contentY = (PreviewScrollViewer.VerticalOffset + mousePosition.Y) / oldScale;

            SetZoom(scale, false);

            PreviewScrollViewer.ScrollToHorizontalOffset(contentX * scale - mousePosition.X);
            PreviewScrollViewer.ScrollToVerticalOffset(contentY * scale - mousePosition.Y);
        }

        private void AttachFloors(ObservableCollection<FloorNodeViewModel> floors)
        {
            if (floors == null)
            {
                return;
            }

            floors.CollectionChanged += Floors_CollectionChanged;

            for (var i = 0; i < floors.Count; i++)
            {
                AttachFloor(floors[i]);
            }
        }

        private void DetachFloors(ObservableCollection<FloorNodeViewModel> floors)
        {
            if (floors == null)
            {
                return;
            }

            floors.CollectionChanged -= Floors_CollectionChanged;

            for (var i = 0; i < floors.Count; i++)
            {
                DetachFloor(floors[i]);
            }
        }

        private void AttachFloor(FloorNodeViewModel floor)
        {
            if (floor == null)
            {
                return;
            }

            floor.PropertyChanged += Floor_PropertyChanged;

            if (floor.Sets != null)
            {
                floor.Sets.CollectionChanged += Sets_CollectionChanged;

                for (var i = 0; i < floor.Sets.Count; i++)
                {
                    AttachSet(floor.Sets[i]);
                }
            }
        }

        private void DetachFloor(FloorNodeViewModel floor)
        {
            if (floor == null)
            {
                return;
            }

            floor.PropertyChanged -= Floor_PropertyChanged;

            if (floor.Sets != null)
            {
                floor.Sets.CollectionChanged -= Sets_CollectionChanged;

                for (var i = 0; i < floor.Sets.Count; i++)
                {
                    DetachSet(floor.Sets[i]);
                }
            }
        }

        private void AttachSet(ScheduleSetNodeViewModel set)
        {
            if (set == null)
            {
                return;
            }

            set.PropertyChanged += Set_PropertyChanged;
        }

        private void DetachSet(ScheduleSetNodeViewModel set)
        {
            if (set == null)
            {
                return;
            }

            set.PropertyChanged -= Set_PropertyChanged;
        }

        private void Floors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                for (var i = 0; i < e.OldItems.Count; i++)
                {
                    DetachFloor(e.OldItems[i] as FloorNodeViewModel);
                }
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    AttachFloor(e.NewItems[i] as FloorNodeViewModel);
                }
            }

            QueueRefresh(_isFitMode);
        }

        private void Sets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                for (var i = 0; i < e.OldItems.Count; i++)
                {
                    DetachSet(e.OldItems[i] as ScheduleSetNodeViewModel);
                }
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    AttachSet(e.NewItems[i] as ScheduleSetNodeViewModel);
                }
            }

            QueueRefresh(_isFitMode);
        }

        private void Floor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FloorNodeViewModel.IsChecked) || e.PropertyName == nameof(FloorNodeViewModel.DisplayName) || e.PropertyName == nameof(FloorNodeViewModel.Name))
            {
                QueueRefresh(_isFitMode);
            }
        }

        private void Set_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScheduleSetNodeViewModel.IsChecked) || e.PropertyName == nameof(ScheduleSetNodeViewModel.DisplayName) || e.PropertyName == nameof(ScheduleSetNodeViewModel.MemberName))
            {
                QueueRefresh(_isFitMode);
            }
        }
    }
}