namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfBox
    {
        public double Left { get; private set; }
        public double Top { get; private set; }
        public double Right { get; private set; }
        public double Bottom { get; private set; }

        public double Width
        {
            get { return Right - Left; }
        }

        public double Height
        {
            get { return Top - Bottom; }
        }

        public double CenterX
        {
            get { return (Left + Right) / 2.0; }
        }

        public double CenterY
        {
            get { return (Top + Bottom) / 2.0; }
        }

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