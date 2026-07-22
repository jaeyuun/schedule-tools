using GirderSchedule.App.Rendering.Models;
using GirderSchedule.Domain.Layouts;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;
using ScheduleTools.Wpf.Rendering;
using System.Windows.Controls;
using System.Windows.Media;

namespace GirderSchedule.App.Rendering.Section
{
    public sealed class MainRebarRenderer
    {
        private readonly CanvasDrawer _drawer;
        private readonly ScheduleItemQuery _query;
        private readonly RebarPointLayoutService _rebarLayoutService;

        public MainRebarRenderer()
            : this(new CanvasDrawer(), new ScheduleItemQuery(), new RebarPointLayoutService())
        {
        }

        public MainRebarRenderer(CanvasDrawer drawer, ScheduleItemQuery query, RebarPointLayoutService rebarLayoutService)
        {
            _drawer = drawer;
            _query = query;
            _rebarLayoutService = rebarLayoutService;
        }

        public void Draw(Canvas canvas, PreviewSectionLayout layout, ScheduleItem item, PreviewSectionRenderOptions options, Brush barBrush)
        {
            if (canvas == null || layout == null || item == null || options == null)
            {
                return;
            }

            var topFirstCount = _query.GetTopFirstCount(item);
            var topSecondCount = _rebarLayoutService.ClampCount(_query.GetTopSecondCount(item), topFirstCount);
            var bottomFirstCount = _query.GetBottomFirstCount(item);
            var bottomSecondCount = _rebarLayoutService.ClampCount(_query.GetBottomSecondCount(item), bottomFirstCount);

            DrawFirstLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.TopBarY1, topFirstCount, options.BarRadius, barBrush);
            DrawSecondLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.TopBarY2, topFirstCount, topSecondCount, options.BarRadius, barBrush);

            DrawFirstLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.BottomBarY1, bottomFirstCount, options.BarRadius, barBrush);
            DrawSecondLayerRebars(canvas, layout.BarStartX, layout.BarEndX, layout.BottomBarY2, bottomFirstCount, bottomSecondCount, options.BarRadius, barBrush);
        }

        private void DrawFirstLayerRebars(Canvas canvas, double startX, double endX, double y, int count, double radius, Brush barBrush)
        {
            var xs = _rebarLayoutService.GetFirstLayerXs(startX, endX, count);

            for (var i = 0; i < xs.Count; i++)
            {
                AddBar(canvas, xs[i], y, radius, barBrush);
            }
        }

        private void DrawSecondLayerRebars(Canvas canvas, double startX, double endX, double y, int firstLayerCount, int secondLayerCount, double radius, Brush barBrush)
        {
            var xs = _rebarLayoutService.GetSecondLayerXs(startX, endX, firstLayerCount, secondLayerCount);

            for (var i = 0; i < xs.Count; i++)
            {
                AddBar(canvas, xs[i], y, radius, barBrush);
            }
        }

        private void AddBar(Canvas canvas, double x, double y, double radius, Brush barBrush)
        {
            _drawer.DrawCircle(canvas, x, y, radius, null, 0.0, barBrush);
        }
    }
}
