using GirderSchedule.App.Rendering.Previews;
using GirderSchedule.App.Services.Previews;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.App.Views.Schedule.Behaviors;
using GirderSchedule.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace GirderSchedule.App.Views.Schedule
{
    public partial class ScheduleExportPreview : UserControl
    {
        public static readonly DependencyProperty FloorsProperty = DependencyProperty.Register(nameof(Floors), typeof(ObservableCollection<FloorNodeViewModel>), typeof(ScheduleExportPreview), new PropertyMetadata(null, OnFloorsChanged));

        private const int PageSize = 9;

        private readonly PreviewRenderer _renderer = new PreviewRenderer();
        private readonly SchedulePreviewPageService _pageService = new SchedulePreviewPageService();
        private readonly SchedulePreviewChangeWatcher _changeWatcher = new SchedulePreviewChangeWatcher();

        private PreviewPanZoomController _panZoomController;
        private bool _isRefreshQueued;
        private int _currentPageIndex;

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

            control._changeWatcher.Attach(newFloors, control.QueueRefreshByWatcher);
            control.QueueRefresh(true);
        }

        private void QueueRefreshByWatcher()
        {
            QueueRefresh(IsFitMode());
        }

        private void ScheduleExportPreview_Loaded(object sender, RoutedEventArgs e)
        {
            CreatePanZoomController();
            _changeWatcher.Attach(Floors, QueueRefreshByWatcher);
            QueueRefresh(true);
        }

        private void ScheduleExportPreview_Unloaded(object sender, RoutedEventArgs e)
        {
            _changeWatcher.Detach();
            DestroyPanZoomController();
        }

        private void ScheduleExportPreview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsFitMode())
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

            if (IsFitMode())
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

            if (IsFitMode())
            {
                FitToScreen();
            }
        }

        private void Fit_Click(object sender, RoutedEventArgs e)
        {
            FitToScreen();
        }

        private void CreatePanZoomController()
        {
            if (_panZoomController != null)
            {
                return;
            }

            _panZoomController = new PreviewPanZoomController(PreviewScrollViewer, PreviewCanvas, PreviewScaleTransform, UpdateZoomText);
            _panZoomController.Attach();
        }

        private void DestroyPanZoomController()
        {
            if (_panZoomController == null)
            {
                return;
            }

            _panZoomController.Detach();
            _panZoomController = null;
        }

        private bool IsFitMode()
        {
            return _panZoomController == null || _panZoomController.IsFitMode;
        }

        private void FitToScreen()
        {
            CreatePanZoomController();

            if (_panZoomController == null)
            {
                return;
            }

            _panZoomController.FitToScreen();
        }

        private void UpdateZoomText(double scale)
        {
            if (ZoomTextBlock == null)
            {
                return;
            }

            ZoomTextBlock.Text = ((int)Math.Round(scale * 100.0)) + "%";
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
            _pageService.Rebuild(Floors);
            _currentPageIndex = _pageService.NormalizePageIndex(_currentPageIndex, PageSize);

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
            _renderer.Draw(PreviewCanvas, pageSets, _currentPageIndex + 1, GetPageCount());

            UpdatePageState();
        }

        private List<ScheduleSet> GetCurrentPageSets()
        {
            return _pageService.GetPageSets(_currentPageIndex, PageSize);
        }

        private int GetPageCount()
        {
            return _pageService.GetPageCount(PageSize);
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
    }
}