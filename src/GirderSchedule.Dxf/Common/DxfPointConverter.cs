using netDxf;

namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfPointConverter
    {
        private readonly DxfLayout _layout;

        public DxfPointConverter(DxfLayout layout)
        {
            _layout = layout;
        }

        public Vector2 ToVector2(double x, double y)
        {
            return new Vector2(_layout.ToWorldX(x), _layout.ToWorldY(y));
        }

        public Vector3 ToVector3(double x, double y)
        {
            return new Vector3(_layout.ToWorldX(x), _layout.ToWorldY(y), 0.0);
        }
    }
}