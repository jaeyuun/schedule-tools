using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GirderSchedule.App.Preview.Layouts;
using GirderSchedule.App.Preview.Models;
using GirderSchedule.Domain.Girder.Models;

namespace GirderSchedule.App.Preview.Rendering
{
    public sealed class PreviewSectionRenderer
    {
        private const double SkinRebarApplyHeight = 900.0;
        private const double SkinMarkSizeUnit = 25.0;
        private const double SkinMarkWidthUnit = 5.0;

        private readonly Brush _lineBrush = new SolidColorBrush(Color.FromRgb(25, 25, 25));
        private readonly Brush _barBrush = new SolidColorBrush(Color.FromRgb(70, 70, 70));
        private readonly PreviewCanvasDrawer _drawer = new PreviewCanvasDrawer();
        private readonly PreviewSectionLayoutFactory _layoutFactory = new PreviewSectionLayoutFactory();

        public void Draw(Canvas canvas, ScheduleItem item, double areaX, double areaY, double areaWidth, double areaHeight, bool isWidthOver, bool isHeightOver, PreviewSectionRenderOptions options)
        {
            if (canvas == null || item == null || item.Section == null)
            {
                return;
            }

            var renderOptions = options ?? PreviewSectionRenderOptions.Editor;
            var layout = _layoutFactory.Create(areaX, areaY, areaWidth, areaHeight, item.Section.Width, item.Section.Height, renderOptions);

            DrawGirderOutline(canvas, layout, renderOptions);
            DrawMainRebars(canvas, layout, item, renderOptions);
            DrawStirrups(canvas, layout, item, renderOptions);
            DrawSkinRebarMarks(canvas, layout, item);
            DrawDimensions(canvas, layout, item.Section.Width, item.Section.Height, isWidthOver, isHeightOver, renderOptions);
        }

        private void DrawGirderOutline(Canvas canvas, PreviewSectionLayout layout, PreviewSectionRenderOptions options)
        {
            var outer = new Path
            {
                Stroke = _lineBrush,
                StrokeThickness = options.OuterLineThickness,
                Fill = Brushes.Transparent,
                Data = CreateGirderOuterGeometry(layout.OuterLeft, layout.OuterRight, layout.OuterTop, layout.OuterShelfY, layout.OuterBottom, layout.SideWing)
            };

            canvas.Children.Add(outer);
            _drawer.DrawRectangle(canvas, layout.SectionLeft, layout.SectionTop, layout.DrawWidth, layout.DrawHeight, _lineBrush, options.StirrupBoxThickness, Brushes.Transparent);
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

        private void DrawMainRebars(Canvas canvas, PreviewSectionLayout layout, ScheduleItem item, PreviewSectionRenderOptions options)
        {
            var topFirstCount = GetTopFirstCount(item);
            var topSecondCount = GetTopSecondCount(item);
            var bottomFirstCount = GetBottomFirstCount(item);
            var bottomSecondCount = GetBottomSecondCount(item);

            DrawFirstLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.TopBarY1, topFirstCount, options.BarRadius);
            DrawSecondLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.TopBarY2, topFirstCount, topSecondCount, options.BarRadius);

            DrawFirstLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.BottomBarY1, bottomFirstCount, options.BarRadius);
            DrawSecondLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.BottomBarY2, bottomFirstCount, bottomSecondCount, options.BarRadius);
        }

        private void DrawFirstLayerRebars(Canvas canvas, double startX, double endX, double y, int count, double radius)
        {
            if (count <= 0)
            {
                return;
            }

            if (count == 1)
            {
                AddBar(canvas, startX, y, radius);
                return;
            }

            var spacing = (endX - startX) / (count - 1);

            for (var i = 0; i < count; i++)
            {
                AddBar(canvas, startX + spacing * i, y, radius);
            }
        }

        private void DrawSecondLayerRebars(Canvas canvas, double startX, double endX, double y, int firstLayerCount, int secondLayerCount, double radius)
        {
            if (firstLayerCount <= 0 || secondLayerCount <= 0)
            {
                return;
            }

            if (firstLayerCount == 1)
            {
                AddBar(canvas, startX, y, radius);
                return;
            }

            if (secondLayerCount > firstLayerCount)
            {
                secondLayerCount = firstLayerCount;
            }

            var spacing = (endX - startX) / (firstLayerCount - 1);
            var leftCount = (secondLayerCount + 1) / 2;
            var rightCount = secondLayerCount / 2;
            var drawn = new bool[firstLayerCount];

            for (var i = 0; i < leftCount && i < firstLayerCount; i++)
            {
                drawn[i] = true;
            }

            for (var i = 0; i < rightCount && i < firstLayerCount; i++)
            {
                drawn[firstLayerCount - 1 - i] = true;
            }

            for (var i = 0; i < firstLayerCount; i++)
            {
                if (!drawn[i])
                {
                    continue;
                }

                AddBar(canvas, startX + spacing * i, y, radius);
            }
        }

        private void AddBar(Canvas canvas, double x, double y, double radius)
        {
            _drawer.DrawCircle(canvas, x, y, radius, null, 0.0, _barBrush);
        }

        private void DrawStirrups(Canvas canvas, PreviewSectionLayout layout, ScheduleItem item, PreviewSectionRenderOptions options)
        {
            var topFirstLayerCount = GetTopFirstCount(item);
            var bottomFirstLayerCount = GetBottomFirstCount(item);
            var stirrupLegs = GetStirrupLegs(item);

            if (stirrupLegs <= 2)
            {
                return;
            }

            if (topFirstLayerCount <= 2 || bottomFirstLayerCount <= 2)
            {
                return;
            }

            var availableLegs = Math.Min(stirrupLegs, bottomFirstLayerCount);
            var innerCount = availableLegs - 2;

            if (innerCount <= 0)
            {
                return;
            }

            var topXs = GetLayerRebarXs(layout.BarStartX, layout.BarEndX, topFirstLayerCount);
            var bottomXs = GetLayerRebarXs(layout.BarStartX, layout.BarEndX, bottomFirstLayerCount);
            var usedBottom = new bool[bottomFirstLayerCount];

            for (var i = 1; i <= innerCount; i++)
            {
                var rawIndex = (topFirstLayerCount - 1) * i / (double)(availableLegs - 1);
                var topIndex = (int)Math.Floor(rawIndex + 0.5 - 0.000001);

                if (topIndex <= 0)
                {
                    topIndex = 1;
                }

                if (topIndex >= topFirstLayerCount - 1)
                {
                    topIndex = topFirstLayerCount - 2;
                }

                var topX = topXs[topIndex];
                var direction = topIndex < topFirstLayerCount / 2.0 ? -1.0 : 1.0;
                var bottomIndex = FindNearestIndex(bottomXs, topX, usedBottom);

                if (bottomIndex < 0)
                {
                    continue;
                }

                usedBottom[bottomIndex] = true;

                var bottomX = bottomXs[bottomIndex];
                var topLineX = topX + direction * options.BarRadius;
                var bottomLineX = bottomX + direction * options.BarRadius;

                topLineX = Clamp(topLineX, layout.BarStartX, layout.BarEndX);
                bottomLineX = Clamp(bottomLineX, layout.BarStartX, layout.BarEndX);

                _drawer.DrawLine(canvas, topLineX, layout.SectionTop, bottomLineX, layout.SectionBottom, _lineBrush, options.StirrupLineThickness);
            }
        }

        private static double[] GetLayerRebarXs(double startX, double endX, int count)
        {
            if (count <= 0)
            {
                return new double[0];
            }

            if (count == 1)
            {
                return new[] { startX };
            }

            var values = new double[count];
            var spacing = (endX - startX) / (count - 1);

            for (var i = 0; i < count; i++)
            {
                values[i] = startX + spacing * i;
            }

            return values;
        }

        private static int FindNearestIndex(double[] values, double targetX, bool[] used)
        {
            var index = -1;
            var distance = double.MaxValue;

            for (var i = 0; i < values.Length; i++)
            {
                if (used != null && i < used.Length && used[i])
                {
                    continue;
                }

                var currentDistance = Math.Abs(values[i] - targetX);

                if (currentDistance >= distance)
                {
                    continue;
                }

                index = i;
                distance = currentDistance;
            }

            return index;
        }

        private void DrawSkinRebarMarks(Canvas canvas, PreviewSectionLayout layout, ScheduleItem item)
        {
            if (item == null || item.Section == null || item.Section.Height < SkinRebarApplyHeight)
            {
                return;
            }

            var size = SkinMarkSizeUnit * layout.ShapeScale;
            var width = SkinMarkWidthUnit * layout.ShapeScale;

            if (size < 4.0)
            {
                size = 4.0;
            }

            if (width < 1.0)
            {
                width = 1.0;
            }

            var halfSize = size / 2.0;
            var halfWidth = width / 2.0;
            var clearance = halfWidth / Math.Sqrt(2.0);
            var inset = halfSize + clearance;

            var stirrupLeft = layout.SectionLeft;
            var stirrupRight = layout.SectionLeft + layout.DrawWidth;
            var centerY = (layout.OuterTop + layout.OuterBottom) / 2.0;

            if (stirrupRight - stirrupLeft <= inset * 2.0)
            {
                return;
            }

            AddXMark(canvas, stirrupLeft + inset, centerY, size, width);
            AddXMark(canvas, stirrupRight - inset, centerY, size, width);
        }

        private void AddXMark(Canvas canvas, double x, double y, double size, double thickness)
        {
            var half = size / 2.0;

            _drawer.DrawLine(canvas, x - half, y - half, x + half, y + half, _lineBrush, thickness);
            _drawer.DrawLine(canvas, x - half, y + half, x + half, y - half, _lineBrush, thickness);
        }

        private void DrawDimensions(Canvas canvas, PreviewSectionLayout layout, double widthValue, double heightValue, bool isWidthOver, bool isHeightOver, PreviewSectionRenderOptions options)
        {
            const double auxGap = 12.0;

            var scale = layout.DimensionScale;
            var dimensionGap = Math.Max(options.DimensionGapMinimum, 34.0 * scale);
            var extensionOver = Math.Max(options.DimensionExtensionMinimum, 12.0 * scale);
            var tick = Math.Max(options.DimensionTickMinimum, 8.0 * scale);
            var circleRadius = Math.Max(options.DimensionCircleMinimum, 3.0 * scale);
            var textFontSize = Math.Max(options.DimensionTextMinimum, 16.0 * scale);

            var widthText = FormatDimensionText(widthValue, isWidthOver);
            var heightText = FormatDimensionText(heightValue, isHeightOver);
            var outerVisualLeft = layout.OuterLeft - layout.SideWing;

            var topDimY = layout.OuterTop - dimensionGap;
            var topExtTop = topDimY - extensionOver;
            var topExtBottom = layout.OuterTop - auxGap * scale;

            _drawer.DrawLine(canvas, layout.OuterLeft, topDimY, layout.OuterRight, topDimY, _lineBrush, options.DimensionLineThickness);
            _drawer.DrawLine(canvas, layout.OuterLeft, topExtTop, layout.OuterLeft, topExtBottom, _lineBrush, options.DimensionLineThickness);
            _drawer.DrawLine(canvas, layout.OuterRight, topExtTop, layout.OuterRight, topExtBottom, _lineBrush, options.DimensionLineThickness);

            DrawDimensionMark(canvas, layout.OuterLeft, topDimY, circleRadius, tick, options);
            DrawDimensionMark(canvas, layout.OuterRight, topDimY, circleRadius, tick, options);
            _drawer.DrawCenteredText(canvas, layout.OuterLeft + (layout.OuterRight - layout.OuterLeft) / 2.0, topDimY - textFontSize * 0.9, widthText, textFontSize, 0.0, _lineBrush);

            var leftDimX = outerVisualLeft - dimensionGap;
            var leftExtLeft = leftDimX - extensionOver;
            var leftExtRight = outerVisualLeft - auxGap * scale;

            _drawer.DrawLine(canvas, leftDimX, layout.OuterTop, leftDimX, layout.OuterBottom, _lineBrush, options.DimensionLineThickness);
            _drawer.DrawLine(canvas, leftExtLeft, layout.OuterTop, leftExtRight, layout.OuterTop, _lineBrush, options.DimensionLineThickness);
            _drawer.DrawLine(canvas, leftExtLeft, layout.OuterBottom, leftExtRight, layout.OuterBottom, _lineBrush, options.DimensionLineThickness);

            DrawDimensionMark(canvas, leftDimX, layout.OuterTop, circleRadius, tick, options);
            DrawDimensionMark(canvas, leftDimX, layout.OuterBottom, circleRadius, tick, options);
            _drawer.DrawCenteredText(canvas, leftDimX - textFontSize * 0.85, layout.OuterTop + (layout.OuterBottom - layout.OuterTop) / 2.0, heightText, textFontSize, -90.0, _lineBrush);
        }

        private void DrawDimensionMark(Canvas canvas, double x, double y, double radius, double tick, PreviewSectionRenderOptions options)
        {
            _drawer.DrawCircle(canvas, x, y, radius, _lineBrush, options.DimensionMarkThickness, Brushes.Transparent);
            _drawer.DrawLine(canvas, x - tick, y, x + tick, y, _lineBrush, options.DimensionMarkThickness);
            _drawer.DrawLine(canvas, x, y - tick, x, y + tick, _lineBrush, options.DimensionMarkThickness);
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

        private static int GetTopFirstCount(ScheduleItem item)
        {
            return item == null || item.TopRebar == null || item.TopRebar.FirstLayer == null ? 0 : item.TopRebar.FirstLayer.Count;
        }

        private static int GetTopSecondCount(ScheduleItem item)
        {
            return item == null || item.TopRebar == null || item.TopRebar.SecondLayer == null ? 0 : item.TopRebar.SecondLayer.Count;
        }

        private static int GetBottomFirstCount(ScheduleItem item)
        {
            return item == null || item.BottomRebar == null || item.BottomRebar.FirstLayer == null ? 0 : item.BottomRebar.FirstLayer.Count;
        }

        private static int GetBottomSecondCount(ScheduleItem item)
        {
            return item == null || item.BottomRebar == null || item.BottomRebar.SecondLayer == null ? 0 : item.BottomRebar.SecondLayer.Count;
        }

        private static int GetStirrupLegs(ScheduleItem item)
        {
            return item == null || item.Stirrup == null ? 0 : item.Stirrup.Legs;
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }
    }
}
