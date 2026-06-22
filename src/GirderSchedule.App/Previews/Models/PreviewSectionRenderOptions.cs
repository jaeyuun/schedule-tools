namespace GirderSchedule.App.Preview.Models
{
    public sealed class PreviewSectionRenderOptions
    {
        public static readonly PreviewSectionRenderOptions Editor = new PreviewSectionRenderOptions
        {
            ScaleMarginRatio = 0.90,
            BarRadius = 3.0,
            MinimumRebarCover = 4.0,
            MinimumSecondLayerGap = 7.0,
            OuterLineThickness = 1.5,
            StirrupBoxThickness = 1.0,
            StirrupLineThickness = 1.0,
            DimensionLineThickness = 1.0,
            DimensionMarkThickness = 1.0,
            DimensionGapMinimum = 24.0,
            DimensionExtensionMinimum = 8.0,
            DimensionTickMinimum = 4.0,
            DimensionCircleMinimum = 2.0,
            DimensionTextMinimum = 10.0
        };

        public static readonly PreviewSectionRenderOptions Export = new PreviewSectionRenderOptions
        {
            ScaleMarginRatio = 0.94,
            BarRadius = 2.2,
            MinimumRebarCover = 2.6,
            MinimumSecondLayerGap = 5.5,
            OuterLineThickness = 1.1,
            StirrupBoxThickness = 0.8,
            StirrupLineThickness = 0.8,
            DimensionLineThickness = 0.75,
            DimensionMarkThickness = 0.7,
            DimensionGapMinimum = 12.0,
            DimensionExtensionMinimum = 4.0,
            DimensionTickMinimum = 2.5,
            DimensionCircleMinimum = 1.5,
            DimensionTextMinimum = 7.5
        };

        public double ScaleMarginRatio { get; set; }
        public double BarRadius { get; set; }
        public double MinimumRebarCover { get; set; }
        public double MinimumSecondLayerGap { get; set; }
        public double OuterLineThickness { get; set; }
        public double StirrupBoxThickness { get; set; }
        public double StirrupLineThickness { get; set; }
        public double DimensionLineThickness { get; set; }
        public double DimensionMarkThickness { get; set; }
        public double DimensionGapMinimum { get; set; }
        public double DimensionExtensionMinimum { get; set; }
        public double DimensionTickMinimum { get; set; }
        public double DimensionCircleMinimum { get; set; }
        public double DimensionTextMinimum { get; set; }
    }
}
