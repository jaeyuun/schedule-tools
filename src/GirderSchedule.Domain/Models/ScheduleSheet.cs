using GirderSchedule.Domain.Models;
using System.Collections.Generic;

namespace GirderSchedule.Domain.Girder.Models
{
    public sealed class ScheduleSheet
    {
        public string Title { get; set; }
        public List<ScheduleRow> Rows { get; private set; }

        public ScheduleSheet()
        {
            Title = "보 일람표";
            Rows = new List<ScheduleRow>();
        }
    }
}