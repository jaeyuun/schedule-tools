namespace GirderSchedule.Dxf.Layouts
{
    public static class PageLayout
    {
        public const int DefaultColumnCount = 3;

        public const double FrameScale = 65.0;
        public const double FrameSourceWidth = 420.0;
        public const double FrameSourceHeight = 297.0;
        public const double FrameWidth = FrameSourceWidth * FrameScale;
        public const double FrameHeight = FrameSourceHeight * FrameScale;
        public const double SpacingX = FrameWidth + 1200.0;
        public const double SpacingY = FrameHeight + 1200.0;

        public static int NormalizeColumnCount(int columnCount)
        {
            return columnCount < 1 ? DefaultColumnCount : columnCount;
        }

        public static double GetBaseX(int pageIndex, int columnCount)
        {
            columnCount = NormalizeColumnCount(columnCount);
            return pageIndex % columnCount * SpacingX;
        }

        public static double GetBaseY(int pageIndex, int columnCount)
        {
            columnCount = NormalizeColumnCount(columnCount);
            return -(pageIndex / columnCount) * SpacingY;
        }
    }
}
