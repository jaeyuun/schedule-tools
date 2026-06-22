using ClosedXML.Excel;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.Excel
{
    public sealed class ExcelValueMapper
    {
        public ScheduleItem? GetBaseItem(ScheduleSet? set)
        {
            if (set == null)
            {
                return null;
            }

            if (set.Left != null && set.Left.Section != null)
            {
                return set.Left;
            }

            if (set.Center != null && set.Center.Section != null)
            {
                return set.Center;
            }

            if (set.Right != null && set.Right.Section != null)
            {
                return set.Right;
            }

            return null;
        }

        public XLCellValue GetMemberId(ScheduleSet? set)
        {
            return EmptyCell();
        }

        public XLCellValue GetMemberName(ScheduleSet? set)
        {
            if (set == null || string.IsNullOrWhiteSpace(set.MemberName))
            {
                return "부재명 없음";
            }

            return set.MemberName;
        }

        public XLCellValue GetWidth(ScheduleItem? item)
        {
            return item == null || item.Section == null || item.Section.Width <= 0 ? EmptyCell() : item.Section.Width;
        }

        public XLCellValue GetHeight(ScheduleItem? item)
        {
            return item == null || item.Section == null || item.Section.Height <= 0 ? EmptyCell() : item.Section.Height;
        }

        public XLCellValue GetMainDiameter(ScheduleItem? item)
        {
            var top = item == null ? null : item.TopRebar;
            var bottom = item == null ? null : item.BottomRebar;

            if (top != null && top.Diameter > 0)
            {
                return "D" + FormatNumber(top.Diameter);
            }

            if (bottom != null && bottom.Diameter > 0)
            {
                return "D" + FormatNumber(bottom.Diameter);
            }

            return EmptyCell();
        }

        public XLCellValue GetStirrupDiameter(ScheduleItem? item)
        {
            if (item == null || item.Stirrup == null || item.Stirrup.Diameter <= 0)
            {
                return EmptyCell();
            }

            return "D" + FormatNumber(item.Stirrup.Diameter);
        }

        public XLCellValue GetTopFirstCount(ScheduleItem? item)
        {
            var value = item == null || item.TopRebar == null || item.TopRebar.FirstLayer == null ? 0 : item.TopRebar.FirstLayer.Count;
            return ToCellValue(value);
        }

        public XLCellValue GetTopSecondCount(ScheduleItem? item)
        {
            var value = item == null || item.TopRebar == null || item.TopRebar.SecondLayer == null ? 0 : item.TopRebar.SecondLayer.Count;
            return ToCellValue(value);
        }

        public XLCellValue GetBottomFirstCount(ScheduleItem? item)
        {
            var value = item == null || item.BottomRebar == null || item.BottomRebar.FirstLayer == null ? 0 : item.BottomRebar.FirstLayer.Count;
            return ToCellValue(value);
        }

        public XLCellValue GetBottomSecondCount(ScheduleItem? item)
        {
            var value = item == null || item.BottomRebar == null || item.BottomRebar.SecondLayer == null ? 0 : item.BottomRebar.SecondLayer.Count;
            return ToCellValue(value);
        }

        public XLCellValue GetStirrupLegs(ScheduleItem? item)
        {
            var value = item == null || item.Stirrup == null ? 0 : item.Stirrup.Legs;
            return ToCellValue(value);
        }

        public XLCellValue GetStirrupSpacing(ScheduleItem? item)
        {
            return item == null || item.Stirrup == null || item.Stirrup.Spacing <= 0 ? EmptyCell() : item.Stirrup.Spacing;
        }

        private XLCellValue ToCellValue(int value)
        {
            return value <= 0 ? EmptyCell() : value;
        }

        private XLCellValue EmptyCell()
        {
            return string.Empty;
        }

        private string FormatNumber(double value)
        {
            return value.ToString("0");
        }
    }
}