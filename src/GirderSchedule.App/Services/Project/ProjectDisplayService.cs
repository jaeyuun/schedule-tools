using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.Services.Project
{
    public sealed class ProjectDisplayService
    {
        private const string DefaultScheduleTitle = "보 일람표";
        private const string DefaultProjectName = "새 프로젝트";
        private const string DirtyMark = " *";

        public string GetWindowTitle(ScheduleProject project, bool isDirty, string filePath)
        {
            var scheduleTitle = GetScheduleTitle(project);
            var projectName = GetProjectName(project);
            var dirty = isDirty || string.IsNullOrWhiteSpace(filePath) ? DirtyMark : string.Empty;

            return $"{scheduleTitle} - {projectName} {dirty}";
        }

        public string GetProjectDisplayName(ScheduleProject project, bool isDirty)
        {
            var projectName = GetProjectName(project);
            var dirty = isDirty ? DirtyMark : string.Empty;

            return projectName + dirty;
        }

        private string GetScheduleTitle(ScheduleProject project)
        {
            if (project == null || string.IsNullOrWhiteSpace(project.ScheduleTitle))
            {
                return DefaultScheduleTitle;
            }

            return project.ScheduleTitle;
        }

        private string GetProjectName(ScheduleProject project)
        {
            if (project == null || string.IsNullOrWhiteSpace(project.ProjectName))
            {
                return DefaultProjectName;
            }

            return project.ProjectName;
        }
    }
}