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
        private readonly DxfLayerNameResolver _layerResolver;
        private readonly DxfOverrideSet _overrides;

        public DxfEntityStyler(DxfDocument document, DxfLayerNameSet layerNames, DxfOverrideSet overrides)
        {
            _document = document;
            _layerNames = layerNames ?? new DxfLayerNameSet();
            _layerResolver = new DxfLayerNameResolver(document, _layerNames);
            _overrides = overrides;
        }

        public void Apply(EntityObject entity, DxfStyleRole role)
        {
            if (entity == null)
            {
                return;
            }

            var layer = _layerResolver.GetLayer(role);
            var templateLayerName = _layerNames.GetTemplateLayerName(role);

            entity.Layer = layer;
            entity.Color = AciColor.ByLayer;
            entity.Linetype = GetRoleLinetype(role);
            entity.Lineweight = Lineweight.ByLayer;

            if (_overrides != null)
            {
                _overrides.Apply(entity, templateLayerName);
            }

            entity.Layer = layer;
        }

        private Linetype GetRoleLinetype(DxfStyleRole role)
        {
            return role == DxfStyleRole.Girder ? GetLinetype(DxfStyles.RcGirderLine) : Linetype.ByLayer;
        }

        private Linetype GetLinetype(string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && _document.Linetypes.Contains(name))
            {
                return _document.Linetypes[name];
            }

            if (_document.Linetypes.Contains(DxfStyles.Continuous))
            {
                return _document.Linetypes[DxfStyles.Continuous];
            }

            return Linetype.Continuous;
        }
    }
}
