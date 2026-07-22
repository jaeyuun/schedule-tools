using GirderSchedule.App.Constants;
using GirderSchedule.App.Home;
using GirderSchedule.App.Services.Project;
using GirderSchedule.App.Services.Schedule;
using ScheduleTools.Core.RecentProjects;
using ScheduleTools.Core.Settings;
using ScheduleTools.Wpf.Home.Contracts;
using ScheduleTools.Wpf.Home.Models;
using ScheduleTools.Wpf.Home.ViewModels;
using ScheduleTools.Wpf.ProjectCreate.ViewModels;
using ScheduleTools.Wpf.Services;
using ScheduleTools.Wpf.Settings.ViewModels;
using System.Reflection;

namespace GirderSchedule.App.Infrastructure
{
    public static class AppServices
    {
        public static AppSettingService AppSetting { get; } = CreateAppSettingService();

        public static ProjectFileService ProjectFile { get; } = new ProjectFileService();

        public static ScheduleProjectFactory ProjectFactory { get; } = new ScheduleProjectFactory();

        public static RecentProjectService RecentProject { get; } =
            new RecentProjectService(FileConstants.RecentProjectPath);

        public static IProjectHomeHandler ProjectHomeHandler { get; } =
            new GirderProjectHomeHandler(ProjectFile, ProjectFactory);

        public static HomeStartViewModel CreateHomeStartViewModel()
        {
            return new HomeStartViewModel(
                ProjectHomeHandler,
                RecentProject,
                CreateHomeTextOptions(),
                ShowNotice);
        }

        public static ProjectCreateWindowViewModel CreateProjectCreateWindowViewModel()
        {
            return new ProjectCreateWindowViewModel(AppSetting);
        }

        public static AppSettingWindowViewModel CreateAppSettingWindowViewModel()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "정보 없음";

            return new AppSettingWindowViewModel(AppSetting, version);
        }

        private static AppSettingService CreateAppSettingService()
        {
            var options = new AppSettingOptions(
                FileConstants.AppSettingFilePath,
                FileConstants.ApplicationFolderName,
                FileConstants.ProjectFolderName);

            return new AppSettingService(options);
        }

        private static HomeTextOptions CreateHomeTextOptions()
        {
            return new HomeTextOptions
            {
                WindowTitle = "보 일람표 툴"
            };
        }

        private static void ShowNotice(string title, string message)
        {
            DialogService.ShowNotice(title, message);
        }
    }
}