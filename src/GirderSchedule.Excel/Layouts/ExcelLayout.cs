namespace GirderSchedule.Excel.Layouts
{
    public static class ExcelLayout
    {
        public const int FirstDataRow = 3;
        public const int RowsPerSet = 5;
        public const int EndColumn = 20;

        public const int ColumnId = 1;
        public const int ColumnName = 2;
        public const int ColumnSectionWidth = 3;
        public const int ColumnSectionSymbol = 4;
        public const int ColumnSectionHeight = 5;
        public const int ColumnRebarSection = 6;
        public const int ColumnCover = 7;
        public const int ColumnRebarType = 8;
        public const int ColumnRebarDiameter = 9;
        public const int ColumnLayerName = 10;
        public const int ColumnLayerNo = 11;

        public const int EndIStartColumn = 12;
        public const int EndIEndColumn = 14;
        public const int CenterStartColumn = 15;
        public const int CenterEndColumn = 17;
        public const int EndJStartColumn = 18;
        public const int EndJEndColumn = 20;

        public const double DefaultColumnWidth = 8.43;
        public const double HeaderRowHeight = 13.5;
        public const double DefaultRowHeight = 13.5;
        public const double FloorNameRowHeight = 20.0;

        public static readonly ExcelColumnWidth[] ColumnWidths =
        {
            new ExcelColumnWidth("A", DefaultColumnWidth),
            new ExcelColumnWidth("B", 13.0),
            new ExcelColumnWidth("C", 6.0),
            new ExcelColumnWidth("D", 3.0),
            new ExcelColumnWidth("E", 6.0),
            new ExcelColumnWidth("F", 16.0),
            new ExcelColumnWidth("G", 13.0),
            new ExcelColumnWidth("H", DefaultColumnWidth),
            new ExcelColumnWidth("I", DefaultColumnWidth),
            new ExcelColumnWidth("J", DefaultColumnWidth),
            new ExcelColumnWidth("K", DefaultColumnWidth),
            new ExcelColumnWidth("L", DefaultColumnWidth),
            new ExcelColumnWidth("M", 4.0),
            new ExcelColumnWidth("N", DefaultColumnWidth),
            new ExcelColumnWidth("O", DefaultColumnWidth),
            new ExcelColumnWidth("P", 4.0),
            new ExcelColumnWidth("Q", DefaultColumnWidth),
            new ExcelColumnWidth("R", DefaultColumnWidth),
            new ExcelColumnWidth("S", 4.0),
            new ExcelColumnWidth("T", DefaultColumnWidth)
        };

        public static int GetSetEndRow(int startRow)
        {
            return startRow + RowsPerSet - 1;
        }
    }
}