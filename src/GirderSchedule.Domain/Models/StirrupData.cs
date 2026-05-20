namespace GirderSchedule.Domain.Girder.Models
{
    public sealed class StirrupData
    {
        public int Legs { get; set; }
        public int Diameter { get; set; }
        public int Spacing { get; set; }
        public string ExtraText { get; set; }

        public StirrupData()
        {
            ExtraText = string.Empty;
        }
    }
}