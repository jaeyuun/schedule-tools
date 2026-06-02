using System.Collections.Generic;

namespace GirderSchedule.Domain.Girder.Models
{
    public sealed class ScheduleRow
    {
        public List<ScheduleSet> Sets { get; private set; }

        public ScheduleRow()
        {
            Sets = new List<ScheduleSet>();
        }
    }
}