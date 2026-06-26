using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GirderSchedule.App.Views.Schedule.Behaviors
{
    public sealed class PreviewPanZoomController
    {
        private const double MinZoom = 0.2;
        private const double MaxZoom = 4.0;
        private const double WheelZoomFactor = 1.1;

        private readonly ScrollViewer _scrollViewer;
        private readonly FrameworkElement _target;
        private readonly ScaleTransform _scaleTransform;
        private readonly Action<double> _zoomChanged;

        private bool _isAttached;
        private bool _isPanning;
        private bool _isFitMode = true;
        private double _zoomScale = 1.0;
        private Point _panStartPoint;
        private Point _panStartOffset;

        public PreviewPanZoomController(ScrollViewer scrollViewer, FrameworkElement target, ScaleTransform scaleTransform, Action<double> zoomChanged)
        {
            _scrollViewer = scrollViewer;
            _target = target;
            _scaleTransform = scaleTransform;
            _zoomChanged = zoomChanged;
        }

        public bool IsFitMode
        {
            get { return _isFitMode; }
        }

        public double ZoomScale
        {
            get { return _zoomScale; }
        }

        public void Attach()
        {
            if (_isAttached || _scrollViewer == null)
            {
                return;
            }

            _scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            _scrollViewer.PreviewMouseDown += ScrollViewer_PreviewMouseDown;
            _scrollViewer.PreviewMouseMove += ScrollViewer_PreviewMouseMove;
            _scrollViewer.PreviewMouseUp += ScrollViewer_PreviewMouseUp;
            _scrollViewer.MouseLeave += ScrollViewer_MouseLeave;

            _isAttached = true;
        }

        public void Detach()
        {
            if (!_isAttached || _scrollViewer == null)
            {
                return;
            }

            EndPan();

            _scrollViewer.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;
            _scrollViewer.PreviewMouseDown -= ScrollViewer_PreviewMouseDown;
            _scrollViewer.PreviewMouseMove -= ScrollViewer_PreviewMouseMove;
            _scrollViewer.PreviewMouseUp -= ScrollViewer_PreviewMouseUp;
            _scrollViewer.MouseLeave -= ScrollViewer_MouseLeave;

            _isAttached = false;
        }

        public void FitToScreen()
        {
            if (_scrollViewer == null || _target == null)
            {
                return;
            }

            var viewportWidth = _scrollViewer.ActualWidth;
            var viewportHeight = _scrollViewer.ActualHeight;

            if (viewportWidth <= 0 || viewportHeight <= 0)
            {
                return;
            }

            var targetWidth = _target.Width;
            var targetHeight = _target.Height;

            if (targetWidth <= 0 || targetHeight <= 0)
            {
                targetWidth = _target.ActualWidth;
                targetHeight = _target.ActualHeight;
            }

            if (targetWidth <= 0 || targetHeight <= 0)
            {
                return;
            }

            var scaleX = viewportWidth / targetWidth;
            var scaleY = viewportHeight / targetHeight;
            var scale = Math.Min(scaleX, scaleY);

            if (scale > 1.0)
            {
                scale = 1.0;
            }

            SetZoom(scale, true);

            _scrollViewer.ScrollToHorizontalOffset(0);
            _scrollViewer.ScrollToVerticalOffset(0);
        }

        public void SetZoom(double scale, bool fitMode)
        {
            scale = ClampZoom(scale);

            _isFitMode = fitMode;
            _zoomScale = scale;

            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = scale;
                _scaleTransform.ScaleY = scale;
            }

            if (_zoomChanged != null)
            {
                _zoomChanged(scale);
            }
        }

        public void EndPan()
        {
            _isPanning = false;

            if (_scrollViewer == null)
            {
                return;
            }

            _scrollViewer.ReleaseMouseCapture();
            _scrollViewer.Cursor = Cursors.Arrow;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_scrollViewer == null)
            {
                return;
            }

            var mousePosition = e.GetPosition(_scrollViewer);
            var scale = e.Delta > 0 ? _zoomScale * WheelZoomFactor : _zoomScale / WheelZoomFactor;

            SetZoomAtPoint(scale, mousePosition);

            e.Handled = true;
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_scrollViewer == null || e.ChangedButton != MouseButton.Middle)
            {
                return;
            }

            _isPanning = true;
            _panStartPoint = e.GetPosition(_scrollViewer);
            _panStartOffset = new Point(_scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset);

            _scrollViewer.CaptureMouse();
            _scrollViewer.Cursor = Cursors.ScrollAll;

            e.Handled = true;
        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_scrollViewer == null || !_isPanning)
            {
                return;
            }

            var currentPoint = e.GetPosition(_scrollViewer);
            var deltaX = currentPoint.X - _panStartPoint.X;
            var deltaY = currentPoint.Y - _panStartPoint.Y;

            _scrollViewer.ScrollToHorizontalOffset(_panStartOffset.X - deltaX);
            _scrollViewer.ScrollToVerticalOffset(_panStartOffset.Y - deltaY);

            e.Handled = true;
        }

        private void ScrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle)
            {
                return;
            }

            EndPan();
            e.Handled = true;
        }

        private void ScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isPanning)
            {
                return;
            }

            EndPan();
        }

        private void SetZoomAtPoint(double scale, Point mousePosition)
        {
            if (_scrollViewer == null)
            {
                SetZoom(scale, false);
                return;
            }

            var oldScale = _zoomScale;

            if (oldScale <= 0)
            {
                oldScale = 1.0;
            }

            scale = ClampZoom(scale);

            var contentX = (_scrollViewer.HorizontalOffset + mousePosition.X) / oldScale;
            var contentY = (_scrollViewer.VerticalOffset + mousePosition.Y) / oldScale;

            SetZoom(scale, false);

            _scrollViewer.ScrollToHorizontalOffset(contentX * scale - mousePosition.X);
            _scrollViewer.ScrollToVerticalOffset(contentY * scale - mousePosition.Y);
        }

        private double ClampZoom(double scale)
        {
            if (scale < MinZoom)
            {
                return MinZoom;
            }

            if (scale > MaxZoom)
            {
                return MaxZoom;
            }

            return scale;
        }
    }
}