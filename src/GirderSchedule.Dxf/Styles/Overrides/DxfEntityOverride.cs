using netDxf.Entities;

namespace GirderSchedule.Dxf.Styles.Overrides
{
    public sealed class DxfEntityOverride
    {
        private readonly EntityObject _source;

        public string LayerName { get; private set; }
        public string EntityTypeName { get; private set; }

        public DxfEntityOverride(EntityObject source)
        {
            _source = source;
            LayerName = source.Layer == null ? string.Empty : source.Layer.Name;
            EntityTypeName = source.GetType().FullName;
        }

        public bool IsMatch(EntityObject target)
        {
            if (target == null)
            {
                return false;
            }

            var layerName = target.Layer == null ? string.Empty : target.Layer.Name;
            var typeName = target.GetType().FullName;

            return LayerName == layerName && EntityTypeName == typeName;
        }

        public bool IsMatch(EntityObject target, string templateLayerName)
        {
            if (target == null)
            {
                return false;
            }

            var typeName = target.GetType().FullName;

            return LayerName == templateLayerName && EntityTypeName == typeName;
        }

        public void Apply(EntityObject target)
        {
            if (target == null)
            {
                return;
            }

            CopyEntityProperty(target, "Color");
            CopyEntityProperty(target, "Linetype");
            CopyEntityProperty(target, "Lineweight");
            CopyEntityProperty(target, "Transparency");
            CopyEntityProperty(target, "LinetypeScale");
            CopyEntityProperty(target, "Normal");
            CopyEntityProperty(target, "IsVisible");
            CopyEntityProperty(target, "Style");
        }

        private void CopyEntityProperty(EntityObject target, string name)
        {
            DxfOverridePropertyCopier.CopyWritableProperty(_source, target, name);
        }
    }
}