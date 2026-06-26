using GirderSchedule.App.Rendering.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GirderSchedule.App.Rendering.Section
{
    public sealed class GirderRenderer
    {
        private readonly CanvasDrawer _drawer;

        public GirderRenderer()
            : this (new CanvasDrawer())
        {
        }

        public GirderRenderer(CanvasDrawer drawer)
        {
            _drawer = drawer;
        }

        public void Draw(Canvas canvas, PreviewSectionLayout layout, PreviewSectionRenderOptions options, Brush lineBrush)
        {
            if (canvas == null || layout == null || options == null)
            {
                return;
            }

            var outer = new Path
            {
                Stroke = lineBrush,
                StrokeThickness = options.OuterLineThickness,
                Fill = Brushes.Transparent,
                Data = CreateGirderOuterGeometry(layout.OuterLeft, layout.OuterRight, layout.OuterTop, layout.OuterShelfY, layout.OuterBottom, layout.SideWing)
            };

            canvas.Children.Add(outer);
            _drawer.DrawRectangle(canvas, layout.SectionLeft, layout.SectionTop, layout.DrawWidth, layout.DrawHeight, lineBrush, options.StirrupBoxThickness, Brushes.Transparent);
        }

        private static Geometry CreateGirderOuterGeometry(double outerLeft, double outerRight, double outerTop, double outerShelfY, double outerBottom, double sideWing)
        {
            var geometry = new PathGeometry();

            var topLine = new PathFigure
            {
                StartPoint = new Point(outerLeft - sideWing, outerTop),
                IsClosed = false
            };

            topLine.Segments.Add(new LineSegment(new Point(outerRight + sideWing, outerTop), true));

            var lowerShape = new PathFigure
            {
                StartPoint = new Point(outerLeft - sideWing, outerShelfY),
                IsClosed = false
            };

            lowerShape.Segments.Add(new LineSegment(new Point(outerLeft, outerShelfY), true));
            lowerShape.Segments.Add(new LineSegment(new Point(outerLeft, outerBottom), true));
            lowerShape.Segments.Add(new LineSegment(new Point(outerRight, outerBottom), true));
            lowerShape.Segments.Add(new LineSegment(new Point(outerRight, outerShelfY), true));
            lowerShape.Segments.Add(new LineSegment(new Point(outerRight + sideWing, outerShelfY), true));

            geometry.Figures.Add(topLine);
            geometry.Figures.Add(lowerShape);

            return geometry;
        }
    }
}