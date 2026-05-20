using GirderSchedule.Domain.Models;
using System.Collections.Generic;

namespace GirderSchedule.Domain.Girder.Models
{
    public sealed class ScheduleRow
    {
        public List<ScheduleItem> Items { get; private set; }

        public ScheduleRow()
        {
            Items = new List<ScheduleItem>();
        }
    }
}