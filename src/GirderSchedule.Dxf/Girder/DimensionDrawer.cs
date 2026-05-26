using netDxf;
using netDxf.Entities;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Dxf.Common;

namespace GirderSchedule.Dxf.Girder
{
    public sealed class DimensionDrawer
    {
        private readonly DxfDocument _document;
        private readonly DxfPointConverter _pointConverter;

        public DimensionDrawer(DxfDocument document, DxfPointConverter pointConverter)
        {
            _document = document;
            _pointConverter = pointConverter;
        }

        public void Draw(SectionDxfLayout layout, ScheduleItem item, bool isWidthOver, bool isHeightOver)
        {
            AddAlignedDimension(
                layout.SectionLeft,
                layout.SectionTop,
                layout.SectionRight,
                layout.SectionTop,
                220.0,
                FormatDimensionText(item.Section.Width, isWidthOver));

            AddAlignedDimension(
                layout.SectionLeft,
                layout.SectionTop,
                layout.SectionLeft,
                layout.SectionBottom,
                -220.0,
                FormatDimensionText(item.Section.Height, isHeightOver));
        }

        private string FormatDimensionText(double value, bool isOver)
        {
            var text = value.ToString("0");

            if (!isOver)
            {
                return text;
            }

            return text + " 이상";
        }

        private void AddAlignedDimension(double x1, double y1, double x2, double y2, double offset, string text)
        {
            var dimension = new AlignedDimension(
                _pointConverter.ToVector2(x1, y1),
                _pointConverter.ToVector2(x2, y2),
                offset);

            DxfEntityStyle.ApplyByLayer(dimension, _document.Layers[DxfLayers.Dim]);
            dimension.Style = _document.DimensionStyles[DxfStyles.Dimension];
            dimension.UserText = text;

            _document.Entities.Add(dimension);
        }
    }
}