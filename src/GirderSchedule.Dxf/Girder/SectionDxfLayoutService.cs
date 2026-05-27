using System;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Dxf.Common;

namespace GirderSchedule.Dxf.Girder
{
    public sealed class SectionDxfLayoutService
    {
        public SectionDxfLayout Create(DxfBox box, ScheduleItem item)
        {
            var width = item.Section.Width <= 0 ? 400.0 : item.Section.Width;
            var height = item.Section.Height <= 0 ? 600.0 : item.Section.Height;
            var scale = 1.0;

            var drawWidth = width;
            var drawHeight = height;

            var centerX = box.CenterX;
            var centerY = box.CenterY - 120.0;

            var sectionLeft = centerX - drawWidth / 2.0;
            var sectionRight = centerX + drawWidth / 2.0;
            var sectionTop = centerY + drawHeight / 2.0;
            var sectionBottom = centerY - drawHeight / 2.0;

            var sideWing = Math.Max(175.0, drawWidth * 0.28);
            var slabUp = 150.0;

            var outerTop = sectionTop + slabUp;
            var outerShelfY = sectionTop;
            var outerBottom = sectionBottom;

            var stirrupCover = 40.0;
            var rebarRadius = 12.5;
            var rebarInset = stirrupCover + rebarRadius;
            var layerGap = Math.Max(45.0, 40.0 * scale);

            return new SectionDxfLayout
            {
                CellLeft = box.Left,
                CellRight = box.Right,
                CellTop = box.Top,
                CellBottom = box.Bottom,

                OuterLeft = sectionLeft,
                OuterRight = sectionRight,
                OuterTop = outerTop,
                OuterShelfY = outerShelfY,
                OuterBottom = outerBottom,

                SectionLeft = sectionLeft,
                SectionRight = sectionRight,
                SectionTop = sectionTop,
                SectionBottom = sectionBottom,

                SideWing = sideWing,

                BarStartX = sectionLeft + rebarInset,
                BarEndX = sectionRight - rebarInset,

                TopBarY1 = outerTop - rebarInset,
                TopBarY2 = outerTop - rebarInset - layerGap,
                BottomBarY1 = outerBottom + rebarInset,
                BottomBarY2 = outerBottom + rebarInset + layerGap,

                DrawWidth = drawWidth,
                DrawHeight = drawHeight,
                Scale = scale
            };
        }
    }
}