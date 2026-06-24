using GirderSchedule.Dxf.Constants;
using netDxf;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Styles
{
    public sealed class DxfLayerNameResolver
    {
        private readonly DxfDocument _document;
        private readonly DxfLayerNameSet _layerNames;

        public DxfLayerNameResolver(DxfDocument document, DxfLayerNameSet layerNames)
        {
            _document = document;
            _layerNames = layerNames ?? new DxfLayerNameSet();
        }

        public Layer GetLayer(DxfStyleRole role)
        {
            var layerName = GetLayerName(role);
            var templateLayerName = _layerNames.GetTemplateLayerName(role);

            if (string.IsNullOrWhiteSpace(layerName))
            {
                layerName = templateLayerName;
            }

            if (string.IsNullOrWhiteSpace(layerName))
            {
                layerName = DxfLayers.Text;
            }

            if (_document.Layers.Contains(layerName))
            {
                return _document.Layers[layerName];
            }

            var layer = new Layer(layerName);
            CopyLayerProperties(layer, templateLayerName);
            _document.Layers.Add(layer);

            return layer;
        }

        public string GetLayerName(DxfStyleRole role)
        {
            return _layerNames.GetLayerName(role);
        }

        private void CopyLayerProperties(Layer target, string templateLayerName)
        {
            if (target == null || string.IsNullOrWhiteSpace(templateLayerName))
            {
                return;
            }

            if (!_document.Layers.Contains(templateLayerName))
            {
                return;
            }

            var source = _document.Layers[templateLayerName];

            target.Color = source.Color;
            target.Linetype = source.Linetype;
            target.Lineweight = source.Lineweight;
            target.IsVisible = source.IsVisible;
            target.IsFrozen = source.IsFrozen;
            target.Plot = source.Plot;
        }
    }
}