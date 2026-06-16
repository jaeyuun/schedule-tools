using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GirderSchedule.App.ViewModels;
using GirderSchedule.Domain.Girder.Models;

namespace GirderSchedule.App.Views.Controls
{
    public partial class SchedulePreview : UserControl
    {
        private const double BarRadius = 3.0;
        private const double StirrupTouchOffset = BarRadius;
        private const double SkinMarkSizeUnit = 25.0;
        private const double SkinMarkWidthUnit = 5.0;

        private bool _isRedrawQueued;

        public SchedulePreview()
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
            var vm = DataContext as ScheduleViewModel;

            if (vm == null)
            {
                return;
            }

            DrawSection(LeftCanvas, vm.Left, vm.IsWidthOver, vm.IsHeightOver);
            DrawSection(CenterCanvas, vm.Center, vm.IsWidthOver, vm.IsHeightOver);
            DrawSection(RightCanvas, vm.Right, vm.IsWidthOver, vm.IsHeightOver);
        }

        private void DrawSection(Canvas canvas, SectionItemViewModel column, bool isWidthOver, bool isHeightOver)
        {
            if (canvas == null)
            {
                return;
            }

            canvas.Children.Clear();

            if (column == null || !column.IsSectionEnabled)
            {
                return;
            }

            var item = column.Model;
            var layout = CreatePreviewLayout(canvas, item.Section.Width, item.Section.Height);

            DrawGirderOutline(canvas, layout);
            DrawMainRebars(canvas, layout, item);
            DrawStirrups(canvas, layout.BarStartX, layout.BarEndX, layout.SectionTop, layout.SectionBottom, item.TopRebar.FirstLayer.Count, item.BottomRebar.FirstLayer.Count, item.Stirrup.Legs, StirrupTouchOffset);
            DrawSkinRebarMarks(canvas, layout, item);
            DrawPreviewDimensions(canvas, layout.OuterLeft, layout.OuterRight, layout.OuterTop, layout.OuterBottom, layout.SideWing, item.Section.Width, item.Section.Height, isWidthOver, isHeightOver, layout.DimensionScale);
        }

