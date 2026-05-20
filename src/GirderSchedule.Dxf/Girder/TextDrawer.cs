using GirderSchedule.Dxf.Common;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Girder
{
    public sealed class TextDrawer
    {
        private readonly DxfDocument _document;
        private readonly DxfPointConverter _pointConverter;

        public TextDrawer(DxfDocument document, DxfPointConverter pointConverter)
        {
            _document = document;
            _pointConverter = pointConverter;
        }

        public void DrawFormText(string value, double x, double y, double height)
        {
            var text = CreateText(value, x, y, height);
            text.Layer = _document.Layers[DxfLayers.FormText];
            text.Color = AciColor.ByLayer;
            _document.Entities.Add(text);
        }

        public void DrawValueText(string value, double x, double y, double height)
        {
            var text = CreateText(value, x, y, height);
            text.Layer = _document.Layers[DxfLayers.Text];
            text.Color = AciColor.ByLayer;
            _document.Entities.Add(text);
        }

        public void DrawRebarValueText(string value, double x, double y, double height)
        {
            var text = CreateText(value, x, y, height);
            text.Layer = _document.Layers[DxfLayers.Text];
            text.Color = new AciColor(3);
            _document.Entities.Add(text);
        }

        private Text CreateText(string value, double x, double y, double height)
        {
            var text = new Text(value, _pointConverter.ToVector3(x, y), height);
            text.Alignment = TextAlignment.MiddleCenter;

            if (_document.TextStyles.Contains(DxfStyles.Text))
            {
                text.Style = _document.TextStyles[DxfStyles.Text];
            }

            return text;
        }
    }
}