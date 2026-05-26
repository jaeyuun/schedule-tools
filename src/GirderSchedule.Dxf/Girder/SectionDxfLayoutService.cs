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

            var maxWidth = 950.0;
            var maxHeight = 1150.0;
            var scale = Math.Min(maxWidth / width, maxHeight / height);

            if (scale <= 0.0)
            {
                scale = 1.0;
            }

            var drawWidth = width * scale;
            var drawHeight = height * scale;

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

            var cover = Math.Max(40.0, 25.0 * scale);
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

                BarStartX = sectionLeft + cover,
                BarEndX = sectionRight - cover,

                TopBarY1 = sectionTop - cover,
                TopBarY2 = sectionTop - cover - layerGap,
                BottomBarY1 = sectionBottom + cover,
                BottomBarY2 = sectionBottom + cover + layerGap,

                DrawWidth = drawWidth,
                DrawHeight = drawHeight,
                Scale = scale
            };
        }
    }
}