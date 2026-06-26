using GirderSchedule.Domain.Models;

namespace GirderSchedule.Domain.Services
{
    public sealed class ScheduleItemQuery
    {
        public ScheduleItem GetBaseItem(ScheduleSet set)
        {
            if (set == null)
            {
                return null;
            }

            if (set.Left != null && set.Left.Section != null)
            {
                return set.Left;
            }

            if (set.Center != null && set.Center.Section != null)
            {
                return set.Center;
            }

            if (set.Right != null && set.Right.Section != null)
            {
                return set.Right;
            }

            return null;
        }

        public ScheduleItem GetFirstEnabledItem(ScheduleSet set)
        {
            if (set == null)
            {
                return null;
            }

            if (set.Left != null && set.Left.IsSectionEnabled && set.Left.Section != null)
            {
                return set.Left;
            }

            if (set.Center != null && set.Center.IsSectionEnabled && set.Center.Section != null)
            {
                return set.Center;
            }

            if (set.Right != null && set.Right.IsSectionEnabled && set.Right.Section != null)
            {
                return set.Right;
            }

            return null;
        }

        public int GetTopFirstCount(ScheduleItem item)
        {
            return item == null || item.MainRebar == null || item.MainRebar.Top == null || item.MainRebar.Top.FirstCount < 0 ? 0 : item.MainRebar.Top.FirstCount;
        }

        public int GetTopSecondCount(ScheduleItem item)
        {
            return item == null || item.MainRebar == null || item.MainRebar.Top == null || item.MainRebar.Top.SecondCount < 0 ? 0 : item.MainRebar.Top.SecondCount;
        }

        public int GetBottomFirstCount(ScheduleItem item)
        {
            return item == null || item.MainRebar == null || item.MainRebar.Bottom == null || item.MainRebar.Bottom.FirstCount < 0 ? 0 : item.MainRebar.Bottom.FirstCount;
        }

        public int GetBottomSecondCount(ScheduleItem item)
        {
            return item == null || item.MainRebar == null || item.MainRebar.Bottom == null || item.MainRebar.Bottom.SecondCount < 0 ? 0 : item.MainRebar.Bottom.SecondCount;
        }

        public int GetTopTotalCount(ScheduleItem item)
        {
            return GetTopFirstCount(item) + GetTopSecondCount(item);
        }

        public int GetBottomTotalCount(ScheduleItem item)
        {
            return GetBottomFirstCount(item) + GetBottomSecondCount(item);
        }

        public int GetStirrupLegs(ScheduleItem item)
        {
            return item == null || item.Stirrup == null || item.Stirrup.Legs < 0 ? 0 : item.Stirrup.Legs;
        }
    }
}