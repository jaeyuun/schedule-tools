using GirderSchedule.Dxf.Drawers.Common;
using GirderSchedule.Dxf.Geometry;
using GirderSchedule.Dxf.Layouts;
using GirderSchedule.Dxf.Styles;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class SectionDrawer
    {
        private readonly DxfEntityDrawer _entityDrawer;

        public SectionDrawer(DxfEntityDrawer entityDrawer)
        {
            _entityDrawer = entityDrawer;
        }

        public void Draw(SectionLayout layout)
        {
            if (layout == null)
            {
                return;
            }

            _entityDrawer.AddLine(DxfStyleRole.Girder, layout.OuterLeft - layout.SideWing, layout.OuterTop, layout.OuterRight + layout.SideWing, layout.OuterTop);
            _entityDrawer.AddPolyline(DxfStyleRole.Girder, false,
                layout.OuterLeft - layout.SideWing, layout.OuterShelfY,
                layout.OuterLeft, layout.OuterShelfY,
                layout.OuterLeft, layout.OuterBottom,
                layout.OuterRight, layout.OuterBottom,
                layout.OuterRight, layout.OuterShelfY,
                layout.OuterRight + layout.SideWing, layout.OuterShelfY);
        }

        public void DrawDisabled(DxfBox box)
        {
            if (box == null)
            {
                return;
            }

            _entityDrawer.AddLine(DxfStyleRole.Rebar, box.Left, box.Top, box.Right, box.Bottom);
        }
    }
}
