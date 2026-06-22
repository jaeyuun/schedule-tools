using GirderSchedule.Domain.Models;
using System.Collections.Generic;

namespace GirderSchedule.Domain.Models
{
    public sealed class ScheduleFloor
    {
        public string Name { get; set; } = string.Empty;
        public FloorSetting Setting { get; set; } = new FloorSetting();
        public List<ScheduleSet> Sets { get; set; } = new List<ScheduleSet>();
    }
}