using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Dxf.Geometry;
using GirderSchedule.Dxf.Layouts;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class RebarTextDrawer
    {
        private readonly TextDrawer _textDrawer;

        public RebarTextDrawer(TextDrawer textDrawer)
        {
            _textDrawer = textDrawer;
        }

        public void Draw(ScheduleItem item, DxfBox topBox, DxfBox bottomBox, DxfBox stirrupBox, DxfBox skinBox)
        {
            if (item == null)
            {
                return;
            }

            DrawMainRebar(item.TopRebar, topBox);
            DrawMainRebar(item.BottomRebar, bottomBox);
            DrawStirrup(item.Stirrup, stirrupBox);
            DrawSkinRebar(item, skinBox);
        }

        private void DrawMainRebar(RebarSet rebar, DxfBox box)
        {
            if (rebar == null || box == null)
            {
                return;
            }

            var totalCount = GetTotalRebarCount(rebar);

            if (totalCount <= 0)
            {
                return;
            }

            var y = RebarTextLayout.GetRowTextY(box);
            _textDrawer.DrawRebarValueRawText(totalCount.ToString("0"), box.Left + RebarTextLayout.MainRebarCountOffsetX, y, RebarTextLayout.TextHeight, TextAlignment.BaselineRight);
            _textDrawer.DrawRebarValueRawText(rebar.Diameter.ToString("0"), box.Left + RebarTextLayout.MainRebarDiameterOffsetX, y, RebarTextLayout.TextHeight, TextAlignment.BaselineLeft);
        }

        private int GetTotalRebarCount(RebarSet rebar)
        {
            var count = 0;

            if (rebar.FirstLayer != null)
            {
                count += rebar.FirstLayer.Count;
            }

            if (rebar.SecondLayer != null)
            {
                count += rebar.SecondLayer.Count;
            }

            return count;
        }

        private void DrawStirrup(StirrupData stirrup, DxfBox box)
        {
            if (stirrup == null || box == null)
            {
                return;
            }

            var y = RebarTextLayout.GetRowTextY(box);
            _textDrawer.DrawRebarValueRawText(stirrup.Legs.ToString("0"), box.Left + RebarTextLayout.StirrupLegOffsetX, y, RebarTextLayout.TextHeight, TextAlignment.BaselineRight);
            _textDrawer.DrawRebarValueRawText(stirrup.Diameter.ToString("0"), box.Left + RebarTextLayout.StirrupDiameterOffsetX, y, RebarTextLayout.TextHeight, TextAlignment.BaselineCenter);
            _textDrawer.DrawRebarValueRawText(stirrup.Spacing.ToString("0"), box.Left + RebarTextLayout.StirrupSpacingOffsetX, y, RebarTextLayout.TextHeight, TextAlignment.BaselineLeft);
        }

        private void DrawSkinRebar(ScheduleItem item, DxfBox box)
        {
            if (item == null || box == null || string.IsNullOrWhiteSpace(item.SkinRebarText))
            {
                return;
            }

            _textDrawer.DrawRebarValueRawText(item.SkinRebarText, box.Left + RebarTextLayout.SkinRebarTextOffsetX, RebarTextLayout.GetRowTextY(box), RebarTextLayout.TextHeight, TextAlignment.BaselineCenter);
        }
    }
}
