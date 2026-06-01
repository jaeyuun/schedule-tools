namespace GirderSchedule.Domain.Girder.Models
{
    public sealed class MemberForceData
    {
        public double? Moment { get; set; }
        public double? Shear { get; set; }

        public MemberForceData()
        {
            Moment = null;
            Shear = null;
        }
    }
}