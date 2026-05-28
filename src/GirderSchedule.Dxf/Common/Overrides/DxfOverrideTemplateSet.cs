using System.Collections.Generic;
using System.Linq;
using netDxf;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Common.Overrides
{
    public sealed class DxfOverrideTemplateSet
    {
        private readonly List<DxfEntityOverrideTemplate> _entityTemplates;
        private readonly List<DxfDimensionOverrideTemplate> _dimensionTemplates;

        private DxfOverrideTemplateSet()
        {
            _entityTemplates = new List<DxfEntityOverrideTemplate>();
            _dimensionTemplates = new List<DxfDimensionOverrideTemplate>();
        }

        public static DxfOverrideTemplateSet Create(DxfDocument document)
        {
            var set = new DxfOverrideTemplateSet();

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

            var dimension = entity as Dimension;

            if (dimension != null)
            {
                ApplyDimension(dimension);
                return;
            }

            ApplyEntity(entity);
        }

        private static void AddPreferredDimensionTemplates(DxfOverrideTemplateSet set, List<EntityObject> entities)
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

        private static void AddEntityTemplates(DxfOverrideTemplateSet set, List<EntityObject> entities)
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

        private void ApplyEntity(EntityObject entity)
        {
            for (var i = 0; i < _entityTemplates.Count; i++)
            {
                if (!_entityTemplates[i].IsMatch(entity))
                {
                    continue;
                }

                _entityTemplates[i].Apply(entity);
                return;
            }
        }

        private void ApplyDimension(Dimension dimension)
        {
            for (var i = 0; i < _dimensionTemplates.Count; i++)
            {
                if (!_dimensionTemplates[i].IsMatch(dimension))
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
            var template = new DxfEntityOverrideTemplate(entity);

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
            var template = new DxfDimensionOverrideTemplate(dimension);

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