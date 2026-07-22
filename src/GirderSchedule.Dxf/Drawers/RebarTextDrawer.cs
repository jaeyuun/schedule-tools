using GirderSchedule.Domain.Formatting;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;
using GirderSchedule.Dxf.Layouts;
using ScheduleTools.Dxf.Geometry;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class RebarTextDrawer
    {
        private readonly TextDrawer _textDrawer;
        private readonly ScheduleItemQuery _query = new ScheduleItemQuery();
        private readonly ScheduleValueFormatter _formatter = new ScheduleValueFormatter();

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

            DrawTopRebar(item, topBox);
            DrawBottomRebar(item, bottomBox);
            DrawStirrup(item.Stirrup, stirrupBox);
            DrawSkinRebar(item, skinBox);
        }

        private void DrawTopRebar(ScheduleItem item, DxfBox box)
        {
            if (item == null || item.MainRebar == null || box == null)
            {
                return;
            }

            DrawMainRebar(_query.GetTopTotalCount(item), item.MainRebar.Diameter, box);
        }

        private void DrawBottomRebar(ScheduleItem item, DxfBox box)
        {
            if (item == null || item.MainRebar == null || box == null)
            {
                return;
            }

            DrawMainRebar(_query.GetBottomTotalCount(item), item.MainRebar.Diameter, box);
        }

        private void DrawMainRebar(int totalCount, double diameter, DxfBox box)
        {
            if (totalCount <= 0 || diameter <= 0 || box == null)
            {
                return;
            }

            var y = RebarTextLayout.GetRowTextY(box);
            _textDrawer.DrawRebarValueRawText(_formatter.FormatNumber(totalCount), box.Left + RebarTextLayout.MainRebarCountOffsetX, y, RebarTextLayout.TextHeight, TextAlignment.BaselineRight);
            _textDrawer.DrawRebarValueRawText(_formatter.FormatNumber(diameter), box.Left + RebarTextLayout.MainRebarDiameterOffsetX, y, RebarTextLayout.TextHeight, TextAlignment.BaselineLeft);
        }

        private void DrawStirrup(StirrupData stirrup, DxfBox box)
        {
            if (stirrup == null || box == null)
            {
                return;
            }

            var y = RebarTextLayout.GetRowTextY(box);
            _textDrawer.DrawRebarValueRawText(_formatter.FormatNumber(stirrup.Legs), box.Left + RebarTextLayout.StirrupLegOffsetX, y, RebarTextLayout.TextHeight, TextAlignment.BaselineRight);
            _textDrawer.DrawRebarValueRawText(_formatter.FormatNumber(stirrup.Diameter), box.Left + RebarTextLayout.StirrupDiameterOffsetX, y, RebarTextLayout.TextHeight, TextAlignment.BaselineCenter);
            _textDrawer.DrawRebarValueRawText(_formatter.FormatNumber(stirrup.Spacing), box.Left + RebarTextLayout.StirrupSpacingOffsetX, y, RebarTextLayout.TextHeight, TextAlignment.BaselineLeft);
        }

        private void DrawSkinRebar(ScheduleItem item, DxfBox box)
        {
            if (item == null || item.Section == null || item.SkinRebar == null || box == null)
            {
                return;
            }

            var skinRebarText = _formatter.FormatSkinRebar(item);
            var x = box.Left + RebarTextLayout.SkinRebarTextOffsetX;
            var y = RebarTextLayout.GetRowTextY(box);

            if (string.IsNullOrWhiteSpace(item.SkinRebar.Note) || skinRebarText == "-")
            {
                _textDrawer.DrawRebarValueRawText(skinRebarText, x, y, RebarTextLayout.TextHeight, TextAlignment.BaselineCenter);
                return;
            }

            var firstLine = "HD " + _formatter.FormatNumber(item.SkinRebar.Diameter);

            if (item.SkinRebar.Spacing > 0)
            {
                firstLine += " @ " + _formatter.FormatNumber(item.SkinRebar.Spacing);
            }

            _textDrawer.DrawRebarValueRawText(firstLine, x, y + 75.0, RebarTextLayout.TextHeight, TextAlignment.BaselineCenter);
            _textDrawer.DrawRebarValueRawText(item.SkinRebar.Note, x, y - 65.0, RebarTextLayout.TextHeight, TextAlignment.BaselineCenter);
        }
    }
}