        private PreviewLayout CreatePreviewLayout(Canvas canvas, double previewWidth, double previewHeight)
        {
            var widthValue = previewWidth;
            var heightValue = previewHeight;

            var areaWidth = canvas.ActualWidth;
            var areaHeight = canvas.ActualHeight;

            if (areaWidth <= 0 || areaHeight <= 0)
            {
                areaWidth = canvas.Width > 0 ? canvas.Width : 200.0;
                areaHeight = canvas.Height > 0 ? canvas.Height : 240.0;
            }

            var sectionUnitWidth = widthValue > 0 ? widthValue : 600.0;
            var sectionUnitHeight = heightValue > 0 ? heightValue : 900.0;

            const double topGapUnit = 30.0;
            const double sideDropUnit = 150.0;
            const double sideWingUnit = 175.0;
            const double sideOffsetUnit = 25.0;
            const double bottomOffsetUnit = 30.0;
            const double rebarCoverUnit = 25.0;
            const double secondLayerGapUnit = 40.0;

            const double dimensionTopSpaceUnit = 95.0;
            const double dimensionLeftSpaceUnit = 95.0;
            const double dimensionRightSpaceUnit = 35.0;
            const double dimensionBottomSpaceUnit = 25.0;

            var outerUnitWidth = sectionUnitWidth + sideOffsetUnit * 2.0;
            var outerUnitHeight = topGapUnit + sectionUnitHeight + bottomOffsetUnit;

            var modelWidth = dimensionLeftSpaceUnit + sideWingUnit + outerUnitWidth + sideWingUnit + dimensionRightSpaceUnit;
            var modelHeight = dimensionTopSpaceUnit + outerUnitHeight + dimensionBottomSpaceUnit;

            var scaleX = areaWidth * 0.9 / modelWidth;
            var scaleY = areaHeight * 0.9 / modelHeight;
            var scale = Math.Min(scaleX, scaleY);

            if (scale <= 0)
            {
                scale = 1.0;
            }

            const double shapeScaleFactor = 0.82;

            var shapeScale = scale * shapeScaleFactor;
            var dimensionScale = scale;

            var originX = (areaWidth - modelWidth * scale) / 2.0;
            var originY = (areaHeight - modelHeight * scale) / 2.0;

            var topGap = topGapUnit * shapeScale;
            var sideDrop = sideDropUnit * shapeScale;
            var sideWing = sideWingUnit * shapeScale;
            var sideOffset = sideOffsetUnit * shapeScale;
            var bottomOffset = bottomOffsetUnit * shapeScale;
            var rebarCover = Math.Max(4.0, rebarCoverUnit * shapeScale);
            var secondLayerGap = Math.Max(7.0, secondLayerGapUnit * shapeScale);

            var drawWidth = sectionUnitWidth * shapeScale;
            var drawHeight = sectionUnitHeight * shapeScale;

            var dimensionLeftSpace = dimensionLeftSpaceUnit * dimensionScale;
            var dimensionRightSpace = dimensionRightSpaceUnit * dimensionScale;
            var dimensionTopSpace = dimensionTopSpaceUnit * dimensionScale;
            var dimensionBottomSpace = dimensionBottomSpaceUnit * dimensionScale;

            var shapeModelWidth = sideWingUnit * shapeScale + sideOffsetUnit * shapeScale + sectionUnitWidth * shapeScale + sideOffsetUnit * shapeScale + sideWingUnit * shapeScale;
            var shapeModelHeight = topGapUnit * shapeScale + sectionUnitHeight * shapeScale + bottomOffsetUnit * shapeScale;

            var availableShapeWidth = modelWidth * scale - dimensionLeftSpace - dimensionRightSpace;
            var availableShapeHeight = modelHeight * scale - dimensionTopSpace - dimensionBottomSpace;

            var shapeStartX = originX + dimensionLeftSpace + (availableShapeWidth - shapeModelWidth) / 2.0;
            var shapeStartY = originY + dimensionTopSpace + (availableShapeHeight - shapeModelHeight) / 2.0;

            var outerLeft = shapeStartX + sideWing;
            var outerTop = shapeStartY;

            var outerRight = outerLeft + drawWidth + sideOffset * 2.0;
            var outerShelfY = outerTop + sideDrop;
            var outerBottom = outerTop + topGap + drawHeight + bottomOffset;

            var sectionLeft = outerLeft + sideOffset;
            var sectionTop = outerTop + topGap;
            var sectionBottom = sectionTop + drawHeight;

            var barStartX = sectionLeft + rebarCover;
            var barEndX = sectionLeft + drawWidth - rebarCover;

            var topBarY1 = sectionTop + rebarCover;
            var topBarY2 = topBarY1 + secondLayerGap;

            var bottomBarY1 = sectionTop + drawHeight - rebarCover;
            var bottomBarY2 = bottomBarY1 - secondLayerGap;

            return new PreviewLayout
            {
                AreaWidth = areaWidth,
                AreaHeight = areaHeight,
                ShapeScale = shapeScale,
                DimensionScale = dimensionScale,
                OuterLeft = outerLeft,
                OuterRight = outerRight,
                OuterTop = outerTop,
                OuterShelfY = outerShelfY,
                OuterBottom = outerBottom,
                SectionLeft = sectionLeft,
                SectionTop = sectionTop,
                SectionBottom = sectionBottom,
                DrawWidth = drawWidth,
                DrawHeight = drawHeight,
                SideWing = sideWing,
                RebarCover = rebarCover,
                SecondLayerGap = secondLayerGap,
                BarStartX = barStartX,
                BarEndX = barEndX,
                TopBarY1 = topBarY1,
                TopBarY2 = topBarY2,
                BottomBarY1 = bottomBarY1,
                BottomBarY2 = bottomBarY2
            };
        }

