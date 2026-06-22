namespace GirderSchedule.Domain.Layout
{
    public sealed class RebarPointLayout
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public RebarPointLayout(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}