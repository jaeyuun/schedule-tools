using GirderSchedule.Dxf.Common;
using GirderSchedule.Domain.Girder.Layout;
using GirderSchedule.Domain.Girder.Models;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Girder
{
    public sealed class RebarDrawer
    {
        private readonly DxfDocument _document;
        private readonly DxfPointConverter _pointConverter;
        private readonly RebarPointLayoutService _rebarLayoutService;

        public RebarDrawer(DxfDocument document, DxfPointConverter pointConverter)
        {
            _document = document;
            _pointConverter = pointConverter;
            _rebarLayoutService = new RebarPointLayoutService();
        }

        public void Draw(CellBox box, ScheduleItem item)
        {
            var width = item.Section.Width <= 0 ? 400.0 : item.Section.Width;
            var height = item.Section.Height <= 0 ? 600.0 : item.Section.Height;

            var maxWidth = 950.0;
            var maxHeight = 900.0;
            var scale = maxWidth / width;

            if (height * scale > maxHeight)
            {
                scale = maxHeight / height;
            }

            var sectionWidth = width * scale;
            var sectionHeight = height * scale;

            var centerX = box.CenterX;
            var baseY = box.Y + 1550.0;

            var left = centerX - sectionWidth / 2.0;
            var right = centerX + sectionWidth / 2.0;
            var bottom = baseY;
            var top = baseY + sectionHeight;

            DrawStirrup(left + 40.0, bottom + 40.0, right - 40.0, top - 40.0);

            var startX = left + 70.0;
            var endX = right - 70.0;

            var topY1 = top - 80.0;
            var topY2 = top - 150.0;

            var bottomY1 = bottom + 80.0;
            var bottomY2 = bottom + 150.0;

            DrawBars(_rebarLayoutService.GetFirstLayerXs(startX, endX, item.TopRebar.FirstLayer.Count), topY1);
            DrawBars(_rebarLayoutService.GetSecondLayerXs(startX, endX, item.TopRebar.FirstLayer.Count, item.TopRebar.SecondLayer.Count), topY2);

            DrawBars(_rebarLayoutService.GetFirstLayerXs(startX, endX, item.BottomRebar.FirstLayer.Count), bottomY1);
            DrawBars(_rebarLayoutService.GetSecondLayerXs(startX, endX, item.BottomRebar.FirstLayer.Count, item.BottomRebar.SecondLayer.Count), bottomY2);
        }

        private void DrawStirrup(double left, double bottom, double right, double top)
        {
            var line1 = new Line(_pointConverter.ToVector3(left, bottom), _pointConverter.ToVector3(left, top));
            var line2 = new Line(_pointConverter.ToVector3(left, top), _pointConverter.ToVector3(right, top));
            var line3 = new Line(_pointConverter.ToVector3(right, top), _pointConverter.ToVector3(right, bottom));
            var line4 = new Line(_pointConverter.ToVector3(right, bottom), _pointConverter.ToVector3(left, bottom));

            ApplyRebarStyle(line1);
            ApplyRebarStyle(line2);
            ApplyRebarStyle(line3);
            ApplyRebarStyle(line4);

            _document.Entities.Add(line1);
            _document.Entities.Add(line2);
            _document.Entities.Add(line3);
            _document.Entities.Add(line4);
        }

        private void DrawBars(System.Collections.Generic.List<double> xs, double y)
        {
            for (var i = 0; i < xs.Count; i++)
            {
                DrawBar(xs[i], y);
            }
        }

        private void DrawBar(double x, double y)
        {
            var circle = new Circle(_pointConverter.ToVector3(x, y), 9.5);
            ApplyRebarStyle(circle);
            _document.Entities.Add(circle);
        }

        private void ApplyRebarStyle(EntityObject entity)
        {
            entity.Layer = _document.Layers[DxfLayers.Rebar];
            entity.Color = AciColor.ByLayer;
        }
    }
}