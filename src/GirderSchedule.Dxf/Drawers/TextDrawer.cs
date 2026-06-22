using GirderSchedule.Dxf.Constants;
using GirderSchedule.Dxf.Geometry;
using GirderSchedule.Dxf.Styles;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class TextDrawer
    {
        private const string TemplateTextStyleName = "맑은고딕";
        private const short RebarTextColorIndex = 3;

        private readonly DxfDocument _document;
        private readonly DxfEntityStyler _styler;

        public TextDrawer(DxfDocument document, DxfEntityStyler styler)
        {
            _document = document;
            _styler = styler;
        }

        public void DrawFormText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfStyleRole.TitleText, TextAlignment.MiddleCenter, null, 0.0);
        }

        public void DrawValueText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfStyleRole.ContentText, TextAlignment.MiddleCenter, null, 0.0);
        }

        public void DrawRebarValueText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfStyleRole.ContentText, TextAlignment.MiddleCenter, new AciColor(RebarTextColorIndex), 0.0);
        }

        public void DrawDefPointText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfStyleRole.Defpoint, TextAlignment.MiddleCenter, null, 0.0);
        }

        public void DrawFormRawText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfStyleRole.TitleText, TextAlignment.BaselineLeft, null, 0.0);
        }

        public void DrawRebarValueRawText(string value, double x, double y, double height, TextAlignment textAlignment)
        {
            Draw(value, x, y, height, DxfStyleRole.ContentText, textAlignment, new AciColor(RebarTextColorIndex), 0.0);
        }

        private void Draw(string value, double x, double y, double height, DxfStyleRole role, TextAlignment alignment, AciColor color, double rotation)
        {
            var text = new Text(value ?? string.Empty, DxfPointConverter.ToVector3(x, y), height);
            text.Alignment = alignment;
            text.Rotation = rotation;

            ApplyTextStyle(text);
            _styler.Apply(text, role);

            if (color != null)
            {
                text.Color = color;
            }

            _document.Entities.Add(text);
        }

        private void ApplyTextStyle(Text text)
        {
            if (_document.TextStyles.Contains(TemplateTextStyleName))
            {
                text.Style = _document.TextStyles[TemplateTextStyleName];
                return;
            }

            if (_document.TextStyles.Contains(DxfStyles.Text))
            {
                text.Style = _document.TextStyles[DxfStyles.Text];
            }
        }
    }
}
