using netDxf;
using netDxf.Entities;
using netDxf.Tables;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Dxf.Common;
using GirderSchedule.Dxf.Common.Overrides;
using GirderSchedule.Dxf.Layout;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class DimensionDrawer
    {
        private const double WidthDimensionOffset = 200;
        private const double HeightDimensionOffset = -250;

        private readonly DxfDocument _document;
        private readonly DxfPointConverter _pointConverter;
        private readonly DxfOverrideTemplateSet _overrides;

        public DimensionDrawer(DxfDocument document, DxfPointConverter pointConverter, DxfOverrideTemplateSet overrides)
        {
            _document = document;
            _pointConverter = pointConverter;
            _overrides = overrides;
        }

        public void Draw(SectionDxfLayout layout, ScheduleItem item, bool isWidthOver, bool isHeightOver)
        {
            DrawWidthDimension(layout, item, isWidthOver);
            DrawHeightDimension(layout, item, isHeightOver);
        }

        private void DrawWidthDimension(SectionDxfLayout layout, ScheduleItem item, bool isWidthOver)
        {
            AddAlignedDimension(
                layout.SectionLeft,
                layout.OuterTop,
                layout.SectionRight,
                layout.OuterTop,
                WidthDimensionOffset,
                FormatUserText(item.Section.Width, isWidthOver));
        }

        private void DrawHeightDimension(SectionDxfLayout layout, ScheduleItem item, bool isHeightOver)
        {
            var outerLeftX = layout.OuterLeft - layout.SideWing;

            AddAlignedDimension(
                outerLeftX,
                layout.OuterTop,
                outerLeftX,
                layout.OuterBottom,
                HeightDimensionOffset,
                FormatUserText(item.Section.Height, isHeightOver));
        }

        private string FormatUserText(double value, bool isOver)
        {
            if (!isOver)
            {
                return string.Empty;
            }

            return value.ToString("0") + " 이상";
        }

        private void AddAlignedDimension(double x1, double y1, double x2, double y2, double offset, string userText)
        {
            var style = GetDimensionStyle();

            var dimension = new AlignedDimension(
                _pointConverter.ToVector2(x1, y1),
                _pointConverter.ToVector2(x2, y2),
                offset,
                style);

            DxfEntityStyle.ApplyDimension(dimension, _document);
            ApplyOverrides(dimension);

            dimension.Layer = _document.Layers[DxfLayers.Dim];
            dimension.Color = AciColor.ByLayer;
            dimension.Linetype = Linetype.ByLayer;
            dimension.Lineweight = Lineweight.ByLayer;

            if (!string.IsNullOrWhiteSpace(userText))
            {
                dimension.UserText = userText;
            }

            dimension.Update();

            _document.Entities.Add(dimension);
        }

        private DimensionStyle GetDimensionStyle()
        {
            if (_document.DimensionStyles.Contains(DxfStyles.Dimension))
            {
                return _document.DimensionStyles[DxfStyles.Dimension];
            }

            return DimensionStyle.Default;
        }

        private void ApplyOverrides(EntityObject entity)
        {
            if (_overrides != null)
            {
                _overrides.Apply(entity);
            }
        }
    }
}