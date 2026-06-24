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
            return item == null || item.TopRebar == null || item.TopRebar.FirstLayer == null || item.TopRebar.FirstLayer.Count < 0 ? 0 : item.TopRebar.FirstLayer.Count;
        }

        public int GetTopSecondCount(ScheduleItem item)
        {
            return item == null || item.TopRebar == null || item.TopRebar.SecondLayer == null || item.TopRebar.SecondLayer.Count < 0 ? 0 : item.TopRebar.SecondLayer.Count;
        }

        public int GetBottomFirstCount(ScheduleItem item)
        {
            return item == null || item.BottomRebar == null || item.BottomRebar.FirstLayer == null || item.BottomRebar.FirstLayer.Count < 0 ? 0 : item.BottomRebar.FirstLayer.Count;
        }

        public int GetBottomSecondCount(ScheduleItem item)
        {
            return item == null || item.BottomRebar == null || item.BottomRebar.SecondLayer == null || item.BottomRebar.SecondLayer.Count < 0 ? 0 : item.BottomRebar.SecondLayer.Count;
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