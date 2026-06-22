using System.Collections;
using System.Reflection;
using netDxf.Entities;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Styles.Overrides
{
    public sealed class DxfDimensionOverride
    {
        private readonly Dimension _source;
        private readonly DimensionStyle _sourceStyle;

        public string LayerName { get; private set; }
        public string StyleName { get; private set; }

        public DxfDimensionOverride(Dimension source)
        {
            _source = source;
            _sourceStyle = source.Style;
            LayerName = source.Layer == null ? string.Empty : source.Layer.Name;
            StyleName = source.Style == null ? string.Empty : source.Style.Name;
        }

        public bool IsMatch(Dimension target)
        {
            if (target == null)
            {
                return false;
            }

            var layerName = target.Layer == null ? string.Empty : target.Layer.Name;
            var styleName = target.Style == null ? string.Empty : target.Style.Name;

            return LayerName == layerName && StyleName == styleName;
        }

        public void Apply(Dimension target)
        {
            if (target == null)
            {
                return;
            }

            if (_sourceStyle != null)
            {
                target.Style = _sourceStyle;
            }

            CopyDimensionEntityValues(target);
            CopyStyleOverrides(target);
        }

        public bool IsMatch(Dimension target, string templateLayerName)
        {
            if (target == null)
            {
                return false;
            }

            var styleName = target.Style == null ? string.Empty : target.Style.Name;

            return LayerName == templateLayerName && StyleName == styleName;
        }

        private void CopyDimensionEntityValues(Dimension target)
        {
            CopyWritableProperty(_source, target, "Color");
            CopyWritableProperty(_source, target, "Linetype");
            CopyWritableProperty(_source, target, "Lineweight");
            CopyWritableProperty(_source, target, "Transparency");
            CopyWritableProperty(_source, target, "LinetypeScale");
            CopyWritableProperty(_source, target, "Normal");
            CopyWritableProperty(_source, target, "Elevation");
        }

        private void CopyStyleOverrides(Dimension target)
        {
            var sourceProperty = FindProperty(_source.GetType(), "StyleOverrides");
            var targetProperty = FindProperty(target.GetType(), "StyleOverrides");

            if (sourceProperty == null || targetProperty == null)
            {
                return;
            }

            if (!sourceProperty.CanRead || !targetProperty.CanRead)
            {
                return;
            }

            var sourceOverrides = sourceProperty.GetValue(_source, null);
            var targetOverrides = targetProperty.GetValue(target, null);

            if (sourceOverrides == null || targetOverrides == null)
            {
                return;
            }

            ClearCollection(targetOverrides);

            var values = GetOverrideObjects(sourceOverrides);

            if (values == null)
            {
                return;
            }

            foreach (var value in values)
            {
                if (value == null)
                {
                    continue;
                }

                AddOverride(targetOverrides, CloneObject(value));
            }
        }

        private IEnumerable GetOverrideObjects(object collection)
        {
            var valuesProperty = collection.GetType().GetProperty("Values", BindingFlags.Instance | BindingFlags.Public);

            if (valuesProperty != null && valuesProperty.CanRead)
            {
                var values = valuesProperty.GetValue(collection, null) as IEnumerable;

                if (values != null)
                {
                    return values;
                }
            }

            return collection as IEnumerable;
        }

        private void AddOverride(object collection, object overrideObject)
        {
            if (collection == null || overrideObject == null)
            {
                return;
            }

            var methods = collection.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];

                if (method.Name != "Add")
                {
                    continue;
                }

                var parameters = method.GetParameters();

                if (parameters.Length != 1)
                {
                    continue;
                }

                if (!parameters[0].ParameterType.IsAssignableFrom(overrideObject.GetType()))
                {
                    continue;
                }

                try
                {
                    method.Invoke(collection, new[] { overrideObject });
                    return;
                }
                catch
                {
                    return;
                }
            }
        }

        private object CloneObject(object value)
        {
            if (value == null)
            {
                return null;
            }

            var cloneMethod = value.GetType().GetMethod("Clone", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

            if (cloneMethod == null)
            {
                return value;
            }

            try
            {
                return cloneMethod.Invoke(value, null);
            }
            catch
            {
                return value;
            }
        }

        private void ClearCollection(object collection)
        {
            var clearMethod = collection.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);

            if (clearMethod == null)
            {
                return;
            }

            try
            {
                clearMethod.Invoke(collection, null);
            }
            catch
            {
            }
        }

        private void CopyWritableProperty(object source, object target, string name)
        {
            if (source == null || target == null)
            {
                return;
            }

            var sourceProperty = FindProperty(source.GetType(), name);
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
                var value = sourceProperty.GetValue(source, null);
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