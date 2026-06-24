using ClosedXML.Excel;
using GirderSchedule.Domain.Formatting;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;

namespace GirderSchedule.Excel
{
    public sealed class ExcelValueMapper
    {
        private readonly ScheduleItemQuery _query = new ScheduleItemQuery();
        private readonly ScheduleValueFormatter _formatter = new ScheduleValueFormatter();

        public ScheduleItem? GetBaseItem(ScheduleSet? set)
        {
            return _query.GetBaseItem(set);
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
            return ToCellValue(_query.GetTopFirstCount(item));
        }

        public XLCellValue GetTopSecondCount(ScheduleItem? item)
        {
            return ToCellValue(_query.GetTopSecondCount(item));
        }

        public XLCellValue GetBottomFirstCount(ScheduleItem? item)
        {
            return ToCellValue(_query.GetBottomFirstCount(item));
        }

        public XLCellValue GetBottomSecondCount(ScheduleItem? item)
        {
            return ToCellValue(_query.GetBottomSecondCount(item));
        }

        public XLCellValue GetStirrupLegs(ScheduleItem? item)
        {
            return ToCellValue(_query.GetStirrupLegs(item));
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
            return _formatter.FormatNumber(value);
        }
    }
}