using System;
using System.Windows;
using System.Windows.Controls;
using GirderSchedule.App.Rendering.Models;
using GirderSchedule.App.Rendering.Section;
using GirderSchedule.App.ViewModels.Schedule;

namespace GirderSchedule.App.Views.Schedule
{
    public partial class ScheduleSetEditor : UserControl
    {
        private readonly SectionRenderer _sectionRenderer = new SectionRenderer();
        private bool _isRedrawQueued;

        public ScheduleSetEditor()
        {
            InitializeComponent();

            AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(Input_TextChanged), true);
            AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(CheckBox_Changed), true);
            AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(CheckBox_Changed), true);

            Loaded += SchedulePreview_Loaded;
            SizeChanged += SchedulePreview_SizeChanged;
            DataContextChanged += SchedulePreview_DataContextChanged;
        }

        private void SchedulePreview_Loaded(object sender, RoutedEventArgs e)
        {
            QueueRedraw();
        }

        private void SchedulePreview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            QueueRedraw();
        }

        private void SchedulePreview_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            QueueRedraw();
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
