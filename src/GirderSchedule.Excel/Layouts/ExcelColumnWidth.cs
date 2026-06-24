namespace GirderSchedule.Excel.Layouts
{
    public sealed class ExcelColumnWidth
    {
        public ExcelColumnWidth(string columnName, double width)
        {
            ColumnName = columnName;
            Width = width;
        }

        public string ColumnName { get; private set; }

        public double Width { get; private set; }
    }
}