using GirderSchedule.Domain.Girder.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GirderSchedule.App.Preview
{
    public sealed class ScheduleExportPreviewDrawer
    {
        private const double CanvasWidth = 1180.0;
        private const double CanvasHeight = 820.0;

        private const double PageX = 30.0;
        private const double PageY = 30.0;
        private const double PageWidth = 1120.0;
        private const double PageHeight = 760.0;

        private const double TitleHeight = 48.0;
        private const double Gap = 0.0;

        private const double CellWidth = 360.0;
        private const double CellHeight = 230.0;

        private const double BarRadius = 2.2;
        private const double StirrupTouchOffset = BarRadius;
        private const double SkinMarkSizeUnit = 25.0;
        private const double SkinMarkWidthUnit = 5.0;

        private readonly Brush _lineBrush = new SolidColorBrush(Color.FromRgb(25, 25, 25));
        private readonly Brush _thinBrush = new SolidColorBrush(Color.FromRgb(120, 120, 120));
        private readonly Brush _textBrush = new SolidColorBrush(Color.FromRgb(30, 30, 30));
        private readonly Brush _barBrush = new SolidColorBrush(Color.FromRgb(70, 70, 70));
        private readonly Brush _disabledBrush = new SolidColorBrush(Color.FromRgb(160, 160, 160));

        public void Draw(Canvas canvas, IList<ScheduleSet> sets, int pageNumber, int pageCount)
        {
            if (canvas == null)
            {
                return;
            }

            canvas.Width = CanvasWidth;
            canvas.Height = CanvasHeight;

            DrawPage(canvas, pageNumber, pageCount);

            if (sets == null || sets.Count == 0)
            {
                DrawText(canvas, "미리보기할 부재가 없습니다. 오른쪽 프로젝트 목록에서 층과 부재를 체크하세요.", 310.0, 390.0, 18.0, _disabledBrush);
                return;
            }

            var count = sets.Count;

            if (count > 9)
            {
                count = 9;
            }

            for (var i = 0; i < count; i++)
            {
                var row = i / 3;
                var col = i % 3;

                var x = PageX + 20.0 + col * CellWidth;
                var y = PageY + TitleHeight + 20.0 + row * CellHeight;

                DrawSetCell(canvas, sets[i], x, y, CellWidth, CellHeight, row, col);
            }
        }

        private void DrawPage(Canvas canvas, int pageNumber, int pageCount)
        {
            DrawRectangle(canvas, PageX, PageY, PageWidth, PageHeight, Brushes.White, _lineBrush, 1.0);
            DrawText(canvas, "보 일람표-" + pageNumber, PageX + 20.0, PageY + 14.0, 20.0, _textBrush);
            DrawText(canvas, pageNumber + " / " + pageCount, PageX + PageWidth - 80.0, PageY + 18.0, 14.0, _thinBrush);
            DrawLine(canvas, PageX, PageY + TitleHeight, PageX + PageWidth, PageY + TitleHeight, _lineBrush, 1.0);
        }

        private void DrawSetCell(Canvas canvas, ScheduleSet set, double x, double y, double width, double height, int row, int col)
        {
            DrawSetCellOuterBorder(canvas, x, y, width, height, row, col);

            var labelWidth = 58.0;
            var contentWidth = width - labelWidth;
            var partWidth = contentWidth / 3.0;

            var nameHeight = 36.0;
            var partHeaderHeight = 20.0;
            var forceHeight = 20.0;
            var sectionHeight = 96.0;

            var sectionTotalHeight = partHeaderHeight + forceHeight + sectionHeight;
            var remainHeight = height - nameHeight - sectionTotalHeight;
            var textRowHeight = remainHeight / 4.0;

            var yName = y;
            var ySectionArea = yName + nameHeight;
            var yForce = ySectionArea + partHeaderHeight;
            var ySection = yForce + forceHeight;
            var yTop = ySectionArea + sectionTotalHeight;
            var yBottom = yTop + textRowHeight;
            var yStirrup = yBottom + textRowHeight;
            var ySkin = yStirrup + textRowHeight;
            var yEnd = y + height;

            var xLabelEnd = x + labelWidth;
            var xLeft = xLabelEnd;
            var xCenter = xLabelEnd + partWidth;
            var xRight = xLabelEnd + partWidth * 2.0;

            DrawLine(canvas, xLabelEnd, y, xLabelEnd, yEnd, _lineBrush, 0.8);

            DrawLine(canvas, x, ySectionArea, x + width, ySectionArea, _lineBrush, 0.8);
            DrawLine(canvas, x, yTop, x + width, yTop, _lineBrush, 0.8);
            DrawLine(canvas, x, yBottom, x + width, yBottom, _lineBrush, 0.8);
            DrawLine(canvas, x, yStirrup, x + width, yStirrup, _lineBrush, 0.8);
            DrawLine(canvas, x, ySkin, x + width, ySkin, _lineBrush, 0.8);

            DrawLine(canvas, xLabelEnd, yForce, x + width, yForce, _lineBrush, 0.8);
            DrawLine(canvas, xLabelEnd, ySection, x + width, ySection, _lineBrush, 0.8);

            DrawLine(canvas, xCenter, ySectionArea, xCenter, yEnd, _thinBrush, 0.6);
            DrawLine(canvas, xRight, ySectionArea, xRight, yEnd, _thinBrush, 0.6);

            DrawCellLabel(canvas, "부재명", x, yName, labelWidth, nameHeight);
            DrawCellLabel(canvas, "단면도", x, ySectionArea, labelWidth, sectionTotalHeight);
            DrawCellLabel(canvas, "상부근", x, yTop, labelWidth, textRowHeight);
            DrawCellLabel(canvas, "하부근", x, yBottom, labelWidth, textRowHeight);
            DrawCellLabel(canvas, "스터럽", x, yStirrup, labelWidth, textRowHeight);
            DrawCellLabel(canvas, "표피근", x, ySkin, labelWidth, textRowHeight);

            var memberName = set == null || string.IsNullOrWhiteSpace(set.MemberName) ? "부재명 없음" : set.MemberName;
            var sizeText = FormatSetSectionSize(set);
            var nameText = string.IsNullOrWhiteSpace(sizeText) ? memberName : memberName + "\r\n" + sizeText;

            DrawCenteredText(canvas, nameText, xLabelEnd, yName, contentWidth, nameHeight, 9.5, _textBrush);

            DrawPartColumn(canvas, "LEFT", set == null ? null : set.Left, xLeft, ySectionArea, partWidth, partHeaderHeight, yForce, forceHeight, ySection, sectionHeight, yTop, yBottom, yStirrup, ySkin, textRowHeight);
            DrawPartColumn(canvas, "CENTER", set == null ? null : set.Center, xCenter, ySectionArea, partWidth, partHeaderHeight, yForce, forceHeight, ySection, sectionHeight, yTop, yBottom, yStirrup, ySkin, textRowHeight);
            DrawPartColumn(canvas, "RIGHT", set == null ? null : set.Right, xRight, ySectionArea, partWidth, partHeaderHeight, yForce, forceHeight, ySection, sectionHeight, yTop, yBottom, yStirrup, ySkin, textRowHeight);
        }

        private void DrawSetCellOuterBorder(Canvas canvas, double x, double y, double width, double height, int row, int col)
        {
            if (row == 0)
            {
                DrawLine(canvas, x, y, x + width, y, _lineBrush, 1.0);
            }

            if (col == 0)
            {
                DrawLine(canvas, x, y, x, y + height, _lineBrush, 1.0);
            }

            DrawLine(canvas, x + width, y, x + width, y + height, _lineBrush, 1.0);
            DrawLine(canvas, x, y + height, x + width, y + height, _lineBrush, 1.0);
        }

        private void DrawPartColumn(Canvas canvas, string title, ScheduleItem item, double x, double yPartHeader, double width, double partHeaderHeight, double yForce, double forceHeight, double ySection, double sectionHeight, double yTop, double yBottom, double yStirrup, double ySkin, double textRowHeight)
        {
            DrawCenteredText(canvas, title, x, yPartHeader, width, partHeaderHeight, 9.0, _thinBrush);

            var isDisabled = item == null || !item.IsSectionEnabled || item.Section == null;

            DrawMemberForce(canvas, item, x, yForce, width, forceHeight, isDisabled);

            if (isDisabled)
            {
                DrawLine(canvas, x + 8.0, ySection + 8.0, x + width - 8.0, ySection + sectionHeight - 8.0, _disabledBrush, 0.9);
                DrawCenteredText(canvas, "미사용", x, ySection, width, sectionHeight, 9.0, _disabledBrush);
                return;
            }

            var drawingX = x + 2.0;
            var drawingY = ySection + 3.0;
            var drawingWidth = width - 4.0;
            var drawingHeight = sectionHeight - 6.0;

            DrawSection(canvas, item, drawingX, drawingY, drawingWidth, drawingHeight);

            DrawCenteredText(canvas, FormatTop(item), x, yTop, width, textRowHeight, 8.5, _textBrush);
            DrawCenteredText(canvas, FormatBottom(item), x, yBottom, width, textRowHeight, 8.5, _textBrush);
            DrawCenteredText(canvas, FormatStirrup(item), x, yStirrup, width, textRowHeight, 8.5, _textBrush);
            DrawCenteredText(canvas, FormatSkinRebar(item), x, ySkin, width, textRowHeight, 8.5, _textBrush);
        }

        private void DrawMemberForce(Canvas canvas, ScheduleItem item, double x, double y, double width, double height, bool isDisabled)
        {
            var middleX = x + width / 2.0;
            var halfWidth = width / 2.0;
            var brush = isDisabled ? _thinBrush : _textBrush;

            DrawLine(canvas, middleX, y, middleX, y + height, _thinBrush, 0.6);

            DrawCenteredText(canvas, FormatMomentForce(item), x, y, halfWidth, height, 8.0, brush);
            DrawCenteredText(canvas, FormatShearForce(item), middleX, y, halfWidth, height, 8.0, brush);
        }

        private string FormatMomentForce(ScheduleItem item)
        {
            if (item == null || item.MemberForce == null)
            {
                return "M=";
            }

            var value = FormatValue(item.MemberForce.Moment);
            return string.IsNullOrWhiteSpace(value) ? "M=" : "M=" + value;
        }

        private string FormatShearForce(ScheduleItem item)
        {
            if (item == null || item.MemberForce == null)
            {
                return "V=";
            }

            var value = FormatValue(item.MemberForce.Shear);
            return string.IsNullOrWhiteSpace(value) ? "V=" : "V=" + value;
        }

        private void DrawCellLabel(Canvas canvas, string text, double x, double y, double width, double height)
        {
            DrawCenteredText(canvas, text, x, y, width, height, 8.5, _thinBrush);
        }

        private void DrawCenteredText(Canvas canvas, string text, double x, double y, double width, double height, double size, Brush brush)
        {
            var block = new TextBlock
            {
                Text = text ?? string.Empty,
                FontSize = size,
                Foreground = brush,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                TextWrapping = TextWrapping.NoWrap,
                Width = width,
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                LineHeight = size + 2.0
            };

            block.Measure(new Size(width, double.PositiveInfinity));

            var top = y + (height - block.DesiredSize.Height) / 2.0;

            Canvas.SetLeft(block, x);
            Canvas.SetTop(block, top);
            canvas.Children.Add(block);
        }

        private string FormatSetSectionSize(ScheduleSet set)
        {
            var item = GetFirstEnabledItem(set);

            if (item == null || item.Section == null)
            {
                return string.Empty;
            }

            return "(" + FormatNumber(item.Section.Width) + " x " + FormatNumber(item.Section.Height) + ")";
        }

        private ScheduleItem GetFirstEnabledItem(ScheduleSet set)
        {
            if (set == null)
            {
                return null;
            }

            if (set.Left != null && set.Left.IsSectionEnabled && set.Left.Section != null)
            {
                return set.Left;
            }

            if (set.Center != null && set.Center.IsSectionEnabled && set.Center.Section != null)
            {
                return set.Center;
            }

            if (set.Right != null && set.Right.IsSectionEnabled && set.Right.Section != null)
            {
                return set.Right;
            }

            return null;
        }

        private string FormatValue(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return string.Format(CultureInfo.InvariantCulture, "{0:0}", value);
        }

        private string FormatSkinRebar(ScheduleItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.SkinRebarText))
            {
                return string.Empty;
            }

            return item.SkinRebarText;
        }

        private void DrawSectionItem(Canvas canvas, string title, ScheduleItem item, double x, double y, double width, double height)
        {
            DrawText(canvas, title, x + 6.0, y, 10.0, _thinBrush);

            if (item == null || !item.IsSectionEnabled || item.Section == null)
            {
                DrawLine(canvas, x + 8.0, y + 24.0, x + width - 14.0, y + height - 8.0, _disabledBrush, 1.0);
                DrawText(canvas, "미사용", x + 28.0, y + height / 2.0, 10.0, _disabledBrush);
                return;
            }

            var drawingX = x + 2.0;
            var drawingY = y + 18.0;
            var drawingWidth = width - 4.0;
            var drawingHeight = height - 76.0;

            if (drawingWidth < 80.0)
            {
                drawingWidth = 80.0;
            }

            if (drawingHeight < 90.0)
            {
                drawingHeight = 90.0;
            }

            DrawSection(canvas, item, drawingX, drawingY, drawingWidth, drawingHeight);

            var textY = y + height - 54.0;
            DrawText(canvas, FormatTop(item), x + 6.0, textY, 9.5, _textBrush);
            DrawText(canvas, FormatBottom(item), x + 6.0, textY + 14.0, 9.5, _textBrush);
            DrawText(canvas, FormatStirrup(item), x + 6.0, textY + 28.0, 9.5, _textBrush);

            var sizeText = FormatNumber(item.Section.Width) + " x " + FormatNumber(item.Section.Height);
            DrawText(canvas, sizeText, x + 6.0, textY + 42.0, 9.5, _thinBrush);
        }

        private void DrawSection(Canvas canvas, ScheduleItem item, double x, double y, double areaWidth, double areaHeight)
        {
            var layout = CreatePreviewLayout(x, y, areaWidth, areaHeight, item.Section.Width, item.Section.Height);

            DrawGirderOutline(canvas, layout);
            DrawMainRebars(canvas, layout, item);
            DrawStirrups(canvas, layout.BarStartX, layout.BarEndX, layout.SectionTop, layout.SectionBottom, GetTopFirstCount(item), GetBottomFirstCount(item), GetStirrupLegs(item), StirrupTouchOffset);
            DrawSkinRebarMarks(canvas, layout, item);
            DrawPreviewDimensions(canvas, layout.OuterLeft, layout.OuterRight, layout.OuterTop, layout.OuterBottom, layout.SideWing, item.Section.Width, item.Section.Height, false, false, layout.DimensionScale);
        }

        private PreviewLayout CreatePreviewLayout(double areaX, double areaY, double areaWidth, double areaHeight, double previewWidth, double previewHeight)
        {
            var widthValue = previewWidth;
            var heightValue = previewHeight;

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

            var scaleX = areaWidth * 0.94 / modelWidth;
            var scaleY = areaHeight * 0.94 / modelHeight;
            var scale = Math.Min(scaleX, scaleY);

            if (scale <= 0)
            {
                scale = 1.0;
            }

            const double shapeScaleFactor = 0.82;

            var shapeScale = scale * shapeScaleFactor;
            var dimensionScale = scale;

            var originX = areaX + (areaWidth - modelWidth * scale) / 2.0;
            var originY = areaY + (areaHeight - modelHeight * scale) / 2.0;

            var topGap = topGapUnit * shapeScale;
            var sideDrop = sideDropUnit * shapeScale;
            var sideWing = sideWingUnit * shapeScale;
            var sideOffset = sideOffsetUnit * shapeScale;
            var bottomOffset = bottomOffsetUnit * shapeScale;
            var rebarCover = Math.Max(2.6, rebarCoverUnit * shapeScale);
            var secondLayerGap = Math.Max(5.5, secondLayerGapUnit * shapeScale);

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
            var outer = new Path
            {
                Stroke = _lineBrush,
                StrokeThickness = 1.1,
                Fill = Brushes.Transparent,
                Data = CreateGirderOuterGeometry(layout.OuterLeft, layout.OuterRight, layout.OuterTop, layout.OuterShelfY, layout.OuterBottom, layout.SideWing)
            };

            canvas.Children.Add(outer);

            var stirrup = new Rectangle
            {
                Width = layout.DrawWidth,
                Height = layout.DrawHeight,
                Stroke = _lineBrush,
                StrokeThickness = 0.8,
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
            DrawFirstLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.TopBarY1, GetTopFirstCount(item));
            DrawSecondLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.TopBarY2, GetTopFirstCount(item), GetTopSecondCount(item));

            DrawFirstLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.BottomBarY1, GetBottomFirstCount(item));
            DrawSecondLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.BottomBarY2, GetBottomFirstCount(item), GetBottomSecondCount(item));
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
                Fill = _barBrush
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

        private void AddStirrupLine(Canvas canvas, double topX, double bottomX, double topY, double bottomY)
        {
            var line = new Line
            {
                X1 = topX,
                Y1 = topY,
                X2 = bottomX,
                Y2 = bottomY,
                Stroke = _lineBrush,
                StrokeThickness = 0.8
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

        private void DrawSkinRebarMarks(Canvas canvas, PreviewLayout layout, ScheduleItem item)
        {
            if (item == null || item.Section == null || item.Section.Height < 900.0)
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

            DrawLine(canvas, x - half, y - half, x + half, y + half, _lineBrush, thickness);
            DrawLine(canvas, x - half, y + half, x + half, y - half, _lineBrush, thickness);
        }

        private void DrawPreviewDimensions(Canvas canvas, double outerLeft, double outerRight, double outerTop, double outerBottom, double sideWing, double widthValue, double heightValue, bool isWidthOver, bool isHeightOver, double scale)
        {
            const double auxGap = 12.0;

            var dimensionGap = Math.Max(12.0, 34.0 * scale);
            var extensionOver = Math.Max(4.0, 12.0 * scale);
            var tick = Math.Max(2.5, 8.0 * scale);
            var circleRadius = Math.Max(1.5, 3.0 * scale);
            var textFontSize = Math.Max(7.5, 16.0 * scale);

            var widthText = FormatDimensionText(widthValue, isWidthOver);
            var heightText = FormatDimensionText(heightValue, isHeightOver);
            var outerVisualLeft = outerLeft - sideWing;

            var topDimY = outerTop - dimensionGap;
            var topExtTop = topDimY - extensionOver;
            var topExtBottom = outerTop - auxGap * scale;

            DrawLine(canvas, outerLeft, topDimY, outerRight, topDimY, _lineBrush, 0.75);
            DrawLine(canvas, outerLeft, topExtTop, outerLeft, topExtBottom, _lineBrush, 0.75);
            DrawLine(canvas, outerRight, topExtTop, outerRight, topExtBottom, _lineBrush, 0.75);

            DrawDimensionMark(canvas, outerLeft, topDimY, circleRadius, tick, _lineBrush);
            DrawDimensionMark(canvas, outerRight, topDimY, circleRadius, tick, _lineBrush);
            DrawDimensionText(canvas, outerLeft + (outerRight - outerLeft) / 2.0, topDimY - textFontSize * 0.9, widthText, textFontSize, 0.0, _lineBrush);

            var leftDimX = outerVisualLeft - dimensionGap;
            var leftExtLeft = leftDimX - extensionOver;
            var leftExtRight = outerVisualLeft - auxGap * scale;

            DrawLine(canvas, leftDimX, outerTop, leftDimX, outerBottom, _lineBrush, 0.75);
            DrawLine(canvas, leftExtLeft, outerTop, leftExtRight, outerTop, _lineBrush, 0.75);
            DrawLine(canvas, leftExtLeft, outerBottom, leftExtRight, outerBottom, _lineBrush, 0.75);

            DrawDimensionMark(canvas, leftDimX, outerTop, circleRadius, tick, _lineBrush);
            DrawDimensionMark(canvas, leftDimX, outerBottom, circleRadius, tick, _lineBrush);
            DrawDimensionText(canvas, leftDimX - textFontSize * 0.85, outerTop + (outerBottom - outerTop) / 2.0, heightText, textFontSize, -90.0, _lineBrush);
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

        private void DrawDimensionMark(Canvas canvas, double x, double y, double radius, double tick, Brush stroke)
        {
            var circle = new Ellipse
            {
                Width = radius * 2.0,
                Height = radius * 2.0,
                Stroke = stroke,
                StrokeThickness = 0.7,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(circle, x - radius);
            Canvas.SetTop(circle, y - radius);
            canvas.Children.Add(circle);

            DrawLine(canvas, x - tick, y, x + tick, y, stroke, 0.7);
            DrawLine(canvas, x, y - tick, x, y + tick, stroke, 0.7);
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

        private string FormatTop(ScheduleItem item)
        {
            if (item == null || item.TopRebar == null)
            {
                return string.Empty;
            }

            var count = GetTopFirstCount(item) + GetTopSecondCount(item);
            return FormatRebar(count, item.TopRebar.Diameter);
        }

        private string FormatBottom(ScheduleItem item)
        {
            if (item == null || item.BottomRebar == null)
            {
                return string.Empty;
            }

            var count = GetBottomFirstCount(item) + GetBottomSecondCount(item);
            return FormatRebar(count, item.BottomRebar.Diameter);
        }

        private string FormatStirrup(ScheduleItem item)
        {
            if (item == null || item.Stirrup == null)
            {
                return string.Empty;
            }

            if (item.Stirrup.Legs <= 0 || item.Stirrup.Diameter <= 0 || item.Stirrup.Spacing <= 0)
            {
                return string.Empty;
            }

            return item.Stirrup.Legs + "-HD" + FormatNumber(item.Stirrup.Diameter) + "@" + FormatNumber(item.Stirrup.Spacing);
        }

        private string FormatRebar(int count, double diameter)
        {
            if (count <= 0 || diameter <= 0)
            {
                return string.Empty;
            }

            return count + "-HD" + FormatNumber(diameter);
        }

        private int GetTopFirstCount(ScheduleItem item)
        {
            return item == null || item.TopRebar == null || item.TopRebar.FirstLayer == null ? 0 : item.TopRebar.FirstLayer.Count;
        }

        private int GetTopSecondCount(ScheduleItem item)
        {
            return item == null || item.TopRebar == null || item.TopRebar.SecondLayer == null ? 0 : item.TopRebar.SecondLayer.Count;
        }

        private int GetBottomFirstCount(ScheduleItem item)
        {
            return item == null || item.BottomRebar == null || item.BottomRebar.FirstLayer == null ? 0 : item.BottomRebar.FirstLayer.Count;
        }

        private int GetBottomSecondCount(ScheduleItem item)
        {
            return item == null || item.BottomRebar == null || item.BottomRebar.SecondLayer == null ? 0 : item.BottomRebar.SecondLayer.Count;
        }

        private int GetStirrupLegs(ScheduleItem item)
        {
            return item == null || item.Stirrup == null ? 0 : item.Stirrup.Legs;
        }

        private string FormatNumber(double value)
        {
            return value.ToString("0", CultureInfo.InvariantCulture);
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

        private void DrawRectangle(Canvas canvas, double x, double y, double width, double height, Brush fill, Brush stroke, double thickness)
        {
            var rect = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = fill,
                Stroke = stroke,
                StrokeThickness = thickness
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            canvas.Children.Add(rect);
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

        private void DrawText(Canvas canvas, string text, double x, double y, double size, Brush brush)
        {
            var block = new TextBlock
            {
                Text = text ?? string.Empty,
                FontSize = size,
                Foreground = brush
            };

            Canvas.SetLeft(block, x);
            Canvas.SetTop(block, y);
            canvas.Children.Add(block);
        }

        private sealed class PreviewLayout
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