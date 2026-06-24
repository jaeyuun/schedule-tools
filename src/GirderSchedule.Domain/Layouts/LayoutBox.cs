namespace GirderSchedule.Domain.Layouts
{
    public sealed class LayoutBox
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public double Left
        {
            get { return X; }
        }

        public double Right
        {
            get { return X + Width; }
        }

        public double Top
        {
            get { return Y + Height; }
        }

        public double Bottom
        {
            get { return Y; }
        }

        public double CenterX
        {
            get { return X + Width / 2.0; }
        }

        public double CenterY
        {
            get { return Y + Height / 2.0; }
        }

        public LayoutBox(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public LayoutBox Offset(double x, double y)
        {
            return new LayoutBox(X + x, Y + y, Width, Height);
        }

        public LayoutBox Resize(double width, double height)
        {
            return new LayoutBox(X, Y, width, height);
        }

        public LayoutBox Inset(double left, double bottom, double right, double top)
        {
            return new LayoutBox(X + left, Y + bottom, Width - left - right, Height - bottom - top);
        }
    }
}