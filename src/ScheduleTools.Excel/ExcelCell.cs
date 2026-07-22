using ClosedXML.Excel;

namespace ScheduleTools.Excel
{
    public static class ExcelCell
    {
        public static void SetMergedValue(IXLWorksheet sheet, int startRow, int column, int endRow, XLCellValue value)
        {
            ArgumentNullException.ThrowIfNull(sheet);
            sheet.Range(startRow, column, endRow, column).Merge().Value = value;
        }
    }
}
