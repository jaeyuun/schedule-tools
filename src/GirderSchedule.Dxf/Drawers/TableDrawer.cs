using GirderSchedule.Dxf.Drawers.Common;
using GirderSchedule.Dxf.Geometry;
using GirderSchedule.Dxf.Layouts;
using GirderSchedule.Dxf.Styles;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class TableDrawer
    {

        private const string MemberNameLabel = "부 재 명";
        private const string SectionSizeLabel = "( B × H )";
        private const string SectionLabel = "단    면";
        private const string TopRebarLabel = "상 부 근";
        private const string BottomRebarLabel = "하 부 근";
        private const string StirrupLabel = "스 터 럽";
        private const string SkinRebarLabel = "표피철근(X)";
        private const string DashHdLabel = "- HD";
        private const string AtLabel = "@";

        private readonly TextDrawer _textDrawer;
        private readonly DxfEntityDrawer _entityDrawer;

        public TableDrawer(TextDrawer textDrawer, DxfEntityDrawer entityDrawer)
        {
            _textDrawer = textDrawer;
            _entityDrawer = entityDrawer;
        }

        public void Draw(double x, double y)
        {
            DrawFormLines(x, y);
            DrawDefpointGuides(x, y);
            DrawFormTexts(x, y);
            DrawRebarStaticTexts(x, y);
        }

        private void DrawFormLines(double x, double y)
        {
            DrawHorizontalFormLine(x, y, ScheduleTableLayout.HeaderTopY);
            DrawHorizontalFormLine(x, y, ScheduleTableLayout.HeaderBottomY);
            DrawHorizontalFormLine(x, y, ScheduleTableLayout.SectionBottomY);
            DrawHorizontalFormLine(x, y, ScheduleTableLayout.TopRebarBottomY);
            DrawHorizontalFormLine(x, y, ScheduleTableLayout.BottomRebarBottomY);
            DrawHorizontalFormLine(x, y, ScheduleTableLayout.StirrupBottomY);
            DrawHorizontalFormLine(x, y, ScheduleTableLayout.SkinRebarBottomY);

            DrawVerticalFormLine(x, y, 0.0, ScheduleTableLayout.SkinRebarBottomY, ScheduleTableLayout.HeaderTopY);
            DrawVerticalFormLine(x, y, ScheduleTableLayout.LabelWidth, ScheduleTableLayout.SkinRebarBottomY, ScheduleTableLayout.HeaderTopY);
            DrawVerticalFormLine(x, y, ScheduleTableLayout.FormWidth, ScheduleTableLayout.SkinRebarBottomY, ScheduleTableLayout.HeaderTopY);

            for (var i = 1; i < ScheduleCellLayout.SlotCount; i++)
            {
                var slotX = ScheduleTableLayout.LabelWidth + ScheduleCellLayout.SlotWidth * i;
                var topY = i % ScheduleCellLayout.SlotCountPerSet == 0 ? ScheduleTableLayout.HeaderTopY : ScheduleTableLayout.HeaderBottomY;
                DrawVerticalFormLine(x, y, slotX, ScheduleTableLayout.SkinRebarBottomY, topY);
            }
        }

        private void DrawDefpointGuides(double x, double y)
        {
            DrawDefpointLine(x, y, ScheduleTableLayout.LabelWidth, ScheduleTableLayout.MemberForceTopY, ScheduleTableLayout.FormWidth, ScheduleTableLayout.MemberForceTopY);
            DrawDefpointLine(x, y, ScheduleTableLayout.LabelWidth, ScheduleTableLayout.MemberForceBottomY, ScheduleTableLayout.FormWidth, ScheduleTableLayout.MemberForceBottomY);

            for (var i = 0; i < ScheduleCellLayout.SlotCount; i++)
            {
                var slot = ScheduleCellLayout.GetSlotBox(i);
                DrawDefpointLine(x, y, slot.CenterX, ScheduleTableLayout.HeaderBottomY, slot.CenterX, ScheduleTableLayout.SectionBottomY);
            }
        }

        private void DrawFormTexts(double x, double y)
        {
            _textDrawer.DrawFormText(MemberNameLabel, x + ScheduleTableLayout.LabelTextX, y + ScheduleTableLayout.MemberNameLabelTextY, ScheduleTableLayout.HeaderTextHeight);
            _textDrawer.DrawFormText(SectionSizeLabel, x + ScheduleTableLayout.LabelTextX, y + ScheduleTableLayout.SectionSizeLabelTextY, ScheduleTableLayout.HeaderTextHeight);
            _textDrawer.DrawFormText(SectionLabel, x + ScheduleTableLayout.LabelTextX, y + ScheduleTableLayout.SectionLabelTextY, ScheduleTableLayout.LabelTextHeight);
            _textDrawer.DrawFormText(TopRebarLabel, x + ScheduleTableLayout.LabelTextX, y + ScheduleTableLayout.TopRebarLabelTextY, ScheduleTableLayout.LabelTextHeight);
            _textDrawer.DrawFormText(BottomRebarLabel, x + ScheduleTableLayout.LabelTextX, y + ScheduleTableLayout.BottomRebarLabelTextY, ScheduleTableLayout.LabelTextHeight);
            _textDrawer.DrawFormText(StirrupLabel, x + ScheduleTableLayout.LabelTextX, y + ScheduleTableLayout.StirrupLabelTextY, ScheduleTableLayout.LabelTextHeight);
            _textDrawer.DrawFormText(SkinRebarLabel, x + ScheduleTableLayout.LabelTextX, y + ScheduleTableLayout.SkinRebarLabelTextY, ScheduleTableLayout.LabelTextHeight);
        }

        private void DrawRebarStaticTexts(double x, double y)
        {
            for (var i = 0; i < ScheduleCellLayout.SlotCount; i++)
            {
                DrawMainRebarStaticText(ScheduleCellLayout.GetTopRebarBox(i).Move(x, y));
                DrawMainRebarStaticText(ScheduleCellLayout.GetBottomRebarBox(i).Move(x, y));
                DrawStirrupStaticText(ScheduleCellLayout.GetStirrupBox(i).Move(x, y));
            }
        }

        private void DrawMainRebarStaticText(DxfBox box)
        {
            _textDrawer.DrawFormRawText(DashHdLabel, box.Left + RebarTextLayout.MainRebarDashHdLabelOffsetX, RebarTextLayout.GetRowTextY(box), RebarTextLayout.TextHeight);
        }

        private void DrawStirrupStaticText(DxfBox box)
        {
            var y = RebarTextLayout.GetRowTextY(box);
            _textDrawer.DrawFormRawText(DashHdLabel, box.Left + RebarTextLayout.StirrupDashHdLabelOffsetX, y, RebarTextLayout.TextHeight);
            _textDrawer.DrawFormRawText(AtLabel, box.Left + RebarTextLayout.StirrupAtLabelOffsetX, y, RebarTextLayout.TextHeight);
        }

        private void DrawHorizontalFormLine(double baseX, double baseY, double y)
        {
            DrawFormLine(baseX, baseY, 0.0, y, ScheduleTableLayout.FormWidth, y);
        }

        private void DrawVerticalFormLine(double baseX, double baseY, double x, double bottomY, double topY)
        {
            DrawFormLine(baseX, baseY, x, bottomY, x, topY);
        }

        private void DrawFormLine(double baseX, double baseY, double x1, double y1, double x2, double y2)
        {
            _entityDrawer.AddPolyline(DxfStyleRole.Form, false, baseX + x1, baseY + y1, baseX + x2, baseY + y2);
        }

        private void DrawDefpointLine(double baseX, double baseY, double x1, double y1, double x2, double y2)
        {
            _entityDrawer.AddPolyline(DxfStyleRole.Defpoint, false, baseX + x1, baseY + y1, baseX + x2, baseY + y2);
        }
    }
}
