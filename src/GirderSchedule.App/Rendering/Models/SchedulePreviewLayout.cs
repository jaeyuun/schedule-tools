namespace GirderSchedule.App.Rendering.Models
{
    public static class SchedulePreviewLayout
    {
        public const double CanvasWidth = 1420.0;
        public const double CanvasHeight = 960.0;

        public const double PageX = 30.0;
        public const double PageY = 30.0;
        public const double PageWidth = 1360.0;
        public const double PageHeight = 900.0;

        public const double TitleHeight = 48.0;

        public const double CellWidth = 430.0;
        public const double CellHeight = 270.0;

        public const int MaxSetCount = 9;
        public const int ColumnCount = 3;

        public static int RowCount
        {
            get { return (MaxSetCount + ColumnCount - 1) / ColumnCount; }
        }

        public static double TableWidth
        {
            get { return CellWidth * ColumnCount; }
        }

        public static double TableHeight
        {
            get { return CellHeight * RowCount; }
        }

        public static double ContentHeight
        {
            get { return PageHeight - TitleHeight; }
        }

        public static double CellStartOffsetX
        {
            get { return (PageWidth - TableWidth) / 2.0; }
        }

        public static double CellStartOffsetY
        {
            get { return TitleHeight + (ContentHeight - TableHeight) / 2.0; }
        }

        public static double GetCellX(int column)
        {
            return PageX + CellStartOffsetX + column * CellWidth;
        }

        public static double GetCellY(int row)
        {
            return PageY + CellStartOffsetY + row * CellHeight;
        }
    }
}