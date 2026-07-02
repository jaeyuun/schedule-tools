using GirderSchedule.App.Rendering.Previews;
using GirderSchedule.App.Services.Previews;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.ViewModels.Export;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.App.Views.Schedule.Behaviors;
using GirderSchedule.Domain.Models.Export;
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
        public static readonly DependencyProperty ScheduleTitleProperty = DependencyProperty.Register(nameof(ScheduleTitle), typeof(string), typeof(ScheduleExportPreview), new PropertyMetadata("보 일람표", OnScheduleTitleChanged));

        private readonly PreviewRenderer _renderer = new PreviewRenderer();
        private readonly ScheduleExportPageBuilder _pageBuilder = new ScheduleExportPageBuilder();
        private readonly SchedulePreviewChangeWatcher _changeWatcher = new SchedulePreviewChangeWatcher();

        private readonly List<ScheduleExportPage> _pages = new List<ScheduleExportPage>();

        private PreviewPanZoomController _panZoomController;
        private bool _isRefreshQueued;
        private int _currentPageIndex;

        public event EventHandler PreviewStateChanged;

        public ObservableCollection<FloorNodeViewModel> Floors
        {
            get { return (ObservableCollection<FloorNodeViewModel>)GetValue(FloorsProperty); }
            set { SetValue(FloorsProperty, value); }
        }

        public string ScheduleTitle
        {
            get { return (string)GetValue(ScheduleTitleProperty); }
            set { SetValue(ScheduleTitleProperty, value); }
        }

        public bool CanGoPreviousPage
        {
            get { return _pages.Count > 0 && _currentPageIndex > 0; }
        }

        public bool CanGoNextPage
        {
            get { return _pages.Count > 0 && _currentPageIndex < GetPageCount() - 1; }
        }

        public string PageText
        {
            get { return (_currentPageIndex + 1) + " / " + GetPageCount(); }
        }

        public string ZoomText
        {
            get
            {
                if (_panZoomController == null)
                {
                    return "100%";
                }

                return ((int)Math.Round(PreviewScaleTransform.ScaleX * 100.0)) + "%";
            }
        }

        public ScheduleExportPreview()
        {
            InitializeComponent();

            Loaded += ScheduleExportPreview_Loaded;
            Unloaded += ScheduleExportPreview_Unloaded;
            SizeChanged += ScheduleExportPreview_SizeChanged;
        }

        public void GoPreviousPage()
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

            RaisePreviewStateChanged();
        }

        public void GoNextPage()
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

            RaisePreviewStateChanged();
        }

        public void FitToScreenView()
        {
            FitToScreen();
            RaisePreviewStateChanged();
        }

        private static void OnFloorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ScheduleExportPreview;

            if (control == null)
            {
                return;
            }

            var newFloors = e.NewValue as ObservableCollection<FloorNodeViewModel>;

            control._changeWatcher.Attach(newFloors, control.QueueRefreshByWatcher);
            control.QueueRefresh(true);
        }

        private static void OnScheduleTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ScheduleExportPreview;

            if (control == null)
            {
                return;
            }

            control.QueueRefresh(control.IsFitMode());
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
            RaisePreviewStateChanged();
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

                RaisePreviewStateChanged();
            }));
        }

        private void RebuildPages()
        {
            _pages.Clear();

            var setting = new DxfExportSetting();
            setting.ContinueAcrossFloors = true;

            var pages = _pageBuilder.Build(ScheduleTitle, Floors, setting);

            for (var i = 0; i < pages.Count; i++)
            {
                _pages.Add(pages[i]);
            }

            NormalizePageIndex();
            RaisePreviewStateChanged();
        }

        private void NormalizePageIndex()
        {
            var pageCount = GetPageCount();

            if (pageCount <= 0)
            {
                _currentPageIndex = 0;
                return;
            }

            if (_currentPageIndex < 0)
            {
                _currentPageIndex = 0;
            }

            if (_currentPageIndex >= pageCount)
            {
                _currentPageIndex = pageCount - 1;
            }
        }

        private void Redraw()
        {
            if (PreviewCanvas == null)
            {
                return;
            }

            PreviewCanvas.Children.Clear();

            var page = GetCurrentPage();
            _renderer.Draw(PreviewCanvas, page, _currentPageIndex + 1, GetPageCount());

            RaisePreviewStateChanged();
        }

        private ScheduleExportPage GetCurrentPage()
        {
            if (_pages.Count == 0)
            {
                return null;
            }

            if (_currentPageIndex < 0 || _currentPageIndex >= _pages.Count)
            {
                return null;
            }

            return _pages[_currentPageIndex];
        }

        private int GetPageCount()
        {
            if (_pages.Count == 0)
            {
                return 1;
            }

            return _pages.Count;
        }

        private void RaisePreviewStateChanged()
        {
            var handler = PreviewStateChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}