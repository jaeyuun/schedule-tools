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
    }
}