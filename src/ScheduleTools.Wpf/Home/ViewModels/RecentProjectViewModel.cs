using ScheduleTools.Core.RecentProjects;
using System.IO;

namespace ScheduleTools.Wpf.Home.ViewModels
{
    public sealed class RecentProjectViewModel
    {
        public RecentProjectViewModel(RecentProjectInfo project)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
        }

        public RecentProjectInfo Project { get; }

        public string Name => string.IsNullOrWhiteSpace(Project.ProjectName)
            ? Path.GetFileNameWithoutExtension(Project.FilePath)
            : Project.ProjectName;

        public string FilePath => Project.FilePath;

        public string FolderPath => string.IsNullOrWhiteSpace(FilePath)
            ? string.Empty
            : Path.GetDirectoryName(FilePath) ?? string.Empty;

        public DateTime LastOpenedAt => Project.LastOpenedAt;

        public string LastOpenedText => GetLastOpenedText(LastOpenedAt);

        private static string GetLastOpenedText(DateTime lastOpenedAt)
        {
            var elapsed = DateTime.Now - lastOpenedAt;

            if (elapsed.TotalMinutes < 1)
                return "방금 전";

            if (elapsed.TotalHours < 1)
                return $"{Math.Max(1, (int)elapsed.TotalMinutes)}분 전";

            if (lastOpenedAt.Date == DateTime.Today)
                return $"{Math.Max(1, (int)elapsed.TotalHours)}시간 전";

            if (lastOpenedAt.Date == DateTime.Today.AddDays(-1))
                return "어제";

            return lastOpenedAt.ToString("yyyy.MM.dd");
        }
    }
}