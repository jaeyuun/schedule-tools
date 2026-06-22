namespace GirderSchedule.Dxf.Layouts
{
    public sealed class SectionLayout
    {
        public double CellLeft { get; set; }
        public double CellRight { get; set; }
        public double CellTop { get; set; }
        public double CellBottom { get; set; }

        public double OuterLeft { get; set; }
        public double OuterRight { get; set; }
        public double OuterTop { get; set; }
        public double OuterShelfY { get; set; }
        public double OuterBottom { get; set; }

        public double SectionLeft { get; set; }
        public double SectionRight { get; set; }
        public double SectionTop { get; set; }
        public double SectionBottom { get; set; }

        public double SideWing { get; set; }

        public double BarStartX { get; set; }
        public double BarEndX { get; set; }

        public double TopBarY1 { get; set; }
        public double TopBarY2 { get; set; }
        public double BottomBarY1 { get; set; }
        public double BottomBarY2 { get; set; }

        public double DrawWidth { get; set; }
        public double DrawHeight { get; set; }
        public double Scale { get; set; }
    }
}