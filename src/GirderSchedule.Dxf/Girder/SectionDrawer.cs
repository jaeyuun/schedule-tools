using System.Collections.Generic;
using GirderSchedule.Dxf.Common;
using GirderSchedule.Domain.Girder.Layout;
using GirderSchedule.Domain.Girder.Models;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Girder
{
    public sealed class SectionDrawer
    {
        private readonly DxfDocument _document;
        private readonly DxfPointConverter _pointConverter;

        public SectionDrawer(DxfDocument document, DxfPointConverter pointConverter)
        {
            _document = document;
            _pointConverter = pointConverter;
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

            var slabOverhang = 175.0;
            var slabThickness = 150.0;

            DrawSlab(left - slabOverhang, top + slabThickness, left, top, right, right + slabOverhang);
            DrawBeam(left, bottom, right, top);
        }

        private void DrawBeam(double left, double bottom, double right, double top)
        {
            var vertices = new List<Polyline2DVertex>();
            vertices.Add(new Polyline2DVertex(_pointConverter.ToVector2(left, top)));
            vertices.Add(new Polyline2DVertex(_pointConverter.ToVector2(left, bottom)));
            vertices.Add(new Polyline2DVertex(_pointConverter.ToVector2(right, bottom)));
            vertices.Add(new Polyline2DVertex(_pointConverter.ToVector2(right, top)));

            var polyline = new Polyline2D(vertices, false);
            ApplySectionStyle(polyline);

            _document.Entities.Add(polyline);
        }

        private void DrawSlab(double leftEnd, double slabTop, double left, double beamTop, double right, double rightEnd)
        {
            var vertices = new List<Polyline2DVertex>();
            vertices.Add(new Polyline2DVertex(_pointConverter.ToVector2(leftEnd, slabTop)));
            vertices.Add(new Polyline2DVertex(_pointConverter.ToVector2(left, slabTop)));
            vertices.Add(new Polyline2DVertex(_pointConverter.ToVector2(left, beamTop)));
            vertices.Add(new Polyline2DVertex(_pointConverter.ToVector2(right, beamTop)));
            vertices.Add(new Polyline2DVertex(_pointConverter.ToVector2(right, slabTop)));
            vertices.Add(new Polyline2DVertex(_pointConverter.ToVector2(rightEnd, slabTop)));

            var polyline = new Polyline2D(vertices, false);
            ApplySectionStyle(polyline);

            _document.Entities.Add(polyline);
        }

        private void ApplySectionStyle(EntityObject entity)
        {
            entity.Layer = _document.Layers[DxfLayers.RcGirder];
            entity.Color = AciColor.ByLayer;

            if (_document.Linetypes.Contains(DxfStyles.RcGirderLine))
            {
                entity.Linetype = _document.Linetypes[DxfStyles.RcGirderLine];
            }
        }
    }
}