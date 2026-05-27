using netDxf;
using netDxf.Entities;
using GirderSchedule.Dxf.Common;

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

        public void Draw(SectionDxfLayout layout)
        {
            AddLine(
                layout.OuterLeft - layout.SideWing,
                layout.OuterTop,
                layout.OuterRight + layout.SideWing,
                layout.OuterTop);

            AddPolyline(
                layout.OuterLeft - layout.SideWing, layout.OuterShelfY,
                layout.OuterLeft, layout.OuterShelfY,
                layout.OuterLeft, layout.OuterBottom,
                layout.OuterRight, layout.OuterBottom,
                layout.OuterRight, layout.OuterShelfY,
                layout.OuterRight + layout.SideWing, layout.OuterShelfY);
        }

        private void AddLine(double x1, double y1, double x2, double y2)
        {
            var line = new Line(_pointConverter.ToVector3(x1, y1), _pointConverter.ToVector3(x2, y2));
            DxfEntityStyle.ApplyRcGir(line, _document);
            _document.Entities.Add(line);
        }

        private void AddPolyline(params double[] values)
        {
            if (values == null || values.Length == 0)
            {
                return;
            }

            if (values.Length % 2 != 0)
            {
                return;
            }

            var polyline = new Polyline2D();

            for (var i = 0; i < values.Length; i += 2)
            {
                polyline.Vertexes.Add(new Polyline2DVertex(_pointConverter.ToVector2(values[i], values[i + 1])));
            }

            polyline.IsClosed = false;

            DxfEntityStyle.ApplyRcGir(polyline, _document);
            _document.Entities.Add(polyline);
        }
    }
}