using GirderSchedule.Domain.Models.Settings;
using System.Collections.Generic;

namespace GirderSchedule.Domain.Models
{
    public sealed class ScheduleFloor
    {
        public FloorSetting Setting { get; set; } = new FloorSetting();
        public List<ScheduleSet> Sets { get; set; } = new List<ScheduleSet>();
    }
}