using System.IO;

namespace GirderSchedule.Excel.Utils
{
    public static class ExcelFileUtil
    {
        public static void EnsureDirectory(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}