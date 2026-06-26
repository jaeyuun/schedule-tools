namespace GirderSchedule.Excel.Layouts
{
    public sealed class ExcelColumnWidth
    {
        public string ColumnName { get; private set; }
        public double Width { get; private set; }

        public ExcelColumnWidth(string columnName, double width)
        {
            ColumnName = columnName;
            Width = width;
        }
    }
}