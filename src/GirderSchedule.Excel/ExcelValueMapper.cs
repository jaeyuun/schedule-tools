using ClosedXML.Excel;
using GirderSchedule.Domain.Formatting;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;

namespace GirderSchedule.Excel
{
    public sealed class ExcelValueMapper
    {
        private const double DefaultCover = 40.0;
        private readonly ScheduleItemQuery _query = new ScheduleItemQuery();
        private readonly ScheduleValueFormatter _formatter = new ScheduleValueFormatter();

        public ScheduleItem? GetBaseItem(ScheduleSet? set)
        {
            return _query.GetBaseItem(set);
        }

        public XLCellValue GetMemberId(ScheduleSet? set)
        {
            return string.Empty;
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

        public XLCellValue GetRebarSectionText(ScheduleSet set)
        {
            if (set == null)
            {
                return string.Empty;
            }

            if (IsEnabled(set.Left) && IsEnabled(set.Center) && IsEnabled(set.Right))
            {
                return "EACH";
            }

            if (IsEnabled(set.Left) || IsEnabled(set.Right))
            {
                return "BOTH";
            }

            if (IsEnabled(set.Center))
            {
                return "ALL";
            }

            return string.Empty;
        }

        private bool IsEnabled(ScheduleItem? item)
        {
            return item != null && item.IsSectionEnabled;
        }

        public XLCellValue GetCoverValue(ScheduleItem? item)
        {
            var cover = DefaultCover;
            var mainRebar = item == null ? null : item.MainRebar;
            var stirrup = item == null ? null : item.Stirrup;

            // 40 + STR직경 + MAIN BAR(주근)직경 / 2
            if (stirrup != null && stirrup.Diameter > 0)
            {
                cover += stirrup.Diameter;
            }

            if (mainRebar != null && mainRebar.Diameter > 0)
            {
                cover += mainRebar.Diameter / 2;
            }

            return Math.Round(cover, 1);
        }

        public XLCellValue GetMainDiameter(ScheduleItem? item)
        {
            var mainRebar = item == null ? null : item.MainRebar;

            if (mainRebar != null && mainRebar.Diameter > 0)
            {
                return "D" + FormatNumber(mainRebar.Diameter);
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
            return item == null || item.Stirrup == null || item.Stirrup.Spacing <= 0.0 ? EmptyCell() : item.Stirrup.Spacing;
        }

        private XLCellValue ToCellValue(int value)
        {
            return value <= 0 ? EmptyCell() : value;
        }

        private XLCellValue EmptyCell()
        {
            return 0;
        }

        private string FormatNumber(double value)
        {
            return _formatter.FormatNumber(value);
        }
    }
}