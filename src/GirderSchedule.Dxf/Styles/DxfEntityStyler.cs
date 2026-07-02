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
        private readonly DxfLayerNameResolver _layerResolver;
        private readonly DxfOverrideSet _overrides;

        public DxfEntityStyler(DxfDocument document, DxfLayerNameSet layerNames, DxfOverrideSet overrides)
        {
            _document = document;
            _layerNames = layerNames ?? new DxfLayerNameSet();
            _layerResolver = new DxfLayerNameResolver(document, _layerNames);
            _overrides = overrides;
        }

        public void Apply(EntityObject entity, DxfLayerRole role)
        {
            if (entity == null)
            {
                return;
            }

            ApplyBaseStyle(entity, role);
            ApplyTemplateOverride(entity, role);
        }

        public void ApplyDimension(EntityObject entity)
        {
            if (entity == null)
            {
                return;
            }

            ApplyBaseStyle(entity, DxfLayers.Dim, DxfLayers.Dim);
        }

        public void ApplyMemberForce(EntityObject entity)
        {
            if (entity == null)
            {
                return;
            }

            ApplyBaseStyle(entity, DxfLayerRole.MemberForce);
            ApplyTemplateOverride(entity, DxfLayerRole.MemberForce);
        }

        private void ApplyBaseStyle(EntityObject entity, DxfLayerRole role)
        {
            var layer = _layerResolver.GetLayer(role);

            entity.Layer = layer;
            entity.Color = AciColor.ByLayer;
            entity.Linetype = GetRoleLinetype(role);
            entity.Lineweight = Lineweight.ByLayer;
        }

        private void ApplyBaseStyle(EntityObject entity, string layerName, string templateLayerName)
        {
            var layer = _layerResolver.GetLayer(layerName, templateLayerName);

            entity.Layer = layer;
            entity.Color = AciColor.ByLayer;
            entity.Linetype = Linetype.ByLayer;
            entity.Lineweight = Lineweight.ByLayer;

            ApplyTemplateOverride(entity, templateLayerName);
        }

        private void ApplyTemplateOverride(EntityObject entity, DxfLayerRole role)
        {
            if (_overrides == null)
            {
                return;
            }

            var layer = entity.Layer;
            var templateLayerName = _layerNames.GetTemplateLayerName(role);

            _overrides.Apply(entity, templateLayerName);

            if (layer != null)
            {
                entity.Layer = layer;
            }
        }

        private void ApplyTemplateOverride(EntityObject entity, string templateLayerName)
        {
            if (_overrides == null)
            {
                return;
            }

            var layer = entity.Layer;

            _overrides.Apply(entity, templateLayerName);

            if (layer != null)
            {
                entity.Layer = layer;
            }
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