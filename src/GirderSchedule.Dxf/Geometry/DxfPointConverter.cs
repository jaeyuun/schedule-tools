using GirderSchedule.Dxf.Layouts;
using netDxf;

namespace GirderSchedule.Dxf.Geometry
{
    public static class DxfPointConverter
    {
        public static Vector2 ToVector2(double x, double y)
        {
            return new Vector2(DxfLayout.OriginX + x, DxfLayout.OriginY + y);
        }

        public static Vector3 ToVector3(double x, double y)
        {
            return new Vector3(DxfLayout.OriginX + x, DxfLayout.OriginY + y, 0.0);
        }
    }
}