namespace ScheduleTools.Core.IO
{
    public sealed class ApplicationStoragePaths
    {
        public const string SettingsDirectoryName = "Settings";
        public const string TemplatesDirectoryName = "Templates";

        public string ApplicationDataDirectory { get; }
        public string DocumentsDirectory { get; }
        public string SettingsDirectory { get; }
        public string TemplatesDirectory { get; }

        public ApplicationStoragePaths(string companyName, string applicationName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException("회사 폴더 이름이 비어 있습니다.", nameof(companyName));

            if (string.IsNullOrWhiteSpace(applicationName))
                throw new ArgumentException("애플리케이션 폴더 이름이 비어 있습니다.", nameof(applicationName));

            ApplicationDataDirectory = PathHelper.GetApplicationDataDirectory(companyName, applicationName);
            DocumentsDirectory = PathHelper.GetDocumentsDirectory(applicationName);
            SettingsDirectory = Path.Combine(ApplicationDataDirectory, SettingsDirectoryName);
            TemplatesDirectory = Path.Combine(ApplicationDataDirectory, TemplatesDirectoryName);
        }
    }
}
