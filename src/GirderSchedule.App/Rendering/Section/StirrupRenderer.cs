using GirderSchedule.App.Rendering.Models;
using GirderSchedule.Domain.Layouts;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace GirderSchedule.App.Rendering.Section
{
    public sealed class StirrupRenderer
    {
        private readonly CanvasDrawer _drawer;
        private readonly ScheduleItemQuery _query;
        private readonly RebarPointLayoutService _rebarLayoutService;

        public StirrupRenderer()
            : this(new CanvasDrawer(), new ScheduleItemQuery(), new RebarPointLayoutService())
        {
        }

        public StirrupRenderer(CanvasDrawer drawer, ScheduleItemQuery query, RebarPointLayoutService rebarLayoutService)
        {
            _drawer = drawer;
            _query = query;
            _rebarLayoutService = rebarLayoutService;
        }


        public void Draw(Canvas canvas, PreviewSectionLayout layout, ScheduleItem item, PreviewSectionRenderOptions options, Brush lineBrush)
        {
            if (canvas == null || layout == null || item == null || options == null)
            {
                return;
            }

            var topFirstLayerCount = _query.GetTopFirstCount(item);
            var bottomFirstLayerCount = _query.GetBottomFirstCount(item);
            var stirrupLegs = _query.GetStirrupLegs(item);

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

            var topXs = _rebarLayoutService.GetFirstLayerXs(layout.BarStartX, layout.BarEndX, topFirstLayerCount);
            var bottomXs = _rebarLayoutService.GetFirstLayerXs(layout.BarStartX, layout.BarEndX, bottomFirstLayerCount);
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

                _drawer.DrawLine(canvas, topLineX, layout.SectionTop, bottomLineX, layout.SectionBottom, lineBrush, options.StirrupLineThickness);
            }
        }

        private static int FindNearestIndex(IList<double> values, double targetX, bool[] used)
        {
            var index = -1;
            var distance = double.MaxValue;

            for (var i = 0; i < values.Count; i++)
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
    }
}