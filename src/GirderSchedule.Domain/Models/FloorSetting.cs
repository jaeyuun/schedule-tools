namespace GirderSchedule.Domain.Models
{
    public sealed class FloorSetting
    {
        public string FloorPrefix { get; set; } = string.Empty;
        public string FloorName { get; set; } = string.Empty;
        public int MainRebarDiameter { get; set; }
        public int StirrupDiameter { get; set; }
        public int SkinRebarDiameter { get; set; }
    }
}