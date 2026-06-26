namespace GirderSchedule.Domain.Models
{
    public sealed class FloorSetting
    {
        public string FloorPrefix { get; set; } = string.Empty;
        public string FloorName { get; set; } = string.Empty;
        public double MainRebarDiameter { get; set; }
        public double StirrupDiameter { get; set; }
        public double SkinRebarDiameter { get; set; }
    }
}