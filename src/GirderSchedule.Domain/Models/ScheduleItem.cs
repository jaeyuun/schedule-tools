namespace GirderSchedule.Domain.Models
{
    public sealed class ScheduleItem
    {
        public string Position { get; set; } = string.Empty;
        public bool IsSectionEnabled { get; set; } = true;

        public SectionData Section { get; set; } = new SectionData();
        public MainRebarData MainRebar { get; set; } = new MainRebarData();
        public StirrupData Stirrup { get; set; } = new StirrupData();
        public MemberForceData MemberForce { get; set; } = new MemberForceData();
        public SkinRebarData SkinRebar { get; set; } = new SkinRebarData();
    }
}