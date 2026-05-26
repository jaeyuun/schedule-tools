using netDxf;
using netDxf.Entities;
using netDxf.Tables;
using GirderSchedule.Dxf.Common;

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
            Draw(value, x, y, height, DxfLayers.FormText);
        }

        public void DrawValueText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfLayers.Text);
        }

        public void DrawRebarValueText(string value, double x, double y, double height)
        {
            Draw(value, x, y, height, DxfLayers.Text);
        }

        public void DrawDimText(string value, double x, double y, double height, double rotation)
        {
            var text = CreateText(value, x, y, height, DxfLayers.Dim);
            text.Rotation = rotation;
            _document.Entities.Add(text);
        }

        private void Draw(string value, double x, double y, double height, string layerName)
        {
            var text = CreateText(value, x, y, height, layerName);
            _document.Entities.Add(text);
        }

        private Text CreateText(string value, double x, double y, double height, string layerName)
        {
            var text = new Text(value ?? string.Empty, _pointConverter.ToVector3(x, y), height);
            DxfEntityStyle.ApplyByLayer(text, _document.Layers[layerName]);
            text.Alignment = TextAlignment.MiddleCenter;

            if (_document.TextStyles.Contains(DxfStyles.Text))
            {
                text.Style = _document.TextStyles[DxfStyles.Text];
            }

            return text;
        }
    }
}