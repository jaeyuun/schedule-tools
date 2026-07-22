namespace ScheduleTools.Core.IO
{
    public static class PathHelper
    {
        public static string GetDefaultProjectDirectory(string applicationFolderName, string projectFolderName)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (string.IsNullOrWhiteSpace(documentsPath))
            {
                documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }

            return Path.Combine(documentsPath, applicationFolderName, projectFolderName);
        }
    }
}