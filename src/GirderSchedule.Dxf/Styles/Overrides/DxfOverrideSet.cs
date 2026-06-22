using GirderSchedule.Dxf.Constants;
using netDxf;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Styles.Overrides
{
    public sealed class DxfOverrideSet
    {
        private readonly List<DxfEntityOverride> _entityTemplates;
        private readonly List<DxfDimensionOverride> _dimensionTemplates;

        private DxfOverrideSet()
        {
            _entityTemplates = new List<DxfEntityOverride>();
            _dimensionTemplates = new List<DxfDimensionOverride>();
        }

        public static DxfOverrideSet Create(DxfDocument document)
        {
            var set = new DxfOverrideSet();

            if (document == null)
            {
                return set;
            }

            var entities = document.Entities.All.ToList();

            AddPreferredDimensionTemplates(set, entities);
            AddEntityTemplates(set, entities);

            return set;
        }

        public void Apply(EntityObject entity)
        {
            if (entity == null)
            {
                return;
            }

            var layerName = entity.Layer == null ? string.Empty : entity.Layer.Name;
            Apply(entity, layerName);
        }

        public void Apply(EntityObject entity, string templateLayerName)
        {
            if (entity == null)
            {
                return;
            }

            var dimension = entity as Dimension;

            if (dimension != null)
            {
                ApplyDimension(dimension, templateLayerName);
                return;
            }

            ApplyEntity(entity, templateLayerName);
        }

        private static void AddPreferredDimensionTemplates(DxfOverrideSet set, List<EntityObject> entities)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                var dimension = entities[i] as Dimension;

                if (dimension == null)
                {
                    continue;
                }

                if (dimension.Layer == null || dimension.Style == null)
                {
                    continue;
                }

                if (dimension.Layer.Name != DxfLayers.Dim)
                {
                    continue;
                }

                if (dimension.Style.Name != DxfStyles.Dimension)
                {
                    continue;
                }

                set.AddDimensionTemplate(dimension);
            }

            if (set._dimensionTemplates.Count > 0)
            {
                return;
            }

            for (var i = 0; i < entities.Count; i++)
            {
                var dimension = entities[i] as Dimension;

                if (dimension == null)
                {
                    continue;
                }

                if (dimension.Layer == null)
                {
                    continue;
                }

                if (dimension.Layer.Name != DxfLayers.Dim)
                {
                    continue;
                }

                set.AddDimensionTemplate(dimension);
            }
        }

        private static void AddEntityTemplates(DxfOverrideSet set, List<EntityObject> entities)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];

                if (entity == null || entity.Layer == null)
                {
                    continue;
                }

                var dimension = entity as Dimension;

                if (dimension != null)
                {
                    continue;
                }

                set.AddEntityTemplate(entity);
            }
        }

        private void ApplyEntity(EntityObject entity, string templateLayerName)
        {
            for (var i = 0; i < _entityTemplates.Count; i++)
            {
                if (!_entityTemplates[i].IsMatch(entity, templateLayerName))
                {
                    continue;
                }

                _entityTemplates[i].Apply(entity);
                return;
            }
        }

        private void ApplyDimension(Dimension dimension, string templateLayerName)
        {
            for (var i = 0; i < _dimensionTemplates.Count; i++)
            {
                if (!_dimensionTemplates[i].IsMatch(dimension, templateLayerName))
                {
                    continue;
                }

                _dimensionTemplates[i].Apply(dimension);
                return;
            }

            for (var i = 0; i < _dimensionTemplates.Count; i++)
            {
                if (_dimensionTemplates[i].LayerName != DxfLayers.Dim)
                {
                    continue;
                }

                _dimensionTemplates[i].Apply(dimension);
                return;
            }

            if (_dimensionTemplates.Count > 0)
            {
                _dimensionTemplates[0].Apply(dimension);
            }
        }

        private void AddEntityTemplate(EntityObject entity)
        {
            var template = new DxfEntityOverride(entity);

            for (var i = 0; i < _entityTemplates.Count; i++)
            {
                if (_entityTemplates[i].LayerName == template.LayerName && _entityTemplates[i].EntityTypeName == template.EntityTypeName)
                {
                    return;
                }
            }

            _entityTemplates.Add(template);
        }

        private void AddDimensionTemplate(Dimension dimension)
        {
            var template = new DxfDimensionOverride(dimension);

            for (var i = 0; i < _dimensionTemplates.Count; i++)
            {
                if (_dimensionTemplates[i].LayerName == template.LayerName && _dimensionTemplates[i].StyleName == template.StyleName)
                {
                    return;
                }
            }

            _dimensionTemplates.Add(template);
        }
    }
}