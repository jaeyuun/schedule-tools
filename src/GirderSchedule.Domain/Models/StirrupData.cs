namespace GirderSchedule.Domain.Models
{
    public sealed class StirrupData
    {
        public int Legs { get; set; }
        public double Diameter { get; set; }
        public int Spacing { get; set; }
        public string ExtraText { get; set; }

        public StirrupData()
        {
            ExtraText = string.Empty;
        }
    }
}