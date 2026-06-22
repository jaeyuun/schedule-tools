using GirderSchedule.Domain.Models;
using GirderSchedule.Dxf.Geometry;

namespace GirderSchedule.Dxf.Layouts
{
    public static class ScheduleCellLayout
    {
        public const int SetsPerRow = 3;
        public const int SlotCountPerSet = 3;
        public const int SlotCount = 9;
        public const int SetsPerPage = SetsPerRow * ScheduleTableLayout.RowsPerPage;

        public const double SlotWidth = 2390.0;
        public const double SetWidth = SlotWidth * SlotCountPerSet;

        public static int GetRowIndex(int setIndex)
        {
            return setIndex / SetsPerRow;
        }

        public static int GetColumnIndex(int setIndex)
        {
            return setIndex % SetsPerRow;
        }

        public static double GetSetRowOffsetY(int setIndex)
        {
            return ScheduleTableLayout.GetRowOffsetY(GetRowIndex(setIndex));
        }

        public static int GetItemIndex(int setIndex, ScheduleSlotType type)
        {
            return GetColumnIndex(setIndex) * SlotCountPerSet + GetSlotOffset(type);
        }

        public static int GetSlotOffset(ScheduleSlotType type)
        {
            if (type == ScheduleSlotType.Left)
            {
                return 0;
            }

            if (type == ScheduleSlotType.Center)
            {
                return 1;
            }

            return 2;
        }

        public static DxfBox GetMemberNameBox(int setIndex)
        {
            var columnIndex = GetColumnIndex(setIndex);
            var rowOffsetY = GetSetRowOffsetY(setIndex);
            var left = ScheduleTableLayout.LabelWidth + SetWidth * columnIndex;

            return new DxfBox(left, ScheduleTableLayout.HeaderTopY + rowOffsetY, left + SetWidth, ScheduleTableLayout.HeaderBottomY + rowOffsetY);
        }

        public static DxfBox GetSlotBox(int setIndex, ScheduleSlotType type)
        {
            return GetSlotBox(GetItemIndex(setIndex, type)).Move(0.0, GetSetRowOffsetY(setIndex));
        }

        public static DxfBox GetHeaderBox(int setIndex, ScheduleSlotType type)
        {
            return GetHeaderBox(GetItemIndex(setIndex, type)).Move(0.0, GetSetRowOffsetY(setIndex));
        }

        public static DxfBox GetSectionBox(int setIndex, ScheduleSlotType type)
        {
            return GetSectionBox(GetItemIndex(setIndex, type)).Move(0.0, GetSetRowOffsetY(setIndex));
        }

        public static DxfBox GetPositionBox(int setIndex, ScheduleSlotType type)
        {
            var section = GetSectionBox(setIndex, type);
            var rowOffsetY = GetSetRowOffsetY(setIndex);

            return new DxfBox(section.Left, ScheduleTableLayout.SectionTopY + rowOffsetY, section.Right, ScheduleTableLayout.MemberForceTopY + rowOffsetY);
        }

        public static DxfBox GetMemberForceBox(int setIndex, ScheduleSlotType type)
        {
            var section = GetSectionBox(setIndex, type);
            var rowOffsetY = GetSetRowOffsetY(setIndex);

            return new DxfBox(section.Left, ScheduleTableLayout.MemberForceTopY + rowOffsetY, section.Right, ScheduleTableLayout.MemberForceBottomY + rowOffsetY);
        }

        public static DxfBox GetTopRebarBox(int setIndex, ScheduleSlotType type)
        {
            return GetTopRebarBox(GetItemIndex(setIndex, type)).Move(0.0, GetSetRowOffsetY(setIndex));
        }

        public static DxfBox GetBottomRebarBox(int setIndex, ScheduleSlotType type)
        {
            return GetBottomRebarBox(GetItemIndex(setIndex, type)).Move(0.0, GetSetRowOffsetY(setIndex));
        }

        public static DxfBox GetStirrupBox(int setIndex, ScheduleSlotType type)
        {
            return GetStirrupBox(GetItemIndex(setIndex, type)).Move(0.0, GetSetRowOffsetY(setIndex));
        }

        public static DxfBox GetSkinRebarBox(int setIndex, ScheduleSlotType type)
        {
            return GetSkinRebarBox(GetItemIndex(setIndex, type)).Move(0.0, GetSetRowOffsetY(setIndex));
        }

        public static DxfBox GetSlotBox(int index)
        {
            var left = ScheduleTableLayout.LabelWidth + SlotWidth * index;

            return new DxfBox(left, ScheduleTableLayout.HeaderTopY, left + SlotWidth, ScheduleTableLayout.SkinRebarBottomY);
        }

        public static DxfBox GetHeaderBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, ScheduleTableLayout.HeaderTopY, slot.Right, ScheduleTableLayout.HeaderBottomY);
        }

        public static DxfBox GetSectionBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, ScheduleTableLayout.SectionTopY, slot.Right, ScheduleTableLayout.SectionBottomY);
        }

        public static DxfBox GetTopRebarBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, ScheduleTableLayout.TopRebarTopY, slot.Right, ScheduleTableLayout.TopRebarBottomY);
        }

        public static DxfBox GetBottomRebarBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, ScheduleTableLayout.BottomRebarTopY, slot.Right, ScheduleTableLayout.BottomRebarBottomY);
        }

        public static DxfBox GetStirrupBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, ScheduleTableLayout.StirrupTopY, slot.Right, ScheduleTableLayout.StirrupBottomY);
        }

        public static DxfBox GetSkinRebarBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, ScheduleTableLayout.SkinRebarTopY, slot.Right, ScheduleTableLayout.SkinRebarBottomY);
        }
    }
}
