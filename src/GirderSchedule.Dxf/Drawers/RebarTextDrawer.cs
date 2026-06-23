using GirderSchedule.Domain.Models;
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
            DrawSkinRebar(item.SkinRebar, skinBox);
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

        private void DrawSkinRebar(SkinRebarData skinRebar, DxfBox box)
        {
            if (skinRebar == null || box == null)
            {
                return;
            }

            string skinRebarText;
            if (skinRebar.Diameter <= 0 || skinRebar.Spacing <= 0)
            {
                skinRebarText = "-";
            }
            else
            {
                skinRebarText = "HD " + skinRebar.Diameter.ToString("0") + " @ " + skinRebar.Spacing.ToString("0");
            }

            var x = box.Left + RebarTextLayout.SkinRebarTextOffsetX;
            var y = RebarTextLayout.GetRowTextY(box);

            if (string.IsNullOrWhiteSpace(skinRebar.Note))
            {
                _textDrawer.DrawRebarValueRawText(skinRebarText, x, y, RebarTextLayout.TextHeight, TextAlignment.BaselineCenter);
                return;
            }

            _textDrawer.DrawRebarValueRawText(skinRebarText, x, y + 75.0, RebarTextLayout.TextHeight, TextAlignment.BaselineCenter);
            _textDrawer.DrawRebarValueRawText(skinRebar.Note, x, y - 65.0, RebarTextLayout.TextHeight, TextAlignment.BaselineCenter);
        }
    }
}
