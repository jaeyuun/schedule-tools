namespace GirderSchedule.Dxf.Common
{
    public static class DxfLayout
    {
        public const double OriginX = 310378.9119;
        public const double OriginY = 34880.5467;

        public const double SheetWidth = 22710.0;
        public const double SheetHeight = 5250.0;

        public const double LabelWidth = 1200.0;
        public const double SectionGroupWidth = 7170.0;

        public const double HeaderTopY = 0.0;
        public const double HeaderBottomY = -850.0;

        public const double SectionTopY = -850.0;
        public const double SectionBottomY = -4050.0;

        public const double TopRebarTopY = -4050.0;
        public const double TopRebarBottomY = -4350.0;

        public const double BottomRebarTopY = -4350.0;
        public const double BottomRebarBottomY = -4650.0;

        public const double StirrupTopY = -4650.0;
        public const double StirrupBottomY = -4950.0;

        public const double SkinTopY = -4950.0;
        public const double SkinBottomY = -5250.0;

        public static DxfBox GetGroupBox(int index)
        {
            var left = LabelWidth + SectionGroupWidth * index;

            return new DxfBox(
                left,
                HeaderTopY,
                left + SectionGroupWidth,
                SkinBottomY);
        }

        public static DxfBox GetHeaderBox(int index)
        {
            var group = GetGroupBox(index);
            return new DxfBox(group.Left, HeaderTopY, group.Right, HeaderBottomY);
        }

        public static DxfBox GetSectionBox(int index)
        {
            var group = GetGroupBox(index);
            return new DxfBox(group.Left, SectionTopY, group.Right, SectionBottomY);
        }

        public static DxfBox GetTopRebarBox(int index)
        {
            var group = GetGroupBox(index);
            return new DxfBox(group.Left, TopRebarTopY, group.Right, TopRebarBottomY);
        }

        public static DxfBox GetBottomRebarBox(int index)
        {
            var group = GetGroupBox(index);
            return new DxfBox(group.Left, BottomRebarTopY, group.Right, BottomRebarBottomY);
        }

        public static DxfBox GetStirrupBox(int index)
        {
            var group = GetGroupBox(index);
            return new DxfBox(group.Left, StirrupTopY, group.Right, StirrupBottomY);
        }

        public static DxfBox GetSkinBox(int index)
        {
            var group = GetGroupBox(index);
            return new DxfBox(group.Left, SkinTopY, group.Right, SkinBottomY);
        }
    }
}