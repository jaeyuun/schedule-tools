using GirderSchedule.Domain.Models;
using System.Collections.Generic;

namespace GirderSchedule.Domain.Models
{
    public sealed class ScheduleExportPage
    {
        public string SheetTitle { get; set; } = string.Empty;
        public string FloorName { get; set; } = string.Empty;
        public string SheetTitleName { get; set; } = string.Empty;
        public List<ScheduleSet> Sets { get; set; } = new List<ScheduleSet>();
    }
}