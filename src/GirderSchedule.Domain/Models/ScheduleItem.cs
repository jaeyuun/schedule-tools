namespace GirderSchedule.Domain.Girder.Models
{
    public sealed class ScheduleItem
    {
        public string Name { get; set; }
        public string Position { get; set; }

        public SectionData Section { get; set; }
        public RebarSet TopRebar { get; set; }
        public RebarSet BottomRebar { get; set; }
        public StirrupData Stirrup { get; set; }
        public MemberForceData MemberForce { get; set; }

        public string SkinRebarText { get; set; }
        public string Note { get; set; }

        public ScheduleItem()
        {
            Name = string.Empty;
            Position = string.Empty;

            Section = new SectionData();
            TopRebar = new RebarSet();
            BottomRebar = new RebarSet();
            Stirrup = new StirrupData();
            MemberForce = new MemberForceData();

            SkinRebarText = string.Empty;
            Note = string.Empty;
        }
    }
}