using netDxf;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Styles.Overrides
{
    public sealed class DxfOverrideSet
    {
        private readonly List<DxfDimensionOverride> _dimensionOverrides = new List<DxfDimensionOverride>();

        public static DxfOverrideSet Create(DxfDocument document)
        {
            var set = new DxfOverrideSet();

            if (document == null)
            {
                return set;
            }

            foreach (var entity in document.Entities.All)
            {
                set.Add(entity);
            }

            return set;
        }

        public void Add(EntityObject entity)
        {
            if (entity == null)
            {
                return;
            }

            if (entity is Dimension dimension)
            {
                _dimensionOverrides.Add(new DxfDimensionOverride(dimension));
                return;
            }

            // _entityOverrides.Add(new DxfEntityOverride(entity));
        }

        public void ApplyDimensionOverride(EntityObject entity)
        {
            var dimension = entity as Dimension;
            
            if (dimension == null)
            {
                return;
            }

            var overrideItem = FindDimensionOverride(dimension);

            if (overrideItem != null)
            {
                overrideItem.Apply(dimension);
            }
        }

        private DxfDimensionOverride FindDimensionOverride(Dimension dimension)
        {
            for (var i = 0; i < _dimensionOverrides.Count; i++)
            {
                if (_dimensionOverrides[i].IsMatch(dimension))
                {
                    return _dimensionOverrides[i];
                }
            }

            return null;
        }
    }
}