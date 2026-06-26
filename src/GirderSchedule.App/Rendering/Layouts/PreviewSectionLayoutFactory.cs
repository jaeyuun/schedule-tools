using System;
using GirderSchedule.App.Rendering.Models;

namespace GirderSchedule.App.Rendering.Layouts
{
    public sealed class PreviewSectionLayoutFactory
    {
        private const double DefaultSectionWidth = 600.0;
        private const double DefaultSectionHeight = 900.0;

        private const double TopGapUnit = 30.0;
        private const double SideDropUnit = 150.0;
        private const double SideWingUnit = 175.0;
        private const double SideOffsetUnit = 25.0;
        private const double BottomOffsetUnit = 30.0;
        private const double RebarCoverUnit = 25.0;
        private const double SecondLayerGapUnit = 40.0;

        private const double DimensionTopSpaceUnit = 95.0;
        private const double DimensionLeftSpaceUnit = 95.0;
        private const double DimensionRightSpaceUnit = 35.0;
        private const double DimensionBottomSpaceUnit = 25.0;

        private const double ShapeScaleFactor = 0.82;

        public PreviewSectionLayout Create(double areaX, double areaY, double areaWidth, double areaHeight, double sectionWidth, double sectionHeight, PreviewSectionRenderOptions options)
        {
            var widthValue = sectionWidth > 0.0 ? sectionWidth : DefaultSectionWidth;
            var heightValue = sectionHeight > 0.0 ? sectionHeight : DefaultSectionHeight;
            var renderOptions = options ?? PreviewSectionRenderOptions.Editor;

            if (areaWidth <= 0.0)
            {
                areaWidth = 200.0;
            }

            if (areaHeight <= 0.0)
            {
                areaHeight = 240.0;
            }

            var outerUnitWidth = widthValue + SideOffsetUnit * 2.0;
            var outerUnitHeight = TopGapUnit + heightValue + BottomOffsetUnit;

            var modelWidth = DimensionLeftSpaceUnit + SideWingUnit + outerUnitWidth + SideWingUnit + DimensionRightSpaceUnit;
            var modelHeight = DimensionTopSpaceUnit + outerUnitHeight + DimensionBottomSpaceUnit;

            var scaleX = areaWidth * renderOptions.ScaleMarginRatio / modelWidth;
            var scaleY = areaHeight * renderOptions.ScaleMarginRatio / modelHeight;
            var scale = Math.Min(scaleX, scaleY);

            if (scale <= 0.0)
            {
                scale = 1.0;
            }

            var shapeScale = scale * ShapeScaleFactor;
            var dimensionScale = scale;

            var originX = areaX + (areaWidth - modelWidth * scale) / 2.0;
            var originY = areaY + (areaHeight - modelHeight * scale) / 2.0;

            var topGap = TopGapUnit * shapeScale;
            var sideDrop = SideDropUnit * shapeScale;
            var sideWing = SideWingUnit * shapeScale;
            var sideOffset = SideOffsetUnit * shapeScale;
            var bottomOffset = BottomOffsetUnit * shapeScale;
            var rebarCover = Math.Max(renderOptions.MinimumRebarCover, RebarCoverUnit * shapeScale);
            var secondLayerGap = Math.Max(renderOptions.MinimumSecondLayerGap, SecondLayerGapUnit * shapeScale);

            var drawWidth = widthValue * shapeScale;
            var drawHeight = heightValue * shapeScale;

            var dimensionLeftSpace = DimensionLeftSpaceUnit * dimensionScale;
            var dimensionRightSpace = DimensionRightSpaceUnit * dimensionScale;
            var dimensionTopSpace = DimensionTopSpaceUnit * dimensionScale;
            var dimensionBottomSpace = DimensionBottomSpaceUnit * dimensionScale;

            var shapeModelWidth = SideWingUnit * shapeScale + SideOffsetUnit * shapeScale + widthValue * shapeScale + SideOffsetUnit * shapeScale + SideWingUnit * shapeScale;
            var shapeModelHeight = TopGapUnit * shapeScale + heightValue * shapeScale + BottomOffsetUnit * shapeScale;

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

            return new PreviewSectionLayout
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
    }
}
