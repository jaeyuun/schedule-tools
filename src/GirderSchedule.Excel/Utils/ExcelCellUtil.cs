using ClosedXML.Excel;

namespace GirderSchedule.Excel.Utils
{
    public static class ExcelCellUtil
    {
        public static void SetMergedValue(IXLWorksheet sheet, int startRow, int column, int endRow, XLCellValue value)
        {
            sheet.Range(startRow, column, endRow, column).Merge().Value = value;
        }
    }
}