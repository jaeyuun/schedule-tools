using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Constants;

namespace GirderSchedule.Dxf.Styles
{
    public sealed class DxfStyleNameSet
    {
        public string Text { get; set; }
        public string Dimension { get; set; }
        public string Block { get; set; }

        public DxfStyleNameSet()
        {
            Text = DxfStyles.DefaultText;
            Dimension = DxfStyles.Dimension;
            Block = DxfBlocks.TitleBlock;
        }

        public string GetStyleName(DxfStyleRole role)
        {
            switch (role)
            {
                case DxfStyleRole.Text:
                    return Text;
                case DxfStyleRole.Block:
                    return Block;
                case DxfStyleRole.Dimension:
                    return Dimension;
                default:
                    return string.Empty;
            }
        }
    }
}