namespace GirderSchedule.Domain.Girder.Layout
{
    public sealed class CellBox
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public double CenterX
        {
            get { return X + Width / 2.0; }
        }

        public double CenterY
        {
            get { return Y + Height / 2.0; }
        }

        public CellBox(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}