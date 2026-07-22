namespace ScheduleTools.Core.IO
{
    public static class PathHelper
    {
        public static string GetDocumentsDirectory()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!string.IsNullOrWhiteSpace(documentsPath))
                return documentsPath;

            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        public static string GetLocalApplicationDataDirectory()
        {
            var localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (!string.IsNullOrWhiteSpace(localApplicationDataPath))
                return localApplicationDataPath;

            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        public static string GetApplicationDataDirectory(params string[] folderNames)
        {
            ArgumentNullException.ThrowIfNull(folderNames);

            return Path.Combine(
                new[] { GetLocalApplicationDataDirectory() }
                    .Concat(folderNames)
                    .ToArray());
        }

        public static string GetDocumentsDirectory(params string[] folderNames)
        {
            ArgumentNullException.ThrowIfNull(folderNames);

            return Path.Combine(
                new[] { GetDocumentsDirectory() }
                    .Concat(folderNames)
                    .ToArray());
        }
    }
}