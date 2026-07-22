using netDxf;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Styles.Overrides
{
    public sealed class DxfOverrideSet
    {
        private readonly List<DxfDimensionOverride> _dimensionOverrides = new List<DxfDimensionOverride>();
        private readonly List<DxfBlockInsertOverride> _blockInsertOverrides = new List<DxfBlockInsertOverride>();

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

            if (entity is Insert insert)
            {
                _blockInsertOverrides.Add(new DxfBlockInsertOverride(insert));
            }
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
                return;
            }

            DxfDimensionOverride.ApplyDefault(dimension);
        }

        public void ApplyBlockInsertOverride(EntityObject entity, double defaultScale, double defaultRotation)
        {
            var insert = entity as Insert;

            if (insert == null)
            {
                return;
            }

            var overrideItem = FindBlockInsertOverride(insert);

            if (overrideItem != null)
            {
                overrideItem.Apply(insert);
                return;
            }

            DxfBlockInsertOverride.ApplyDefault(insert, defaultScale, defaultRotation);
        }

        private DxfDimensionOverride? FindDimensionOverride(Dimension dimension)
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

        private DxfBlockInsertOverride? FindBlockInsertOverride(Insert insert)
        {
            for (var i = 0; i < _blockInsertOverrides.Count; i++)
            {
                if (_blockInsertOverrides[i].IsMatch(insert))
                {
                    return _blockInsertOverrides[i];
                }
            }

            return null;
        }
    }
}
