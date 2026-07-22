using GirderSchedule.App.Rendering.Models;
using GirderSchedule.Domain.Models;
using ScheduleTools.Wpf.Rendering;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace GirderSchedule.App.Rendering.Section
{
    public sealed class SkinRebarRenderer
    {
        private const double SkinRebarApplyHeight = 900.0;
        private const double SkinMarkSizeUnit = 25.0;
        private const double SkinMarkWidthUnit = 5.0;

        private readonly CanvasDrawer _drawer;

        public SkinRebarRenderer()
            : this(new CanvasDrawer())
        {
        }

        public SkinRebarRenderer(CanvasDrawer drawer)
        {
            _drawer = drawer;
        }

        public void Draw(Canvas canvas, PreviewSectionLayout layout, ScheduleItem item, Brush lineBrush)
        {
            if (canvas == null || layout == null || item == null || item.Section == null)
            {
                return;
            }

            if (item.Section.Height <= SkinRebarApplyHeight)
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

            AddXMark(canvas, stirrupLeft + inset, centerY, size, width, lineBrush);
            AddXMark(canvas, stirrupRight - inset, centerY, size, width, lineBrush);
        }

        private void AddXMark(Canvas canvas, double x, double y, double size, double thickness, Brush lineBrush)
        {
            var half = size / 2.0;

            _drawer.DrawLine(canvas, x - half, y - half, x + half, y + half, lineBrush, thickness);
            _drawer.DrawLine(canvas, x - half, y + half, x + half, y - half, lineBrush, thickness);
        }
    }
}
