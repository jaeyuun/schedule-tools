using ScheduleTools.Wpf.Home.Models;

namespace ScheduleTools.Wpf.Home.Contracts
{
    public interface IProjectHomeHandler
    {
        string ProjectDialogTitle { get; }
        string ProjectFilter { get; }
        string DefaultExtension { get; }
        HomeProjectLaunchResult CreateProject();
        HomeProjectLaunchResult OpenProject();
        HomeProjectLaunchResult OpenProject(string filePath);
        void ShowMainWindow(HomeProjectLaunchResult result);
    }
}