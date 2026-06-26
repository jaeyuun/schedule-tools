using GirderSchedule.Domain.Enums;

namespace GirderSchedule.Domain.Models
{
    public sealed class ScheduleSet
    {
        public string MemberName { get; set; } = string.Empty;

        public ScheduleItem Left { get; set; } = new ScheduleItem();
        public ScheduleItem Center { get; set; } = new ScheduleItem();
        public ScheduleItem Right { get; set; } = new ScheduleItem();

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