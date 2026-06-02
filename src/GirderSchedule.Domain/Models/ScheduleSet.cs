namespace GirderSchedule.Domain.Girder.Models
{
    public sealed class ScheduleSet
    {
        public string MemberName { get; set; }

        public ScheduleItem Left { get; set; }
        public ScheduleItem Center { get; set; }
        public ScheduleItem Right { get; set; }

        public ScheduleSet()
        {
            MemberName = string.Empty;
        }

        public ScheduleItem GetItem(ScheduleSlotType type)
        {
            if (type == ScheduleSlotType.Left)
            {
                return Left;
            }

            if (type == ScheduleSlotType.Center)
            {
                return Center;
            }

            return Right;
        }
    }
}