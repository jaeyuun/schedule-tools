namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfLayout
    {
        public const double OriginX = 310378.9119;
        public const double OriginY = 34880.5467;

        public const double SheetWidth = 27300.0;
        public const double SheetHeight = 19305.0;

        public double ToWorldX(double x)
        {
            return OriginX + x;
        }

        public double ToWorldY(double y)
        {
            return OriginY + y;
        }

        public double ToLocalX(double worldX)
        {
            return worldX - OriginX;
        }

        public double ToLocalY(double worldY)
        {
            return worldY - OriginY;
        }
    }
}