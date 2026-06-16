using System;

namespace GirderSchedule.App.Models
{
    public sealed class RecentProjectInfo
    {
        public string ProjectName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime LastOpenedAt { get; set; }
    }
}