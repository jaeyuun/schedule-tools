using System.Collections.Generic;

namespace GirderSchedule.Domain.Models
{
    public sealed class ScheduleProject
    {
        public string ProjectName { get; set; } = "새 프로젝트";
        public string ScheduleTitle { get; set; } = "보 일람표";
        public List<ScheduleFloor> Floors { get; set; } = new List<ScheduleFloor>();
    }
}