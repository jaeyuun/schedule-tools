using GirderSchedule.App.Rendering.Layouts;
using GirderSchedule.App.Rendering.Models;
using GirderSchedule.Domain.Layouts;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;
using ScheduleTools.Wpf.Rendering;
using System.Windows.Controls;

namespace GirderSchedule.App.Rendering.Section
{
    public sealed class SectionRenderer
    {
        private readonly CanvasDrawer _drawer;
        private readonly RenderBrushSet _brushes;
        private readonly ScheduleItemQuery _query = new ScheduleItemQuery();
        private readonly RebarPointLayoutService _rebarLayoutService = new RebarPointLayoutService();

        private readonly PreviewSectionLayoutFactory _layoutFactory = new PreviewSectionLayoutFactory();
        private readonly MainRebarRenderer _rebarRenderer;
        private readonly StirrupRenderer _stirrupRenderer;
        private readonly DimensionRenderer _dimensionRenderer;
        private readonly SkinRebarRenderer _skinRebarRenderer;
        private readonly GirderRenderer _girderRenderer;

        public SectionRenderer()
            : this(new CanvasDrawer(), new RenderBrushSet())
        {
        }

        public SectionRenderer(CanvasDrawer drawer, RenderBrushSet brushes)
        {
            _drawer = drawer ?? new CanvasDrawer();
            _brushes = brushes ?? new RenderBrushSet();

            _girderRenderer = new GirderRenderer(_drawer);
            _dimensionRenderer = new DimensionRenderer(_drawer);
            _stirrupRenderer = new StirrupRenderer(_drawer, _query, _rebarLayoutService);
            _rebarRenderer = new MainRebarRenderer(_drawer, _query, _rebarLayoutService);
            _skinRebarRenderer = new SkinRebarRenderer(_drawer);
        }

        public void Draw(Canvas canvas, ScheduleItem item, double areaX, double areaY, double areaWidth, double areaHeight, bool isWidthOver, bool isHeightOver, PreviewSectionRenderOptions options)
        {
            if (canvas == null || item == null || item.Section == null)
            {
                return;
            }

            var renderOptions = options ?? PreviewSectionRenderOptions.Editor;
            var layout = _layoutFactory.Create(areaX, areaY, areaWidth, areaHeight, item.Section.Width, item.Section.Height, renderOptions);

            _girderRenderer.Draw(canvas, layout, renderOptions, _brushes.Line);
            _dimensionRenderer.Draw(canvas, layout, item.Section.Width, item.Section.Height, isWidthOver, isHeightOver, renderOptions, _brushes.Line);
            _stirrupRenderer.Draw(canvas, layout, item, renderOptions, _brushes.Line);
            _rebarRenderer.Draw(canvas, layout, item, renderOptions, _brushes.Rebar);
            _skinRebarRenderer.Draw(canvas, layout, item, _brushes.Line);
        }
    }
}
