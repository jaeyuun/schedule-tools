using GirderSchedule.Excel.Constants;

namespace GirderSchedule.Excel.Utils
{
    public static class ExcelSheetNameUtil
    {
        public static string GetSheetName(ExcelExportOptions options)
        {
            var name = options == null || string.IsNullOrWhiteSpace(options.SheetName) ? ExcelLabels.DefaultSheetName : options.SheetName.Trim();

            name = name.Replace(":", string.Empty)
                       .Replace("\\", string.Empty)
                       .Replace("/", string.Empty)
                       .Replace("?", string.Empty)
                       .Replace("*", string.Empty)
                       .Replace("[", string.Empty)
                       .Replace("]", string.Empty);

            if (string.IsNullOrWhiteSpace(name))
            {
                return ExcelLabels.DefaultSheetName;
            }

            return name.Length > 31 ? name.Substring(0, 31) : name;
        }
    }
}