namespace GirderSchedule.Excel.Girder
{
    public static class Layout
    {
        public const int StartColumn = 1;
        public const int EndColumn = 20;

        public const int HeaderRow1 = 1;
        public const int HeaderRow2 = 2;
        public const int FirstDataRow = 3;

        public const int RowsPerSet = 5;

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

        public static int GetSetEndRow(int startRow)
        {
            return startRow + RowsPerSet - 1;
        }
    }
}