using netDxf;

namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfPointConverter
    {
        public Vector2 ToVector2(double x, double y)
        {
            return new Vector2(DxfLayout.OriginX + x, DxfLayout.OriginY + y);
        }

        public Vector3 ToVector3(double x, double y)
        {
            return new Vector3(DxfLayout.OriginX + x, DxfLayout.OriginY + y, 0.0);
        }
    }
}