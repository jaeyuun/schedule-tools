namespace GirderSchedule.Domain.Models
{
    public sealed class FloorSetting
    {
        public string Prefix { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double MainRebarDiameter { get; set; }
        public double StirrupDiameter { get; set; }
        public double SkinRebarDiameter { get; set; }
    }
}