using GirderSchedule.App.Constants;
using GirderSchedule.App.Home;
using GirderSchedule.App.Services.Export;
using GirderSchedule.App.Services.Project;
using GirderSchedule.App.Services.Schedule;
using ScheduleTools.Core.Application.Contracts;
using ScheduleTools.Core.Application.Services;
using ScheduleTools.Core.RecentProjects;
using ScheduleTools.Core.Settings;
using ScheduleTools.Wpf.FileDialogs.Contracts;
using ScheduleTools.Wpf.FileDialogs.Services;
using ScheduleTools.Wpf.Home.Contracts;
using ScheduleTools.Wpf.Home.Models;
using ScheduleTools.Wpf.Home.ViewModels;
using ScheduleTools.Wpf.Services;
using ScheduleTools.Wpf.Settings.ViewModels;
using ScheduleTools.Wpf.Windows.Contracts;
using ScheduleTools.Wpf.Windows.Services;
using System.Reflection;

namespace GirderSchedule.App.Infrastructure
{
    public static class AppServices
    {
        public static AppSettingService AppSetting { get; } = CreateAppSettingService();
        public static DxfSettingService DxfSetting { get; } = new DxfSettingService(AppStoragePaths.CreateDxfStorageOptions());
        public static ProjectFileService ProjectFile { get; } = new ProjectFileService(DxfSetting);
        public static ScheduleProjectFactory ProjectFactory { get; } = new ScheduleProjectFactory();
        public static RecentProjectService RecentProject { get; } = new RecentProjectService(AppStoragePaths.SettingsDirectory);
        public static IWindowService WindowService { get; } = new WindowService();
        public static IFileDialogService FileDialogService { get; } = new FileDialogService();
        public static IFolderDialogService FolderDialogService { get; } = new FolderDialogService();
        public static IApplicationInfoService ApplicationInfo { get; } = new ApplicationInfoService(Assembly.GetExecutingAssembly());
        public static IProjectHomeHandler ProjectHomeHandler { get; } = CreateGirderProjectHomeHandler();

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
            return new ProjectCreateWindowViewModel(AppSetting, FolderDialogService);
        }

        public static AppSettingWindowViewModel CreateAppSettingWindowViewModel()
        {
            return new AppSettingWindowViewModel(
                AppSetting,
                ApplicationInfo.InformationalVersion);
        }

        public static ProjectService CreateProjectService()
        {
            return new ProjectService(ProjectFile, RecentProject, AppSetting, ProjectFactory, WindowService, FileDialogService, FolderDialogService);
        }

        public static ScheduleExportService CreateScheduleExportService()
        {
            return new ScheduleExportService(DxfSetting);
        }

        public static GirderProjectHomeHandler CreateGirderProjectHomeHandler()
        {
            return new GirderProjectHomeHandler(ProjectFile, ProjectFactory, FileDialogService, WindowService);
        }

        private static AppSettingService CreateAppSettingService()
        {
            var options = new AppSettingOptions(
                AppStoragePaths.AppSettingFilePath,
                AppStoragePaths.DocumentsDirectory);

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
