using System.Reflection;
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

            CopyWritableProperty("Color", target);
            CopyWritableProperty("Linetype", target);
            CopyWritableProperty("Lineweight", target);
            CopyWritableProperty("Transparency", target);
            CopyWritableProperty("LinetypeScale", target);
            CopyWritableProperty("Normal", target);
            CopyWritableProperty("IsVisible", target);
            CopyWritableProperty("Style", target);
        }

        private void CopyWritableProperty(string name, EntityObject target)
        {
            var sourceProperty = FindProperty(_source.GetType(), name);
            var targetProperty = FindProperty(target.GetType(), name);

            if (sourceProperty == null || targetProperty == null)
            {
                return;
            }

            if (!sourceProperty.CanRead || !targetProperty.CanWrite)
            {
                return;
            }

            if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
            {
                return;
            }

            try
            {
                var value = sourceProperty.GetValue(_source, null);
                targetProperty.SetValue(target, value, null);
            }
            catch
            {
            }
        }

        private PropertyInfo FindProperty(Type type, string name)
        {
            while (type != null)
            {
                var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (property != null)
                {
                    return property;
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}