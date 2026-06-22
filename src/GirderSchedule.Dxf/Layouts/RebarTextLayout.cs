using GirderSchedule.Dxf.Geometry;

namespace GirderSchedule.Dxf.Layouts
{
    public static class RebarTextLayout
    {
        public const double TextHeight = 90.0;

        public const double MainRebarCountOffsetX = 1045.0;
        public const double MainRebarDashHdLabelOffsetX = 1080.0;
        public const double MainRebarDiameterOffsetX = 1395.0;

        public const double StirrupLegOffsetX = 795.0;
        public const double StirrupDashHdLabelOffsetX = 830.0;
        public const double StirrupDiameterOffsetX = 1215.0;
        public const double StirrupAtLabelOffsetX = 1330.0;
        public const double StirrupSpacingOffsetX = 1490.0;

        public const double SkinRebarTextOffsetX = 1210.0;

        public static double GetRowTextY(DxfBox box)
        {
            return box.CenterY - TextHeight * 0.5;
        }
    }
}
