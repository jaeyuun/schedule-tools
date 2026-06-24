namespace GirderSchedule.App.Previews.Models
{
    public sealed class PreviewSectionLayout
    {
        public double AreaWidth { get; set; }
        public double AreaHeight { get; set; }

        public double ShapeScale { get; set; }
        public double DimensionScale { get; set; }

        public double OuterLeft { get; set; }
        public double OuterRight { get; set; }
        public double OuterTop { get; set; }
        public double OuterShelfY { get; set; }
        public double OuterBottom { get; set; }

        public double SectionLeft { get; set; }
        public double SectionTop { get; set; }
        public double SectionBottom { get; set; }

        public double DrawWidth { get; set; }
        public double DrawHeight { get; set; }

        public double SideWing { get; set; }
        public double RebarCover { get; set; }
        public double SecondLayerGap { get; set; }

        public double BarStartX { get; set; }
        public double BarEndX { get; set; }

        public double TopBarY1 { get; set; }
        public double TopBarY2 { get; set; }
        public double BottomBarY1 { get; set; }
        public double BottomBarY2 { get; set; }
    }
}
