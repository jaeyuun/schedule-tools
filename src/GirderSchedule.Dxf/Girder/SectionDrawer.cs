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
            AddLine(layout.OuterLeft - layout.SideWing, layout.OuterTop, layout.OuterRight + layout.SideWing, layout.OuterTop, DxfLayers.RcGir);

            AddLine(layout.OuterLeft - layout.SideWing, layout.OuterShelfY, layout.OuterLeft, layout.OuterShelfY, DxfLayers.RcGir);
            AddLine(layout.OuterLeft, layout.OuterShelfY, layout.OuterLeft, layout.OuterBottom, DxfLayers.RcGir);
            AddLine(layout.OuterLeft, layout.OuterBottom, layout.OuterRight, layout.OuterBottom, DxfLayers.RcGir);
            AddLine(layout.OuterRight, layout.OuterBottom, layout.OuterRight, layout.OuterShelfY, DxfLayers.RcGir);
            AddLine(layout.OuterRight, layout.OuterShelfY, layout.OuterRight + layout.SideWing, layout.OuterShelfY, DxfLayers.RcGir);
        }

        private void AddLine(double x1, double y1, double x2, double y2, string layerName)
        {
            var line = new Line(_pointConverter.ToVector3(x1, y1), _pointConverter.ToVector3(x2, y2));
            DxfEntityStyle.ApplyByLayer(line, _document.Layers[layerName]);
            _document.Entities.Add(line);
        }
    }
}