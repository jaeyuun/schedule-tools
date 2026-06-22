using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Dxf.Constants;
using GirderSchedule.Dxf.Geometry;
using GirderSchedule.Dxf.Layouts;
using GirderSchedule.Dxf.Styles;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class DimensionDrawer
    {
        private const double WidthDimensionOffset = 200.0;
        private const double HeightDimensionOffset = -250.0;

        private readonly DxfDocument _document;
        private readonly DxfEntityStyler _styler;

        public DimensionDrawer(DxfDocument document, DxfEntityStyler styler)
        {
            _document = document;
            _styler = styler;
        }

        public void Draw(SectionLayout layout, ScheduleItem item, bool isWidthOver, bool isHeightOver)
        {
            if (layout == null || item == null || item.Section == null)
            {
                return;
            }

            DrawWidthDimension(layout, item.Section.Width, isWidthOver);
            DrawHeightDimension(layout, item.Section.Height, isHeightOver);
        }

        private void DrawWidthDimension(SectionLayout layout, double width, bool isWidthOver)
        {
            AddAlignedDimension(layout.SectionLeft, layout.OuterTop, layout.SectionRight, layout.OuterTop, WidthDimensionOffset, FormatUserText(width, isWidthOver));
        }

        private void DrawHeightDimension(SectionLayout layout, double height, bool isHeightOver)
        {
            var outerLeftX = layout.OuterLeft - layout.SideWing;
            AddAlignedDimension(outerLeftX, layout.OuterTop, outerLeftX, layout.OuterBottom, HeightDimensionOffset, FormatUserText(height, isHeightOver));
        }

        private string FormatUserText(double value, bool isOver)
        {
            return isOver ? value.ToString("0") + " 이상" : string.Empty;
        }

        private void AddAlignedDimension(double x1, double y1, double x2, double y2, double offset, string userText)
        {
            var dimension = new AlignedDimension(DxfPointConverter.ToVector2(x1, y1), DxfPointConverter.ToVector2(x2, y2), offset, GetDimensionStyle());

            _styler.Apply(dimension, DxfStyleRole.Dimension);

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
    }
}
