using netDxf;
using netDxf.Entities;
using netDxf.Tables;
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
            DrawWidthDimension(layout, item, isWidthOver);
            DrawHeightDimension(layout, item, isHeightOver);
        }

        private void DrawWidthDimension(SectionDxfLayout layout, ScheduleItem item, bool isWidthOver)
        {
            var outerLeftX = layout.OuterLeft - layout.SideWing;
            var outerRightX = layout.OuterRight + layout.SideWing;

            AddAlignedDimension(
                layout.SectionLeft,
                layout.OuterTop,
                layout.SectionRight,
                layout.OuterTop,
                220.0,
                FormatDimensionText(item.Section.Width, isWidthOver));
        }

        private void DrawHeightDimension(SectionDxfLayout layout, ScheduleItem item, bool isHeightOver)
        {
            var outerLeftX = layout.OuterLeft - layout.SideWing;

            AddAlignedDimension(
                outerLeftX,
                layout.OuterTop,
                outerLeftX,
                layout.OuterBottom,
                -320.0,
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

            ApplyTemplateDimensionStyle(dimension);

            //dimension.UserText = text;

            _document.Entities.Add(dimension);
        }

        private void ApplyTemplateDimensionStyle(AlignedDimension dimension)
        {
            dimension.Layer = _document.Layers[DxfLayers.Dim];
            dimension.Color = AciColor.ByLayer;
            dimension.Linetype = Linetype.ByLayer;
            dimension.Lineweight = Lineweight.ByLayer;

            if (_document.DimensionStyles.Contains(DxfStyles.Dimension))
            {
                dimension.Style = _document.DimensionStyles[DxfStyles.Dimension];
            }
        }
    }
}