using GirderSchedule.Dxf.Constants;

namespace GirderSchedule.Dxf.Styles
{
    public sealed class DxfStyleNameSet
    {
        public string TextStyleName { get; set; }
        public string DimensionStyleName { get; set; }
        public string DrawingBlockName { get; set; }

        public DxfStyleNameSet()
        {
            TextStyleName = DxfStyles.DefaultText;
            DimensionStyleName = DxfStyles.Dimension;
            DrawingBlockName = DxfBlocks.TitleBlock;
        }
    }
}