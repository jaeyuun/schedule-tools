namespace GirderSchedule.Domain.Girder.Models
{
    public sealed class RebarSet
    {
        public int Diameter { get; set; }

        public RebarLayer FirstLayer { get; set; }
        public RebarLayer SecondLayer { get; set; }

        public RebarSet()
        {
            Diameter = 19;
            FirstLayer = new RebarLayer();
            SecondLayer = new RebarLayer();
        }
    }
}