using System.Collections.Generic;

namespace GirderSchedule.Domain.Models.Export
{
    public sealed class ScheduleExportPage
    {
        public string Title { get; set; } = string.Empty;
        public List<ScheduleSet?> Sets { get; set; } = new List<ScheduleSet?>();
    }
}
