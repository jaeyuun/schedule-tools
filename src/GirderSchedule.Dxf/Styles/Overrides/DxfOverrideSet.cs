using System.Collections.Generic;
using netDxf;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Styles.Overrides
{
    public sealed class DxfOverrideSet
    {
        private readonly List<DxfEntityOverride> _entityOverrides = new List<DxfEntityOverride>();
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

            var dimension = entity as Dimension;

            if (dimension != null)
            {
                _dimensionOverrides.Add(new DxfDimensionOverride(dimension));
                return;
            }

            _entityOverrides.Add(new DxfEntityOverride(entity));
        }

        public void Apply(EntityObject entity)
        {
            if (entity == null)
            {
                return;
            }

            var dimension = entity as Dimension;

            if (dimension != null && ApplyDimensionOverride(dimension))
            {
                return;
            }

            ApplyEntityOverride(entity);
        }

        public void Apply(EntityObject entity, string templateLayerName)
        {
            if (entity == null)
            {
                return;
            }

            var dimension = entity as Dimension;

            if (dimension != null && ApplyDimensionOverride(dimension, templateLayerName))
            {
                return;
            }

            ApplyEntityOverride(entity, templateLayerName);
        }

        private bool ApplyDimensionOverride(Dimension dimension)
        {
            var overrideItem = FindDimensionOverride(dimension);

            if (overrideItem == null)
            {
                return false;
            }

            overrideItem.Apply(dimension);
            return true;
        }

        private bool ApplyDimensionOverride(Dimension dimension, string templateLayerName)
        {
            var overrideItem = FindDimensionOverride(dimension, templateLayerName);

            if (overrideItem == null)
            {
                return false;
            }

            overrideItem.Apply(dimension);
            return true;
        }

        private void ApplyEntityOverride(EntityObject entity)
        {
            var overrideItem = FindEntityOverride(entity);

            if (overrideItem == null)
            {
                return;
            }

            overrideItem.Apply(entity);
        }

        private void ApplyEntityOverride(EntityObject entity, string templateLayerName)
        {
            var overrideItem = FindEntityOverride(entity, templateLayerName);

            if (overrideItem == null)
            {
                return;
            }

            overrideItem.Apply(entity);
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

        private DxfDimensionOverride FindDimensionOverride(Dimension dimension, string templateLayerName)
        {
            for (var i = 0; i < _dimensionOverrides.Count; i++)
            {
                if (_dimensionOverrides[i].IsMatch(dimension, templateLayerName))
                {
                    return _dimensionOverrides[i];
                }
            }

            return null;
        }

        private DxfEntityOverride FindEntityOverride(EntityObject entity)
        {
            for (var i = 0; i < _entityOverrides.Count; i++)
            {
                if (_entityOverrides[i].IsMatch(entity))
                {
                    return _entityOverrides[i];
                }
            }

            return null;
        }

        private DxfEntityOverride FindEntityOverride(EntityObject entity, string templateLayerName)
        {
            for (var i = 0; i < _entityOverrides.Count; i++)
            {
                if (_entityOverrides[i].IsMatch(entity, templateLayerName))
                {
                    return _entityOverrides[i];
                }
            }

            return null;
        }
    }
}