namespace ScheduleTools.Dxf.Geometry
{
    public class DxfBox
    {
        public double Left { get; }
        public double Top { get; }
        public double Right { get; }
        public double Bottom { get; }

        public double Width => Right - Left;
        public double Height => Top - Bottom;
        public double CenterX => (Left + Right) / 2.0;
        public double CenterY => (Top + Bottom) / 2.0;

        public DxfBox(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public DxfBox Move(double offsetX, double offsetY)
        {
            return new DxfBox(Left + offsetX, Top + offsetY, Right + offsetX, Bottom + offsetY);
        }
    }
}
