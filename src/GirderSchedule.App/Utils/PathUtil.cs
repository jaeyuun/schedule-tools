using System;
using System.IO;

namespace GirderSchedule.App.Utils
{
    public static class PathUtil
    {
        public static string GetDownloadsDirectory()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (string.IsNullOrWhiteSpace(userProfile))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            var downloads = Path.Combine(userProfile, "Downloads");

            if (Directory.Exists(downloads))
            {
                return downloads;
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public static string GetDefaultExcelFileName(string projectName)
        {
            var name = string.IsNullOrWhiteSpace(projectName) ? "보 일람표" : projectName;
            return name + "_보일람표.xlsx";
        }

        public static string GetDefaultExcelPath(string projectName)
        {
            return Path.Combine(GetDownloadsDirectory(), GetDefaultExcelFileName(projectName));
        }
    }
}