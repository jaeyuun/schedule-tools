namespace GirderSchedule.Dxf.Layouts
{
    public static class TitleBlockLayout
    {
        public const double BlockScale = PageLayout.FrameScale;

        public const double TitleCircleCenterX = 12025.0;
        public const double TitleCircleCenterY = 17645.0;
        public const double TitleCircleRadius = 325.0;

        public const double TitleNumberX = 12020.0;
        public const double TitleNumberY = 17645.0;
        public const double TitleNumberHeight = 180.0;

        public const double TitleLineStartX = TitleCircleCenterX + TitleCircleRadius;
        public const double TitleLineEndX = 14840.0;
        public const double TitleLineY = TitleCircleCenterY;

        public const double TitleTextX = 13650.0;
        public const double TitleTextY = 17840.0;
        public const double TitleTextHeight = 230.0;

        public const double DrawingNameTextX = 25350.0;
        public const double DrawingNameTextY = 1775.0;
        public const double DrawingNameTextHeight = 120.0;
    }
}
