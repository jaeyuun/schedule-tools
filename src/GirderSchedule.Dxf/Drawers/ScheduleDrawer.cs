using GirderSchedule.Domain.Enums;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Export;
using GirderSchedule.Dxf.Export;
using GirderSchedule.Dxf.Geometry;
using GirderSchedule.Dxf.Layouts;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class ScheduleDrawer
    {
        private readonly TextDrawer _textDrawer;
        private readonly TableDrawer _tableDrawer;
        private readonly TitleBlockDrawer _titleBlockDrawer;
        private readonly SectionDrawer _sectionDrawer;
        private readonly RebarDrawer _rebarDrawer;
        private readonly DimensionDrawer _dimensionDrawer;
        private readonly RebarTextDrawer _rebarTextDrawer;
        private readonly SectionLayoutFactory _layoutFactory;

        public ScheduleDrawer(TextDrawer textDrawer, TableDrawer tableDrawer, TitleBlockDrawer titleBlockDrawer, SectionDrawer sectionDrawer, RebarDrawer rebarDrawer, DimensionDrawer dimensionDrawer, RebarTextDrawer rebarTextDrawer, SectionLayoutFactory layoutFactory)
        {
            _textDrawer = textDrawer;
            _tableDrawer = tableDrawer;
            _titleBlockDrawer = titleBlockDrawer;
            _sectionDrawer = sectionDrawer;
            _rebarDrawer = rebarDrawer;
            _dimensionDrawer = dimensionDrawer;
            _rebarTextDrawer = rebarTextDrawer;
            _layoutFactory = layoutFactory;
        }

        public void Draw(ScheduleExportPage page, double pageBaseX, double pageBaseY, DxfExportOptions options)
        {
            if (page == null)
            {
                return;
            }

            _titleBlockDrawer.Draw(page, pageBaseX, pageBaseY);
            DrawTables(pageBaseX, pageBaseY);
            DrawSets(page, ScheduleTableLayout.GetBaseX(pageBaseX), ScheduleTableLayout.GetBaseY(pageBaseY), options);
        }

        private void DrawTables(double pageBaseX, double pageBaseY)
        {
            for (var rowIndex = 0; rowIndex < ScheduleTableLayout.RowsPerPage; rowIndex++)
            {
                _tableDrawer.Draw(ScheduleTableLayout.GetRowBaseX(pageBaseX, rowIndex), ScheduleTableLayout.GetRowBaseY(pageBaseY, rowIndex));
            }
        }

        private void DrawSets(ScheduleExportPage page, double scheduleBaseX, double scheduleBaseY, DxfExportOptions options)
        {
            for (var setIndex = 0; setIndex < page.Sets.Count && setIndex < ScheduleCellLayout.SetsPerPage; setIndex++)
            {
                DrawSet(page.Sets[setIndex], setIndex, scheduleBaseX, scheduleBaseY, options);
            }
        }

        private void DrawSet(ScheduleSet set, int setIndex, double baseX, double baseY, DxfExportOptions options)
        {
            if (set == null)
            {
                return;
            }

            DrawMemberHeader(set, setIndex, baseX, baseY);
            DrawSlot(set, setIndex, ScheduleSlotType.Left, CanDrawSlot(set.Left, options.IncludeLeft), baseX, baseY, options);
            DrawSlot(set, setIndex, ScheduleSlotType.Center, CanDrawSlot(set.Center, options.IncludeCenter), baseX, baseY, options);
            DrawSlot(set, setIndex, ScheduleSlotType.Right, CanDrawSlot(set.Right, options.IncludeRight), baseX, baseY, options);
        }

        private bool CanDrawSlot(ScheduleItem item, bool isIncluded)
        {
            return isIncluded && item != null && item.IsSectionEnabled;
        }

        private void DrawMemberHeader(ScheduleSet set, int setIndex, double baseX, double baseY)
        {
            var box = ScheduleCellLayout.GetMemberNameBox(setIndex).Move(baseX, baseY);
            var item = GetHeaderItem(set);
            var rowOffsetY = ScheduleCellLayout.GetSetRowOffsetY(setIndex);

            _textDrawer.DrawValueText(set.MemberName, box.CenterX, baseY + rowOffsetY + ScheduleTextLayout.MemberNameTextY, ScheduleTextLayout.HeaderTextHeight);

            if (item == null || item.Section == null)
            {
                return;
            }

            var sizeText = string.Format("( {0:0} × {1:0} )", item.Section.Width, item.Section.Height);
            _textDrawer.DrawRebarValueText(sizeText, box.CenterX, baseY + rowOffsetY + ScheduleTextLayout.SectionSizeTextY, ScheduleTextLayout.HeaderTextHeight);
        }

        private ScheduleItem GetHeaderItem(ScheduleSet set)
        {
            if (set == null)
            {
                return null;
            }

            if (set.Center != null)
            {
                return set.Center;
            }

            if (set.Left != null)
            {
                return set.Left;
            }

            return set.Right;
        }

        private void DrawSlot(ScheduleSet set, int setIndex, ScheduleSlotType type, bool enabled, double baseX, double baseY, DxfExportOptions options)
        {
            var sectionBox = ScheduleCellLayout.GetSectionBox(setIndex, type).Move(baseX, baseY);

            if (!enabled)
            {
                _sectionDrawer.DrawDisabled(sectionBox);
                return;
            }

            var item = set.GetItem(type);

            if (item == null)
            {
                return;
            }

            var sectionLayout = _layoutFactory.Create(sectionBox, item);

            DrawPositionText(item, ScheduleCellLayout.GetPositionBox(setIndex, type).Move(baseX, baseY));
            DrawMemberForceValues(item, ScheduleCellLayout.GetMemberForceBox(setIndex, type).Move(baseX, baseY));
            DrawSectionNote(item, sectionBox);

            _sectionDrawer.Draw(sectionLayout);
            _rebarDrawer.Draw(sectionLayout, item);
            _dimensionDrawer.Draw(sectionLayout, item, options.IsWidthOver, options.IsHeightOver);
            _rebarTextDrawer.Draw(
                item,
                ScheduleCellLayout.GetTopRebarBox(setIndex, type).Move(baseX, baseY),
                ScheduleCellLayout.GetBottomRebarBox(setIndex, type).Move(baseX, baseY),
                ScheduleCellLayout.GetStirrupBox(setIndex, type).Move(baseX, baseY),
                ScheduleCellLayout.GetSkinRebarBox(setIndex, type).Move(baseX, baseY));
        }

        private void DrawPositionText(ScheduleItem item, DxfBox positionBox)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Position))
            {
                return;
            }

            _textDrawer.DrawRebarValueText(item.Position, positionBox.CenterX, positionBox.Top + ScheduleTextLayout.PositionTextY - ScheduleTableLayout.SectionTopY, ScheduleTextLayout.PositionTextHeight);
        }

        private void DrawMemberForceValues(ScheduleItem item, DxfBox box)
        {
            if (item == null || item.MemberForce == null || box == null)
            {
                return;
            }

            if (item.MemberForce.Moment.HasValue)
            {
                _textDrawer.DrawMemberForceText("M = " + item.MemberForce.Moment.Value.ToString("0"), box.Left + ScheduleTextLayout.MemberForceMomentOffsetX, box.CenterY, ScheduleTextLayout.MemberForceTextHeight);
            }

            if (item.MemberForce.Shear.HasValue)
            {
                _textDrawer.DrawMemberForceText("V = " + item.MemberForce.Shear.Value.ToString("0"), box.Left + ScheduleTextLayout.MemberForceShearOffsetX, box.CenterY, ScheduleTextLayout.MemberForceTextHeight);
            }
        }

        private void DrawSectionNote(ScheduleItem item, DxfBox box)
        {
            if (item == null || item.Section == null || box == null || string.IsNullOrWhiteSpace(item.Section.Note))
            {
                return;
            }

            var x = box.CenterX;
            var y = box.Bottom + ScheduleTextLayout.SectionNoteBottomOffsetY;

            _textDrawer.DrawValueText(item.Section.Note, x, y, ScheduleTextLayout.SectionNoteTextHeight);
        }
    }
}
