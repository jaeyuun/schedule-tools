using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GirderSchedule.App.ViewModels.Girder;
using GirderSchedule.Domain.Girder.Layout;
using GirderSchedule.Domain.Girder.Models;

namespace GirderSchedule.App.Views.Girder
{
    public partial class SchedulePreview : UserControl
    {
        private readonly RebarPointLayoutService _rebarLayoutService;

        public SchedulePreview()
        {
            InitializeComponent();
            _rebarLayoutService = new RebarPointLayoutService();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Redraw();
        }

        private void RedrawButton_Click(object sender, RoutedEventArgs e)
        {
            Redraw();
        }

        private void Redraw()
        {
            var vm = DataContext as ScheduleViewModel;

            if (vm == null)
            {
                return;
            }

            DrawSection(LeftCanvas, vm.Left.Model);
            DrawSection(CenterCanvas, vm.Center.Model);
            DrawSection(RightCanvas, vm.Right.Model);
        }

        private void DrawSection(Canvas canvas, ScheduleItem item)
        {
            canvas.Children.Clear();

            var width = item.Section.Width <= 0 ? 400 : item.Section.Width;
            var height = item.Section.Height <= 0 ? 600 : item.Section.Height;

            var maxW = 190.0;
            var maxH = 210.0;
            var scale = Math.Min(maxW / width, maxH / height);

            var beamW = width * scale;
            var beamH = height * scale;

            var centerX = canvas.Width / 2.0;
            var topY = 52.0;
            var leftX = centerX - beamW / 2.0;

            var slabOverhang = 42.0;
            var slabThickness = 26.0;

            var slab = new Polyline();
            slab.Stroke = Brushes.Black;
            slab.StrokeThickness = 1.2;
            slab.Points.Add(new Point(leftX - slabOverhang, topY));
            slab.Points.Add(new Point(leftX, topY));
            slab.Points.Add(new Point(leftX, topY + slabThickness));
            slab.Points.Add(new Point(leftX + beamW, topY + slabThickness));
            slab.Points.Add(new Point(leftX + beamW, topY));
            slab.Points.Add(new Point(leftX + beamW + slabOverhang, topY));
            canvas.Children.Add(slab);

            var beam = new Rectangle();
            beam.Width = beamW;
            beam.Height = beamH;
            beam.Stroke = Brushes.Black;
            beam.StrokeThickness = 1.4;
            Canvas.SetLeft(beam, leftX);
            Canvas.SetTop(beam, topY + slabThickness);
            canvas.Children.Add(beam);

            var stirrup = new Rectangle();
            stirrup.Width = Math.Max(beamW - 28.0, 10.0);
            stirrup.Height = Math.Max(beamH - 28.0, 10.0);
            stirrup.Stroke = Brushes.Green;
            stirrup.StrokeThickness = 1.2;
            Canvas.SetLeft(stirrup, leftX + 14.0);
            Canvas.SetTop(stirrup, topY + slabThickness + 14.0);
            canvas.Children.Add(stirrup);

            var barStartX = leftX + 28.0;
            var barEndX = leftX + beamW - 28.0;

            var topBarY1 = topY + slabThickness + 28.0;
            var topBarY2 = topY + slabThickness + 50.0;

            var bottomBarY1 = topY + slabThickness + beamH - 28.0;
            var bottomBarY2 = topY + slabThickness + beamH - 50.0;

            DrawBars(canvas, _rebarLayoutService.GetFirstLayerXs(barStartX, barEndX, item.TopRebar.FirstLayer.Count), topBarY1);
            DrawBars(canvas, _rebarLayoutService.GetSecondLayerXs(barStartX, barEndX, item.TopRebar.FirstLayer.Count, item.TopRebar.SecondLayer.Count), topBarY2);

            DrawBars(canvas, _rebarLayoutService.GetFirstLayerXs(barStartX, barEndX, item.BottomRebar.FirstLayer.Count), bottomBarY1);
            DrawBars(canvas, _rebarLayoutService.GetSecondLayerXs(barStartX, barEndX, item.BottomRebar.FirstLayer.Count, item.BottomRebar.SecondLayer.Count), bottomBarY2);

            DrawText(canvas, item.Position, centerX, 16.0, 13.0);
            DrawText(canvas, string.Format("{0:0} x {1:0}", width, height), centerX, 276.0, 12.0);
        }

        private void DrawBars(Canvas canvas, System.Collections.Generic.List<double> xs, double y)
        {
            for (var i = 0; i < xs.Count; i++)
            {
                DrawBar(canvas, xs[i], y);
            }
        }

        private void DrawBar(Canvas canvas, double x, double y)
        {
            var circle = new Ellipse();
            circle.Width = 10.0;
            circle.Height = 10.0;
            circle.Fill = Brushes.Green;
            circle.Stroke = Brushes.Green;
            circle.StrokeThickness = 1.0;

            Canvas.SetLeft(circle, x - circle.Width / 2.0);
            Canvas.SetTop(circle, y - circle.Height / 2.0);

            canvas.Children.Add(circle);
        }

        private void DrawText(Canvas canvas, string text, double x, double y, double size)
        {
            var block = new TextBlock();
            block.Text = text;
            block.FontSize = size;
            block.Foreground = Brushes.Black;

            canvas.Children.Add(block);
            block.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            Canvas.SetLeft(block, x - block.DesiredSize.Width / 2.0);
            Canvas.SetTop(block, y - block.DesiredSize.Height / 2.0);
        }
    }
}