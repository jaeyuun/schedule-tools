namespace GirderSchedule.Dxf.Layouts
{
    public static class ScheduleTableLayout
    {
        public const int RowsPerPage = 3;

        public const double XrefToForm2X = 995.0;
        public const double XrefToForm2Y = 16975.0;
        public const double OriginX = XrefToForm2X;
        public const double OriginY = XrefToForm2Y;

        public const double FormWidth = 22710.0;
        public const double FormHeight = 5250.0;
        public const double RowHeight = FormHeight;
        public const double LabelWidth = 1200.0;

        public const double HeaderTopY = 0.0;
        public const double HeaderBottomY = -850.0;

        public const double SectionTopY = -850.0;
        public const double MemberForceTopY = -1100.0;
        public const double MemberForceBottomY = -1250.0;
        public const double SectionBottomY = -4050.0;

        public const double TopRebarTopY = -4050.0;
        public const double TopRebarBottomY = -4350.0;

        public const double BottomRebarTopY = -4350.0;
        public const double BottomRebarBottomY = -4650.0;

        public const double StirrupTopY = -4650.0;
        public const double StirrupBottomY = -4950.0;

        public const double SkinRebarTopY = -4950.0;
        public const double SkinRebarBottomY = -5250.0;

        public const double LabelTextX = 600.0;
        public const double MemberNameLabelTextY = -325.0;
        public const double SectionSizeLabelTextY = -575.0;
        public const double SectionLabelTextY = -2609.457;
        public const double TopRebarLabelTextY = -4200.0;
        public const double BottomRebarLabelTextY = -4500.0;
        public const double StirrupLabelTextY = -4800.0;
        public const double SkinRebarLabelTextY = -5100.0;
        public const double HeaderTextHeight = 120.0;
        public const double LabelTextHeight = 90.0;

        public static double GetBaseX(double pageBaseX)
        {
            return pageBaseX + OriginX;
        }

        public static double GetBaseY(double pageBaseY)
        {
            return pageBaseY + OriginY;
        }

        public static double GetRowBaseX(double pageBaseX, int rowIndex)
        {
            return GetBaseX(pageBaseX);
        }

        public static double GetRowBaseY(double pageBaseY, int rowIndex)
        {
            return GetBaseY(pageBaseY) + GetRowOffsetY(rowIndex);
        }

        public static double GetRowOffsetY(int rowIndex)
        {
            return -rowIndex * RowHeight;
        }
    }
}
