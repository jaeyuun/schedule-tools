using GirderSchedule.Domain.Girder.Models;

namespace GirderSchedule.Dxf.Common
{
    public static class DxfLayout
    {
        public const double OriginX = 0.0;
        public const double OriginY = 5250.0;

        public const int SetsPerRow = 3;
        public const int RowsPerPage = 3;
        public const int SetsPerPage = SetsPerRow * RowsPerPage;

        public const double PageFrameScale = 65.0;
        public const double PageFrameSourceWidth = 420.0;
        public const double PageFrameSourceHeight = 297.0;
        public const double PageFrameWidth = PageFrameSourceWidth * PageFrameScale;
        public const double PageFrameHeight = PageFrameSourceHeight * PageFrameScale;
        public const double PageSpacingY = PageFrameHeight + 1200.0;
        public const double PageSpacingX = PageFrameWidth + 1200.0;

        public const double XrefToForm2X = 995.0;
        public const double XrefToForm2Y = 16975.0;

        public const double ScheduleOriginX = XrefToForm2X;
        public const double ScheduleOriginY = XrefToForm2Y;

        public const double XrefTitleCircleCenterX = 12025.0;
        public const double XrefTitleCircleCenterY = 17645.0;
        public const double XrefTitleCircleRadius = 325.0;

        public const double XrefTitleNumberX = 12020.0;
        public const double XrefTitleNumberY = 17645.0;
        public const double XrefTitleNumberHeight = 180.0;

        public const double XrefTitleLineStartX = XrefTitleCircleCenterX + XrefTitleCircleRadius;
        public const double XrefTitleLineEndX = 14840.0;
        public const double XrefTitleLineY = XrefTitleCircleCenterY;

        public const double XrefTitleTextX = 13650.0;
        public const double XrefTitleTextY = 17840.0;
        public const double XrefTitleTextHeight = 230.0;

        public const double XrefDrawingNameTextX = 25350.0;
        public const double XrefDrawingNameTextY = 1775.0;
        public const double XrefDrawingNameTextHeight = 120.0;

        public const double Form2Width = 22710.0;
        public const double Form2Height = 5250.0;
        public const double SheetWidth = Form2Width;
        public const double SheetHeight = Form2Height;
        public const double ScheduleRowHeight = Form2Height;

        public const double LabelWidth = 1200.0;
        public const double SlotWidth = 2390.0;
        public const int SlotCountPerSet = 3;
        public const int SlotCount = 9;
        public const double SetWidth = SlotWidth * SlotCountPerSet;

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
        public const double RebarCountAdjustX = 20.0;
        public const double RebarDiameterAdjustX = 20.0;

        public const double StirrupDashHdCenterOffsetX = -135.0;
        public const double StirrupDashHdTextWidth = 315.0;

        public const double StirrupAtCenterOffsetX = 245.0;
        public const double StirrupAtTextWidth = 60.0;

        public const double StirrupLegAdjustX = -60.0;
        public const double StirrupDiameterAdjustX = -100.0;
        public const double StirrupSpacingAdjustX = 20.0;

        public const double RebarValueOffsetY = -10.0;
        public const double StirrupValueOffsetY = -10.0;

        public static double GetPageBaseX(int pageIndex)
        {
            return 0.0;
        }

        public static double GetPageBaseY(int pageIndex)
        {
            return -pageIndex * PageSpacingY;
        }

        public static double GetPageBaseX(int pageIndex, int formColumnCount)
        {
            if (formColumnCount < 1)
            {
                formColumnCount = 3;
            }

            return (pageIndex % formColumnCount) * PageSpacingX;
        }

        public static double GetPageBaseY(int pageIndex, int formColumnCount)
        {
            if (formColumnCount < 1)
            {
                formColumnCount = 3;
            }

            return -(pageIndex / formColumnCount) * PageSpacingY;
        }

        public static double GetForm2BaseX(double pageBaseX)
        {
            return pageBaseX + ScheduleOriginX;
        }

        public static double GetForm2BaseY(double pageBaseY)
        {
            return pageBaseY + ScheduleOriginY;
        }

        public static double GetRowBaseX(double pageBaseX, int rowIndex)
        {
            return GetForm2BaseX(pageBaseX);
        }

        public static double GetRowBaseY(double pageBaseY, int rowIndex)
        {
            return GetForm2BaseY(pageBaseY) + GetRowOffsetYByRowIndex(rowIndex);
        }

