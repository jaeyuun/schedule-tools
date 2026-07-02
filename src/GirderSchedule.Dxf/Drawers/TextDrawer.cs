using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Constants;
using GirderSchedule.Dxf.Geometry;
using GirderSchedule.Dxf.Styles;
using netDxf;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class TextDrawer
    {
        private const short RebarTextColorIndex = 3;

        private readonly DxfDocument _document;
        private readonly DxfEntityStyler _styler;
        private readonly DxfStyleNameSet _styleNames;

        public TextDrawer(DxfDocument document, DxfEntityStyler styler, DxfStyleNameSet styleNames)
        {
            _document = document;
            _styler = styler;
            _styleNames = styleNames ?? new DxfStyleNameSet();
        }

        public void DrawFormText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfLayerRole.FormText, TextAlignment.MiddleCenter, null, 0.0);
        }

        public void DrawValueText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfLayerRole.Text, TextAlignment.MiddleCenter, null, 0.0);
        }

        public void DrawRebarValueText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfLayerRole.Text, TextAlignment.MiddleCenter, new AciColor(RebarTextColorIndex), 0.0);
        }

        public void DrawMemberForceText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfLayerRole.MemberForce, TextAlignment.MiddleCenter, null, 0.0);
        }

        public void DrawFormRawText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfLayerRole.FormText, TextAlignment.BaselineLeft, null, 0.0);
        }

        public void DrawRebarValueRawText(string value, double x, double y, double height, TextAlignment textAlignment)
        {
            Draw(value, x, y, height, DxfLayerRole.Text, textAlignment, new AciColor(RebarTextColorIndex), 0.0);
        }

        private void Draw(string value, double x, double y, double height, DxfLayerRole role, TextAlignment alignment, AciColor color, double rotation)
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
            if (!string.IsNullOrWhiteSpace(_styleNames.TextStyleName) && _document.TextStyles.Contains(_styleNames.TextStyleName))
            {
                text.Style = _document.TextStyles[_styleNames.TextStyleName];
                return;
            }

            if (_document.TextStyles.Contains(DxfStyles.DefaultText))
            {
                text.Style = _document.TextStyles[DxfStyles.DefaultText];
                return;
            }

            if (_document.TextStyles.Contains(DxfStyles.StandardText))
            {
                text.Style = _document.TextStyles[DxfStyles.StandardText];
            }
        }
    }
}
