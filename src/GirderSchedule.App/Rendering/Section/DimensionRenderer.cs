using GirderSchedule.App.Rendering.Models;
using System.Globalization;
using ScheduleTools.Wpf.Rendering;
using System.Windows.Controls;
using System.Windows.Media;

namespace GirderSchedule.App.Rendering.Section
{
    public sealed class DimensionRenderer
    {
        private readonly CanvasDrawer _drawer;

        public DimensionRenderer()
            : this(new CanvasDrawer())
        {
        }

        public DimensionRenderer(CanvasDrawer drawer)
        {
            _drawer = drawer;
        }

        public void Draw(Canvas canvas, PreviewSectionLayout layout, double widthValue, double heightValue, bool isWidthOver, bool isHeightOver, PreviewSectionRenderOptions options, Brush lineBrush)
        {
            if (canvas == null || layout == null || options == null)
            {
                return;
            }

            const double auxGap = 12.0;

            var scale = layout.DimensionScale;
            var dimensionGap = System.Math.Max(options.DimensionGapMinimum, 34.0 * scale);
            var extensionOver = System.Math.Max(options.DimensionExtensionMinimum, 12.0 * scale);
            var tick = System.Math.Max(options.DimensionTickMinimum, 8.0 * scale);
            var circleRadius = System.Math.Max(options.DimensionCircleMinimum, 3.0 * scale);
            var textFontSize = System.Math.Max(options.DimensionTextMinimum, 16.0 * scale);

            var widthText = FormatDimensionText(widthValue, isWidthOver);
            var heightText = FormatDimensionText(heightValue, isHeightOver);
            var outerVisualLeft = layout.OuterLeft - layout.SideWing;

            var topDimY = layout.OuterTop - dimensionGap;
            var topExtTop = topDimY - extensionOver;
            var topExtBottom = layout.OuterTop - auxGap * scale;

            _drawer.DrawLine(canvas, layout.OuterLeft, topDimY, layout.OuterRight, topDimY, lineBrush, options.DimensionLineThickness);
            _drawer.DrawLine(canvas, layout.OuterLeft, topExtTop, layout.OuterLeft, topExtBottom, lineBrush, options.DimensionLineThickness);
            _drawer.DrawLine(canvas, layout.OuterRight, topExtTop, layout.OuterRight, topExtBottom, lineBrush, options.DimensionLineThickness);

            DrawDimensionMark(canvas, layout.OuterLeft, topDimY, circleRadius, tick, options, lineBrush);
            DrawDimensionMark(canvas, layout.OuterRight, topDimY, circleRadius, tick, options, lineBrush);
            _drawer.DrawCenteredText(canvas, layout.OuterLeft + (layout.OuterRight - layout.OuterLeft) / 2.0, topDimY - textFontSize * 0.9, widthText, textFontSize, 0.0, lineBrush);

            var leftDimX = outerVisualLeft - dimensionGap;
            var leftExtLeft = leftDimX - extensionOver;
            var leftExtRight = outerVisualLeft - auxGap * scale;

            _drawer.DrawLine(canvas, leftDimX, layout.OuterTop, leftDimX, layout.OuterBottom, lineBrush, options.DimensionLineThickness);
            _drawer.DrawLine(canvas, leftExtLeft, layout.OuterTop, leftExtRight, layout.OuterTop, lineBrush, options.DimensionLineThickness);
            _drawer.DrawLine(canvas, leftExtLeft, layout.OuterBottom, leftExtRight, layout.OuterBottom, lineBrush, options.DimensionLineThickness);

            DrawDimensionMark(canvas, leftDimX, layout.OuterTop, circleRadius, tick, options, lineBrush);
            DrawDimensionMark(canvas, leftDimX, layout.OuterBottom, circleRadius, tick, options, lineBrush);
            _drawer.DrawCenteredText(canvas, leftDimX - textFontSize * 0.85, layout.OuterTop + (layout.OuterBottom - layout.OuterTop) / 2.0, heightText, textFontSize, -90.0, lineBrush);
        }

        private void DrawDimensionMark(Canvas canvas, double x, double y, double radius, double tick, PreviewSectionRenderOptions options, Brush lineBrush)
        {
            _drawer.DrawCircle(canvas, x, y, radius, lineBrush, options.DimensionMarkThickness, Brushes.Transparent);
            _drawer.DrawLine(canvas, x - tick, y, x + tick, y, lineBrush, options.DimensionMarkThickness);
            _drawer.DrawLine(canvas, x, y - tick, x, y + tick, lineBrush, options.DimensionMarkThickness);
        }

        private static string FormatDimensionText(double value, bool isOver)
        {
            var text = value.ToString(CultureInfo.InvariantCulture);

            if (!isOver)
            {
                return text;
            }

            return text + " 이상";
        }
    }
}
