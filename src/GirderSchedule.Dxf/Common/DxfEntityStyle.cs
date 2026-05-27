using netDxf;
using netDxf.Entities;
using netDxf.Tables;

namespace GirderSchedule.Dxf.Common
{
    public static class DxfEntityStyle
    {
        public static void ApplyByLayer(EntityObject entity, Layer layer)
        {
            entity.Layer = layer;
            entity.Color = AciColor.ByLayer;
            entity.Linetype = Linetype.ByLayer;
            entity.Lineweight = Lineweight.ByLayer;
        }

        public static void ApplyByLayer(EntityObject entity, Layer layer, Linetype linetype)
        {
            entity.Layer = layer;
            entity.Color = AciColor.ByLayer;
            entity.Linetype = linetype ?? Linetype.ByLayer;
            entity.Lineweight = Lineweight.ByLayer;
        }

        public static void Apply(EntityObject entity, Layer layer, AciColor color, Linetype linetype, Lineweight lineweight)
        {
            entity.Layer = layer;
            entity.Color = color ?? AciColor.ByLayer;
            entity.Linetype = linetype ?? Linetype.ByLayer;
            entity.Lineweight = lineweight;
        }

        public static void ApplyRcGir(EntityObject entity, DxfDocument document)
        {
            ApplyByLayer(entity, document.Layers[DxfLayers.RcGir], GetLinetype(document, DxfStyles.RcGirderLine));
        }

        public static void ApplyRebar(EntityObject entity, DxfDocument document)
        {
            ApplyByLayer(entity, document.Layers[DxfLayers.Rebar]);
        }

        public static void ApplyFormLine(EntityObject entity, DxfDocument document)
        {
            ApplyByLayer(entity, document.Layers[DxfLayers.FormLine]);
        }

        public static void ApplyDimension(EntityObject entity, DxfDocument document)
        {
            ApplyByLayer(entity, document.Layers[DxfLayers.Dim]);
        }

        public static Linetype GetLinetype(DxfDocument document, string name)
        {
            if (!string.IsNullOrEmpty(name) && document.Linetypes.Contains(name))
            {
                return document.Linetypes[name];
            }

            if (document.Linetypes.Contains(DxfStyles.Continuous))
            {
                return document.Linetypes[DxfStyles.Continuous];
            }

            return Linetype.Continuous;
        }
    }
}