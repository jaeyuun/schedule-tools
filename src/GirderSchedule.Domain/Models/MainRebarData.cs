namespace GirderSchedule.Domain.Models
{
    public class MainRebarData
    {
        public double Diameter { get; set; }
        public RebarSet Top { get; set; } = new RebarSet();
        public RebarSet Bottom { get; set; } = new RebarSet();
    }
}
