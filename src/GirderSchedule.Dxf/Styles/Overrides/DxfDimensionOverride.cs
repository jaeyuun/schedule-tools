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

        public bool IsMatch(Dimension target, string templateLayerName)
        {
            if (target == null)
            {
                return false;
            }

            var styleName = target.Style == null ? string.Empty : target.Style.Name;

            return LayerName == templateLayerName && StyleName == styleName;
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

        private void CopyDimensionEntityValues(Dimension target)
        {
            CopyDimensionProperty(target, "Color");
            CopyDimensionProperty(target, "Linetype");
            CopyDimensionProperty(target, "Lineweight");
            CopyDimensionProperty(target, "Transparency");
            CopyDimensionProperty(target, "LinetypeScale");
            CopyDimensionProperty(target, "Normal");
            CopyDimensionProperty(target, "Elevation");
        }

        private void CopyDimensionProperty(Dimension target, string name)
        {
            DxfOverridePropertyCopier.CopyWritableProperty(_source, target, name);
        }

        private void CopyStyleOverrides(Dimension target)
        {
            var sourceProperty = DxfOverridePropertyCopier.FindProperty(_source.GetType(), "StyleOverrides");
            var targetProperty = DxfOverridePropertyCopier.FindProperty(target.GetType(), "StyleOverrides");

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

            DxfOverridePropertyCopier.ClearCollection(targetOverrides);

            var values = DxfOverridePropertyCopier.GetCollectionValues(sourceOverrides);

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

                var clone = DxfOverridePropertyCopier.CloneObject(value);
                DxfOverridePropertyCopier.AddObjectToCollection(targetOverrides, clone);
            }
        }
    }
}