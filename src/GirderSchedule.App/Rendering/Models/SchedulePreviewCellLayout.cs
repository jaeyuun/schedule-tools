namespace GirderSchedule.App.Rendering.Models
{
    public sealed class SchedulePreviewCellLayout
    {
        public SchedulePreviewCellLayout(double x, double y, double width, double height)
            : this(x, y, width, height, true)
        {
        }

        public SchedulePreviewCellLayout(double x, double y, double width, double height, bool showLabels)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ShowLabels = showLabels;

            LabelWidth = showLabels ? 58.0 : 0.0;
            ContentWidth = width - LabelWidth;
            PartWidth = ContentWidth / 3.0;

            NameHeight = 36.0;
            TextRowHeight = 16.0;
            PartHeaderHeight = TextRowHeight;
            ForceHeight = TextRowHeight;
            SectionHeight = height - NameHeight - TextRowHeight * 6.0;

            SectionTotalHeight = PartHeaderHeight + ForceHeight + SectionHeight;

            NameY = y;
            SectionAreaY = NameY + NameHeight;
            ForceY = SectionAreaY + PartHeaderHeight;
            SectionY = ForceY + ForceHeight;
            TopRebarY = SectionAreaY + SectionTotalHeight;
            BottomRebarY = TopRebarY + TextRowHeight;
            StirrupY = BottomRebarY + TextRowHeight;
            SkinRebarY = StirrupY + TextRowHeight;
            EndY = y + height;

            LabelEndX = x + LabelWidth;
            LeftX = LabelEndX;
            CenterX = LabelEndX + PartWidth;
            RightX = LabelEndX + PartWidth * 2.0;
        }

        public bool ShowLabels { get; private set; }

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public double LabelWidth { get; private set; }
        public double ContentWidth { get; private set; }
        public double PartWidth { get; private set; }

        public double NameHeight { get; private set; }
        public double PartHeaderHeight { get; private set; }
        public double ForceHeight { get; private set; }
        public double SectionHeight { get; private set; }
        public double SectionTotalHeight { get; private set; }
        public double TextRowHeight { get; private set; }

        public double NameY { get; private set; }
        public double SectionAreaY { get; private set; }
        public double ForceY { get; private set; }
        public double SectionY { get; private set; }
        public double TopRebarY { get; private set; }
        public double BottomRebarY { get; private set; }
        public double StirrupY { get; private set; }
        public double SkinRebarY { get; private set; }
        public double EndY { get; private set; }

        public double LabelEndX { get; private set; }
        public double LeftX { get; private set; }
        public double CenterX { get; private set; }
        public double RightX { get; private set; }
    }
}
