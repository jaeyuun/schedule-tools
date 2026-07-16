using GirderSchedule.App.Constants;
using System;
using System.IO;

namespace GirderSchedule.App.Utils
{
    public static class PathUtil
    {
        public static string GetDefaultProjectDirectory()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (string.IsNullOrWhiteSpace(documentsPath))
            {
                documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }

            return Path.Combine(documentsPath, FileConstants.ApplicationFolderName, FileConstants.ProjectFolderName);
        }
    }
}