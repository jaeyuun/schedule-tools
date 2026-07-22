using GirderSchedule.App.Rendering.Models;
using GirderSchedule.App.Rendering.Section;
using GirderSchedule.App.ViewModels.Schedule;
using ScheduleTools.Wpf.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GirderSchedule.App.Views.Schedule
{
    public partial class ScheduleSetEditor : UserControl
    {
        private readonly SectionRenderer _sectionRenderer = new SectionRenderer();
        private ScheduleViewModel _viewModel;
        private bool _isRedrawQueued;

        public ScheduleSetEditor()
        {
            InitializeComponent();

            AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(Input_TextChanged), true);
            AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(CheckBox_Changed), true);
            AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(CheckBox_Changed), true);

            Loaded += SchedulePreview_Loaded;
            Unloaded += SchedulePreview_Unloaded;
            SizeChanged += SchedulePreview_SizeChanged;
            DataContextChanged += SchedulePreview_DataContextChanged;
        }

        private void SchedulePreview_Loaded(object sender, RoutedEventArgs e)
        {
            NumericTextBoxBehavior.Refresh(this);
            QueueRedraw();
        }

        private void SchedulePreview_Unloaded(object sender, RoutedEventArgs e)
        {
            UnsubscribeViewModel();
        }

        private void SchedulePreview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            QueueRedraw();
        }

        private void SchedulePreview_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UnsubscribeViewModel();

            _viewModel = e.NewValue as ScheduleViewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }

            NumericTextBoxBehavior.Refresh(this);
            QueueRedraw();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsEditorRefreshProperty(e.PropertyName))
            {
                return;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                NumericTextBoxBehavior.Refresh(this);
                QueueRedraw();
            }));
        }

        private bool IsEditorRefreshProperty(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return true;
            }

            return propertyName == nameof(ScheduleViewModel.CurrentSet) ||
                   propertyName == nameof(ScheduleViewModel.MemberName) ||
                   propertyName == nameof(ScheduleViewModel.WidthValue) ||
                   propertyName == nameof(ScheduleViewModel.HeightValue) ||
                   propertyName == nameof(ScheduleViewModel.Left) ||
                   propertyName == nameof(ScheduleViewModel.Center) ||
                   propertyName == nameof(ScheduleViewModel.Right) ||
                   propertyName == nameof(ScheduleViewModel.IsWidthOver) ||
                   propertyName == nameof(ScheduleViewModel.IsHeightOver);
        }

        private void UnsubscribeViewModel()
        {
            if (_viewModel == null)
            {
                return;
            }

            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _viewModel = null;
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            QueueRedraw();
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            QueueRedraw();
        }

        private void QueueRedraw()
        {
            if (_isRedrawQueued)
            {
                return;
            }

            _isRedrawQueued = true;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                _isRedrawQueued = false;
                Redraw();
            }));
        }

        private void Redraw()
        {
            var viewModel = DataContext as ScheduleViewModel;

            if (viewModel == null)
            {
                ClearCanvases();
                return;
            }

            DrawSection(LeftCanvas, viewModel.Left, viewModel.IsWidthOver, viewModel.IsHeightOver);
            DrawSection(CenterCanvas, viewModel.Center, viewModel.IsWidthOver, viewModel.IsHeightOver);
            DrawSection(RightCanvas, viewModel.Right, viewModel.IsWidthOver, viewModel.IsHeightOver);
        }

        private void ClearCanvases()
        {
            ClearCanvas(LeftCanvas);
            ClearCanvas(CenterCanvas);
            ClearCanvas(RightCanvas);
        }

        private void ClearCanvas(Canvas canvas)
        {
            if (canvas == null)
            {
                return;
            }

            canvas.Children.Clear();
        }

        private void DrawSection(Canvas canvas, SectionItemViewModel section, bool isWidthOver, bool isHeightOver)
        {
            if (canvas == null)
            {
                return;
            }

            canvas.Children.Clear();

            if (section == null || !section.IsSectionEnabled || section.Model == null || section.Model.Section == null)
            {
                return;
            }

            var areaWidth = canvas.ActualWidth;
            var areaHeight = canvas.ActualHeight;

            if (areaWidth <= 0.0)
            {
                areaWidth = canvas.Width > 0.0 ? canvas.Width : 200.0;
            }

            if (areaHeight <= 0.0)
            {
                areaHeight = canvas.Height > 0.0 ? canvas.Height : 240.0;
            }

            _sectionRenderer.Draw(canvas, section.Model, 0.0, 0.0, areaWidth, areaHeight, isWidthOver, isHeightOver, PreviewSectionRenderOptions.Editor);
        }
    }
}