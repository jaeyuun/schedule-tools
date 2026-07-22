using System.Windows;

namespace ScheduleTools.Wpf.Home.Models
{
    public sealed class HomeProjectLaunchResult
    {
        public bool IsSuccess { get; }
        public bool IsCanceled { get; }
        public string ProjectName { get; }
        public string FilePath { get; }
        public Window? MainWindow { get; }
        public string ErrorMessage { get; }

        private HomeProjectLaunchResult(bool isSuccess, bool isCanceled, string projectName, string filePath, Window? mainWindow, string errorMessage)
        {
            IsSuccess = isSuccess;
            IsCanceled = isCanceled;
            ProjectName = projectName;
            FilePath = filePath;
            MainWindow = mainWindow;
            ErrorMessage = errorMessage;
        }

        public static HomeProjectLaunchResult Success(string projectName, string filePath, Window mainWindow)
        {
            ArgumentNullException.ThrowIfNull(mainWindow);

            return new HomeProjectLaunchResult(
                true,
                false,
                projectName ?? string.Empty,
                filePath ?? string.Empty,
                mainWindow,
                string.Empty);
        }

        public static HomeProjectLaunchResult Failure(string errorMessage = "")
        {
            return new HomeProjectLaunchResult(
                false,
                false,
                string.Empty,
                string.Empty,
                null,
                errorMessage ?? string.Empty);
        }

        public static HomeProjectLaunchResult Cancel()
        {
            return new HomeProjectLaunchResult(
                false,
                true,
                string.Empty,
                string.Empty,
                null,
                string.Empty);
        }
    }
}