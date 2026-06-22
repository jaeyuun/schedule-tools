using GirderSchedule.Domain.Models;
using GirderSchedule.Dxf.Geometry;

namespace GirderSchedule.Dxf.Layouts
{
    public sealed class SectionLayoutFactory
    {
        private const double DefaultWidth = 400.0;
        private const double DefaultHeight = 600.0;
        private const double SectionCenterOffsetY = -120.0;
        private const double MinimumSideWing = 175.0;
        private const double SideWingRatio = 0.28;
        private const double SlabUp = 150.0;
        private const double MaximumSlabUpRatio = 0.4;

        public SectionLayout Create(DxfBox box, ScheduleItem item)
        {
            if (box == null)
            {
                return null;
            }

            var width = GetSectionWidth(item);
            var height = GetSectionHeight(item);
            var centerX = box.CenterX;
            var centerY = box.CenterY + SectionCenterOffsetY;
            var sideWing = Math.Max(MinimumSideWing, width * SideWingRatio);
            var slabUp = Math.Min(SlabUp, height * MaximumSlabUpRatio);
            var outerLeft = centerX - width / 2.0;
            var outerRight = centerX + width / 2.0;
            var outerTop = centerY + height / 2.0;
            var outerBottom = centerY - height / 2.0;
            var sectionTop = outerTop - slabUp;
            var rebarInset = SectionRebarLayout.StirrupCover + SectionRebarLayout.RebarRadius;

            return new SectionLayout
            {
                CellLeft = box.Left,
                CellRight = box.Right,
                CellTop = box.Top,
                CellBottom = box.Bottom,

                OuterLeft = outerLeft,
                OuterRight = outerRight,
                OuterTop = outerTop,
                OuterShelfY = sectionTop,
                OuterBottom = outerBottom,

                SectionLeft = outerLeft,
                SectionRight = outerRight,
                SectionTop = sectionTop,
                SectionBottom = outerBottom,

                SideWing = sideWing,

                BarStartX = outerLeft + rebarInset,
                BarEndX = outerRight - rebarInset,

                TopBarY1 = outerTop - rebarInset,
                TopBarY2 = outerTop - rebarInset - SectionRebarLayout.LayerGap,
                BottomBarY1 = outerBottom + rebarInset,
                BottomBarY2 = outerBottom + rebarInset + SectionRebarLayout.LayerGap,

                DrawWidth = width,
                DrawHeight = height,
                Scale = 1.0
            };
        }

        private double GetSectionWidth(ScheduleItem item)
        {
            if (item == null || item.Section == null || item.Section.Width <= 0.0)
            {
                return DefaultWidth;
            }

            return item.Section.Width;
        }

        private double GetSectionHeight(ScheduleItem item)
        {
            if (item == null || item.Section == null || item.Section.Height <= 0.0)
            {
                return DefaultHeight;
            }

            return item.Section.Height;
        }
    }
}
