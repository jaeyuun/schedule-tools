namespace GirderSchedule.Domain.Models
{
    public sealed class RebarSet
    {
        public double Diameter { get; set; }
        public RebarLayer FirstLayer { get; set; }
        public RebarLayer SecondLayer { get; set; }

        public RebarSet()
        {
            FirstLayer = new RebarLayer();
            SecondLayer = new RebarLayer();
        }
    }
}