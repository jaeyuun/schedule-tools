namespace ScheduleTools.Excel
{
    public static class ExcelFile
    {
        public static void EnsureDirectory(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
