using GirderSchedule.Domain.Girder.Models;

namespace GirderSchedule.Dxf.Common
{
    public static class DxfLayout
    {
        public const double OriginX = 0.0;
        public const double OriginY = 5250.0;

        public const double SheetWidth = 22710.0;
        public const double SheetHeight = 5250.0;

        public const double LabelWidth = 1200.0;
        public const double SlotWidth = 2390.0;
        public const int SlotCountPerSet = 3;
        public const double SetWidth = SlotWidth * SlotCountPerSet;
        public const int SlotCount = 9;

        public const double HeaderTopY = 0.0;
        public const double HeaderBottomY = -850.0;

        public const double SectionTopY = -850.0;
        public const double MemberForceTopY = -1100.0;
        public const double MemberForceBottomY = -1250.0;
        public const double MemberForceCenterY = (MemberForceTopY + MemberForceBottomY) / 2.0;
        public const double SectionBottomY = -4050.0;

        public const double TopRebarTopY = -4050.0;
        public const double TopRebarBottomY = -4350.0;

        public const double BottomRebarTopY = -4350.0;
        public const double BottomRebarBottomY = -4650.0;

        public const double StirrupTopY = -4650.0;
        public const double StirrupBottomY = -4950.0;

        public const double SkinTopY = -4950.0;
        public const double SkinBottomY = -5250.0;

        public const double HeaderMemberNameTextY = -325.0;
        public const double HeaderSectionSizeTextY = -560.0;
        public const double HeaderTextHeight = 120.0;

        public const double HeaderWidthValueOffsetX = -250.0;
        public const double HeaderHeightValueOffsetX = 250.0;

        public const double RebarTextHeight = 90.0;
        public const double ValueTextGap = 45.0;
        public const double NumberTextWidthFactor = 0.6;

        public const double RebarDashHdCenterOffsetX = 0.0;
        public const double RebarDashHdTextWidth = 315.0;
        public const double RebarCountAdjustX = 20.0; // 상부근/하부근 개수 위치
        public const double RebarDiameterAdjustX = 20.0; // 상부근/하부근 규격 위치

        public const double StirrupDashHdCenterOffsetX = -135.0;
        public const double StirrupDashHdTextWidth = 315.0;

        public const double StirrupAtCenterOffsetX = 245.0;
        public const double StirrupAtTextWidth = 60.0;

        public const double StirrupLegAdjustX = -60.0; // 스터럽 개수 위치
        public const double StirrupDiameterAdjustX = -100.0; // 스터럽 규격 위치
        public const double StirrupSpacingAdjustX = 20.0; // 스터럽 간격 위치

        public const double RebarValueOffsetY = -10.0;
        public const double StirrupValueOffsetY = -10.0;

        public static int GetItemIndex(int setIndex, ScheduleSlotType type)
        {
            return setIndex * SlotCountPerSet + GetSlotOffset(type);
        }

        public static int GetSlotIndex(int setIndex, ScheduleSlotType type)
        {
            return GetItemIndex(setIndex, type);
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
            var left = LabelWidth + SetWidth * setIndex;

            return new DxfBox(
                left,
                HeaderTopY,
                left + SetWidth,
                HeaderBottomY);
        }

        public static DxfBox GetSlotBox(int setIndex, ScheduleSlotType type)
        {
            return GetSlotBox(GetItemIndex(setIndex, type));
        }

        public static DxfBox GetHeaderBox(int setIndex, ScheduleSlotType type)
        {
            return GetHeaderBox(GetItemIndex(setIndex, type));
        }

        public static DxfBox GetSectionBox(int setIndex, ScheduleSlotType type)
        {
            return GetSectionBox(GetItemIndex(setIndex, type));
        }

        public static DxfBox GetPositionBox(int setIndex, ScheduleSlotType type)
        {
            var section = GetSectionBox(setIndex, type);

            return new DxfBox(
                section.Left,
                SectionTopY,
                section.Right,
                MemberForceTopY);
        }

        public static DxfBox GetMemberForceBox(int setIndex, ScheduleSlotType type)
        {
            var section = GetSectionBox(setIndex, type);

            return new DxfBox(
                section.Left,
                MemberForceTopY,
                section.Right,
                MemberForceBottomY);
        }

        public static DxfBox GetTopRebarBox(int setIndex, ScheduleSlotType type)
        {
            return GetTopRebarBox(GetItemIndex(setIndex, type));
        }

        public static DxfBox GetBottomRebarBox(int setIndex, ScheduleSlotType type)
        {
            return GetBottomRebarBox(GetItemIndex(setIndex, type));
        }

        public static DxfBox GetStirrupBox(int setIndex, ScheduleSlotType type)
        {
            return GetStirrupBox(GetItemIndex(setIndex, type));
        }

        public static DxfBox GetSkinBox(int setIndex, ScheduleSlotType type)
        {
            return GetSkinBox(GetItemIndex(setIndex, type));
        }

        public static double GetSlotCenterX(int setIndex, ScheduleSlotType type)
        {
            var box = GetSectionBox(setIndex, type);
            return box.CenterX;
        }

        public static DxfBox GetSlotBox(int index)
        {
            var left = LabelWidth + SlotWidth * index;

            return new DxfBox(
                left,
                HeaderTopY,
                left + SlotWidth,
                SkinBottomY);
        }

        public static DxfBox GetHeaderBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(
                slot.Left,
                HeaderTopY,
                slot.Right,
                HeaderBottomY);
        }

        public static DxfBox GetSectionBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(
                slot.Left,
                SectionTopY,
                slot.Right,
                SectionBottomY);
        }

        public static DxfBox GetTopRebarBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(
                slot.Left,
                TopRebarTopY,
                slot.Right,
                TopRebarBottomY);
        }

        public static DxfBox GetBottomRebarBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(
                slot.Left,
                BottomRebarTopY,
                slot.Right,
                BottomRebarBottomY);
        }

        public static DxfBox GetStirrupBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(
                slot.Left,
                StirrupTopY,
                slot.Right,
                StirrupBottomY);
        }

        public static DxfBox GetSkinBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(
                slot.Left,
                SkinTopY,
                slot.Right,
                SkinBottomY);
        }
    }
}