        public static int GetRowIndex(int setIndex)
        {
            return setIndex / SetsPerRow;
        }

        public static int GetColumnIndex(int setIndex)
        {
            return setIndex % SetsPerRow;
        }

        public static double GetRowOffsetY(int setIndex)
        {
            return -GetRowIndex(setIndex) * ScheduleRowHeight;
        }

        public static double GetRowOffsetYByRowIndex(int rowIndex)
        {
            return -rowIndex * ScheduleRowHeight;
        }

        public static int GetItemIndex(int setIndex, ScheduleSlotType type)
        {
            return GetColumnIndex(setIndex) * SlotCountPerSet + GetSlotOffset(type);
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
            var columnIndex = GetColumnIndex(setIndex);
            var rowOffsetY = GetRowOffsetY(setIndex);
            var left = LabelWidth + SetWidth * columnIndex;

            return new DxfBox(left, HeaderTopY + rowOffsetY, left + SetWidth, HeaderBottomY + rowOffsetY);
        }

        public static DxfBox GetSlotBox(int setIndex, ScheduleSlotType type)
        {
            return GetSlotBox(GetItemIndex(setIndex, type)).Move(0.0, GetRowOffsetY(setIndex));
        }

        public static DxfBox GetHeaderBox(int setIndex, ScheduleSlotType type)
        {
            return GetHeaderBox(GetItemIndex(setIndex, type)).Move(0.0, GetRowOffsetY(setIndex));
        }

        public static DxfBox GetSectionBox(int setIndex, ScheduleSlotType type)
        {
            return GetSectionBox(GetItemIndex(setIndex, type)).Move(0.0, GetRowOffsetY(setIndex));
        }

        public static DxfBox GetPositionBox(int setIndex, ScheduleSlotType type)
        {
            var section = GetSectionBox(setIndex, type);
            var rowOffsetY = GetRowOffsetY(setIndex);

            return new DxfBox(section.Left, SectionTopY + rowOffsetY, section.Right, MemberForceTopY + rowOffsetY);
        }

        public static DxfBox GetMemberForceBox(int setIndex, ScheduleSlotType type)
        {
            var section = GetSectionBox(setIndex, type);
            var rowOffsetY = GetRowOffsetY(setIndex);

            return new DxfBox(section.Left, MemberForceTopY + rowOffsetY, section.Right, MemberForceBottomY + rowOffsetY);
        }

        public static DxfBox GetTopRebarBox(int setIndex, ScheduleSlotType type)
        {
            return GetTopRebarBox(GetItemIndex(setIndex, type)).Move(0.0, GetRowOffsetY(setIndex));
        }

        public static DxfBox GetBottomRebarBox(int setIndex, ScheduleSlotType type)
        {
            return GetBottomRebarBox(GetItemIndex(setIndex, type)).Move(0.0, GetRowOffsetY(setIndex));
        }

        public static DxfBox GetStirrupBox(int setIndex, ScheduleSlotType type)
        {
            return GetStirrupBox(GetItemIndex(setIndex, type)).Move(0.0, GetRowOffsetY(setIndex));
        }

        public static DxfBox GetSkinBox(int setIndex, ScheduleSlotType type)
        {
            return GetSkinBox(GetItemIndex(setIndex, type)).Move(0.0, GetRowOffsetY(setIndex));
        }

        public static double GetSlotCenterX(int setIndex, ScheduleSlotType type)
        {
            var box = GetSectionBox(setIndex, type);
            return box.CenterX;
        }

        public static DxfBox GetSlotBox(int index)
        {
            var left = LabelWidth + SlotWidth * index;

            return new DxfBox(left, HeaderTopY, left + SlotWidth, SkinBottomY);
        }

        public static DxfBox GetHeaderBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, HeaderTopY, slot.Right, HeaderBottomY);
        }

        public static DxfBox GetSectionBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, SectionTopY, slot.Right, SectionBottomY);
        }

        public static DxfBox GetTopRebarBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, TopRebarTopY, slot.Right, TopRebarBottomY);
        }

        public static DxfBox GetBottomRebarBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, BottomRebarTopY, slot.Right, BottomRebarBottomY);
        }

        public static DxfBox GetStirrupBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, StirrupTopY, slot.Right, StirrupBottomY);
        }

        public static DxfBox GetSkinBox(int index)
        {
            var slot = GetSlotBox(index);

            return new DxfBox(slot.Left, SkinTopY, slot.Right, SkinBottomY);
        }
    }
}