        private void DrawGirderOutline(Canvas canvas, PreviewLayout layout)
        {
            var lineBrush = new SolidColorBrush(Color.FromRgb(25, 25, 25));

            var outer = new Path
            {
                Stroke = lineBrush,
                StrokeThickness = 1.5,
                Fill = Brushes.Transparent,
                Data = CreateGirderOuterGeometry(layout.OuterLeft, layout.OuterRight, layout.OuterTop, layout.OuterShelfY, layout.OuterBottom, layout.SideWing)
            };

            canvas.Children.Add(outer);

            var stirrup = new Rectangle
            {
                Width = layout.DrawWidth,
                Height = layout.DrawHeight,
                Stroke = lineBrush,
                StrokeThickness = 1.0,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(stirrup, layout.SectionLeft);
            Canvas.SetTop(stirrup, layout.SectionTop);
            canvas.Children.Add(stirrup);
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

        private void DrawMainRebars(Canvas canvas, PreviewLayout layout, ScheduleItem item)
        {
            DrawFirstLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.TopBarY1, item.TopRebar.FirstLayer.Count);
            DrawSecondLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.TopBarY2, item.TopRebar.FirstLayer.Count, item.TopRebar.SecondLayer.Count);

            DrawFirstLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.BottomBarY1, item.BottomRebar.FirstLayer.Count);
            DrawSecondLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.BottomBarY2, item.BottomRebar.FirstLayer.Count, item.BottomRebar.SecondLayer.Count);
        }

        private void DrawFirstLayerRebars(Canvas canvas, double startX, double endX, double y, int count)
        {
            if (count <= 0)
            {
                return;
            }

            if (count == 1)
            {
                AddBar(canvas, startX, y);
                return;
            }

            var spacing = (endX - startX) / (count - 1);

            for (var i = 0; i < count; i++)
            {
                AddBar(canvas, startX + spacing * i, y);
            }
        }

        private void DrawSecondLayerRebars(Canvas canvas, double startX, double endX, double y, int firstLayerCount, int secondLayerCount)
        {
            if (firstLayerCount <= 0 || secondLayerCount <= 0)
            {
                return;
            }

            if (firstLayerCount == 1)
            {
                AddBar(canvas, startX, y);
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

                AddBar(canvas, startX + spacing * i, y);
            }
        }

        private void AddBar(Canvas canvas, double x, double y)
        {
            var ellipse = new Ellipse
            {
                Width = BarRadius * 2.0,
                Height = BarRadius * 2.0,
                Fill = new SolidColorBrush(Color.FromRgb(70, 70, 70))
            };

            Canvas.SetLeft(ellipse, x - BarRadius);
            Canvas.SetTop(ellipse, y - BarRadius);
            canvas.Children.Add(ellipse);
        }

        private void DrawStirrups(Canvas canvas, double barStartX, double barEndX, double topY, double bottomY, int topFirstLayerCount, int bottomFirstLayerCount, int stirrupLegs, double sideOffset)
        {
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

            var topXs = GetLayerRebarXs(barStartX, barEndX, topFirstLayerCount);
            var bottomXs = GetLayerRebarXs(barStartX, barEndX, bottomFirstLayerCount);
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
                var topLineX = topX + direction * sideOffset;
                var bottomLineX = bottomX + direction * sideOffset;

                topLineX = Clamp(topLineX, barStartX, barEndX);
                bottomLineX = Clamp(bottomLineX, barStartX, barEndX);

                AddStirrupLine(canvas, topLineX, bottomLineX, topY, bottomY);
            }
        }

        private void DrawSkinRebarMarks(Canvas canvas, PreviewLayout layout, ScheduleItem item)
        {
            if (item.Section.Height < 900.0)
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
            var brush = new SolidColorBrush(Color.FromRgb(25, 25, 25));

            DrawLine(canvas, x - half, y - half, x + half, y + half, brush, thickness);
            DrawLine(canvas, x - half, y + half, x + half, y - half, brush, thickness);
        }

        private void AddStirrupLine(Canvas canvas, double topX, double bottomX, double topY, double bottomY)
        {
            var line = new Line
            {
                X1 = topX,
                Y1 = topY,
                X2 = bottomX,
                Y2 = bottomY,
                Stroke = new SolidColorBrush(Color.FromRgb(25, 25, 25)),
                StrokeThickness = 1.0
            };

            canvas.Children.Add(line);
        }

        private double[] GetLayerRebarXs(double startX, double endX, int count)
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

        private void DrawPreviewDimensions(Canvas canvas, double outerLeft, double outerRight, double outerTop, double outerBottom, double sideWing, double widthValue, double heightValue, bool isWidthOver, bool isHeightOver, double scale)
        {
            var dimensionBrush = new SolidColorBrush(Color.FromRgb(25, 25, 25));

            const double auxGap = 12.0;

            var dimensionGap = Math.Max(24.0, 34.0 * scale);
            var extensionOver = Math.Max(8.0, 12.0 * scale);
            var tick = Math.Max(4.0, 8.0 * scale);
            var circleRadius = Math.Max(2.0, 3.0 * scale);
            var textFontSize = Math.Max(10.0, 16.0 * scale);

            var widthText = FormatDimensionText(widthValue, isWidthOver);
            var heightText = FormatDimensionText(heightValue, isHeightOver);
            var outerVisualLeft = outerLeft - sideWing;

            var topDimY = outerTop - dimensionGap;
            var topExtTop = topDimY - extensionOver;
            var topExtBottom = outerTop - auxGap;

            DrawLine(canvas, outerLeft, topDimY, outerRight, topDimY, dimensionBrush, 1.0);
            DrawLine(canvas, outerLeft, topExtTop, outerLeft, topExtBottom, dimensionBrush, 1.0);
            DrawLine(canvas, outerRight, topExtTop, outerRight, topExtBottom, dimensionBrush, 1.0);

            DrawDimensionMark(canvas, outerLeft, topDimY, circleRadius, tick, dimensionBrush);
            DrawDimensionMark(canvas, outerRight, topDimY, circleRadius, tick, dimensionBrush);

            DrawDimensionText(canvas, outerLeft + (outerRight - outerLeft) / 2.0, topDimY - textFontSize * 0.9, widthText, textFontSize, 0.0, dimensionBrush);

            var leftDimX = outerVisualLeft - dimensionGap;
            var leftExtLeft = leftDimX - extensionOver;
            var leftExtRight = outerVisualLeft - auxGap;

            DrawLine(canvas, leftDimX, outerTop, leftDimX, outerBottom, dimensionBrush, 1.0);
            DrawLine(canvas, leftExtLeft, outerTop, leftExtRight, outerTop, dimensionBrush, 1.0);
            DrawLine(canvas, leftExtLeft, outerBottom, leftExtRight, outerBottom, dimensionBrush, 1.0);

            DrawDimensionMark(canvas, leftDimX, outerTop, circleRadius, tick, dimensionBrush);
            DrawDimensionMark(canvas, leftDimX, outerBottom, circleRadius, tick, dimensionBrush);

            DrawDimensionText(canvas, leftDimX - textFontSize * 0.9, outerTop + (outerBottom - outerTop) / 2.0, heightText, textFontSize, -90.0, dimensionBrush);
        }

        private string FormatDimensionText(double value, bool isOver)
        {
            var text = value.ToString(CultureInfo.InvariantCulture);

            if (!isOver)
            {
                return text;
            }

            return text + " 이상";
        }

        private void DrawLine(Canvas canvas, double x1, double y1, double x2, double y2, Brush stroke, double thickness)
        {
            var line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = stroke,
                StrokeThickness = thickness
            };

            canvas.Children.Add(line);
        }

        private void DrawDimensionMark(Canvas canvas, double x, double y, double radius, double tick, Brush stroke)
        {
            var circle = new Ellipse
            {
                Width = radius * 2.0,
                Height = radius * 2.0,
                Stroke = stroke,
                StrokeThickness = 1.0,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(circle, x - radius);
            Canvas.SetTop(circle, y - radius);
            canvas.Children.Add(circle);

            DrawLine(canvas, x - tick, y, x + tick, y, stroke, 1.0);
            DrawLine(canvas, x, y - tick, x, y + tick, stroke, 1.0);
        }

        private void DrawDimensionText(Canvas canvas, double centerX, double centerY, string text, double fontSize, double angle, Brush foreground)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                Foreground = foreground,
                FontWeight = FontWeights.SemiBold,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            Canvas.SetLeft(textBlock, centerX - textBlock.DesiredSize.Width / 2.0);
            Canvas.SetTop(textBlock, centerY - textBlock.DesiredSize.Height / 2.0);

            if (Math.Abs(angle) > 0.001)
            {
                textBlock.RenderTransform = new RotateTransform(angle);
            }

            canvas.Children.Add(textBlock);
        }

        private class PreviewLayout
        {
            public double AreaWidth { get; set; }
            public double AreaHeight { get; set; }

            public double ShapeScale { get; set; }
            public double DimensionScale { get; set; }

            public double OuterLeft { get; set; }
            public double OuterRight { get; set; }
            public double OuterTop { get; set; }
            public double OuterShelfY { get; set; }
            public double OuterBottom { get; set; }

            public double SectionLeft { get; set; }
            public double SectionTop { get; set; }
            public double SectionBottom { get; set; }

            public double DrawWidth { get; set; }
            public double DrawHeight { get; set; }

            public double SideWing { get; set; }
            public double RebarCover { get; set; }
            public double SecondLayerGap { get; set; }

            public double BarStartX { get; set; }
            public double BarEndX { get; set; }

            public double TopBarY1 { get; set; }
            public double TopBarY2 { get; set; }
            public double BottomBarY1 { get; set; }
            public double BottomBarY2 { get; set; }
        }
    }
}