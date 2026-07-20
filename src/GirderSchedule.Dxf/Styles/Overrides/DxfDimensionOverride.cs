using netDxf.Entities;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Styles.Overrides
{
    public sealed class DxfDimensionOverride
    {
        private readonly Dimension _source;

        private string _styleName;

        public DxfDimensionOverride(Dimension source)
        {
            _source = source;
            _styleName = source.Style == null ? string.Empty : source.Style.Name;
        }

        public bool IsMatch(Dimension target)
        {
            if (target == null)
            {
                return false;
            }

            var styleName = target.Style == null ? string.Empty : target.Style.Name;

            return string.Equals(_styleName, styleName, StringComparison.OrdinalIgnoreCase);
        }

        public void Apply(Dimension target)
        {
            if (target == null)
            {
                return;
            }

            CopyDimensionStyleOverrides(target);
        }

        public static void ApplyDefault(Dimension dimension)
        {
            if (dimension == null)
            {
                return;
            }

            DxfOverridePropertyCopier.ClearCollection(dimension.StyleOverrides);

            dimension.StyleOverrides.Add(new DimensionStyleOverride(DimensionStyleOverrideType.ArrowSize, 0.8));
            dimension.StyleOverrides.Add(new DimensionStyleOverride(DimensionStyleOverrideType.TextHeight, 1.5));
            dimension.StyleOverrides.Add(new DimensionStyleOverride(DimensionStyleOverrideType.TextOffset, 0.4));
            dimension.StyleOverrides.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimScaleOverall, 50.0));
            dimension.StyleOverrides.Add(new DimensionStyleOverride(DimensionStyleOverrideType.ExtLineOffset, 1.0));
        }

        private void CopyDimensionStyleOverrides(Dimension target)
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