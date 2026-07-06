using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Constants;
using GirderSchedule.Dxf.Styles.Overrides;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Styles
{
    public sealed class DxfEntityStyler
    {
        private readonly DxfDocument _document;
        private readonly DxfLayerNameSet _layerNames;
        private readonly DxfStyleNameSet _styleNames;
        private readonly DxfOverrideSet _overrides;

        public DxfEntityStyler(DxfDocument document, DxfLayerNameSet layerNames, DxfStyleNameSet styleNames, DxfOverrideSet overrides)
        {
            _document = document;
            _layerNames = layerNames ?? new DxfLayerNameSet();
            _styleNames = styleNames;
            _overrides = overrides;
        }

        public void ApplyLayer(EntityObject entity, DxfLayerRole role)
        {
            if (entity == null)
            {
                return;
            }

            var layer = GetLayer(role);

            entity.Layer = layer;
            entity.Color = AciColor.ByLayer;
            entity.Linetype = GetRoleLinetype(role);
            entity.Lineweight = Lineweight.ByLayer;
            entity.Transparency = Transparency.ByLayer;
        }

        public void ApplyStyle(EntityObject entity, DxfStyleRole role)
        {
            if (entity == null)
            {
                return;
            }

            switch (role)
            {
                case DxfStyleRole.Text:
                    ApplyTextStyle(entity);
                    break;

                case DxfStyleRole.Dimension:
                    ApplyDimensionStyle(entity);
                    break;
            }
        }

        public void ApplyOverride(EntityObject entity, DxfOverrideRole role)
        {
            if (entity == null)
            {
                return;
            }

            switch (role)
            {
                case DxfOverrideRole.Dimension:
                    _overrides.ApplyDimensionOverride(entity);
                    break;
            }
        }

        private Layer GetLayer(DxfLayerRole role)
        {
            var layerName = _layerNames.GetLayerName(role);

            if (!string.IsNullOrWhiteSpace(layerName) && _document.Layers.Contains(layerName))
            {
                return _document.Layers[layerName];
            }

            return Layer.Default;
        }

        private void ApplyTextStyle(EntityObject entity)
        {
            var style = GetTextStyle();

            if (entity is Text text)
            {
                text.Style = style;
                return;
            }

            if (entity is MText mText)
            {
                mText.Style = style;
            }
        }

        private void ApplyDimensionStyle(EntityObject entity)
        {
            var style = GetDimensionStyle();

            if (entity is Dimension dimension)
            {
                dimension.Style = style;
            }
        }

        private DimensionStyle GetDimensionStyle()
        {
            var styleName = _styleNames.GetStyleName(DxfStyleRole.Dimension);

            if (!string.IsNullOrWhiteSpace(styleName) && _document.DimensionStyles.Contains(styleName))
            {
                return _document.DimensionStyles[styleName];
            }

            return DimensionStyle.Default;
        }

        private TextStyle GetTextStyle()
        {
            var styleName = _styleNames.GetStyleName(DxfStyleRole.Text);

            if (!string.IsNullOrWhiteSpace(styleName) && _document.TextStyles.Contains(styleName))
            {
                return _document.TextStyles[styleName];
            }

            return TextStyle.Default;
        }

        private Linetype GetRoleLinetype(DxfLayerRole role)
        {
            return role == DxfLayerRole.Girder ? GetLinetype(DxfStyles.RcGirderLine) : Linetype.ByLayer;
        }

        private Linetype GetLinetype(string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && _document.Linetypes.Contains(name))
            {
                return _document.Linetypes[name];
            }

            if (_document.Linetypes.Contains(DxfStyles.RcGirderLine))
            {
                return _document.Linetypes[DxfStyles.RcGirderLine];
            }

            return Linetype.Continuous;
        }
    }
}