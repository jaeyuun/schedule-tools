using GirderSchedule.Domain.Models;

namespace GirderSchedule.Domain.Girder.Models
{
    public sealed class ScheduleItem
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string MomentText { get; set; }
        public string ShearText { get; set; }

        public SectionData Section { get; set; }
        public RebarSet TopRebar { get; set; }
        public RebarSet BottomRebar { get; set; }
        public StirrupData Stirrup { get; set; }

        public string SkinRebarText { get; set; }
        public string Note { get; set; }

        public ScheduleItem()
        {
            Name = string.Empty;
            Position = string.Empty;
            MomentText = string.Empty;
            ShearText = string.Empty;

            Section = new SectionData();
            TopRebar = new RebarSet();
            BottomRebar = new RebarSet();
            Stirrup = new StirrupData();

            SkinRebarText = string.Empty;
            Note = string.Empty;
        }
    }
}