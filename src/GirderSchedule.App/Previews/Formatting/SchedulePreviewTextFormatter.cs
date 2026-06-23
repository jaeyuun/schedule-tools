using System.Globalization;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.Preview.Formatting
{
    public sealed class SchedulePreviewTextFormatter
    {
        public string FormatMemberName(ScheduleSet set)
        {
            var memberName = set == null || string.IsNullOrWhiteSpace(set.MemberName) ? "부재명 없음" : set.MemberName;
            var sizeText = FormatSetSectionSize(set);

            return string.IsNullOrWhiteSpace(sizeText) ? memberName : memberName + "\r\n" + sizeText;
        }

        public string FormatSetSectionSize(ScheduleSet set)
        {
            var item = GetFirstEnabledItem(set);

            if (item == null || item.Section == null)
            {
                return string.Empty;
            }

            return "(" + FormatNumber(item.Section.Width) + " x " + FormatNumber(item.Section.Height) + ")";
        }

        public string FormatMomentForce(ScheduleItem item)
        {
            if (item == null || item.MemberForce == null)
            {
                return string.Empty;
            }

            var value = FormatValue(item.MemberForce.Moment);
            return string.IsNullOrWhiteSpace(value) ? string.Empty : "M = " + value;
        }

        public string FormatShearForce(ScheduleItem item)
        {
            if (item == null || item.MemberForce == null)
            {
                return string.Empty;
            }

            var value = FormatValue(item.MemberForce.Shear);
            return string.IsNullOrWhiteSpace(value) ? string.Empty : "V = " + value;
        }

        public string FormatTop(ScheduleItem item)
        {
            if (item == null || item.TopRebar == null)
            {
                return string.Empty;
            }

            var count = GetTopFirstCount(item) + GetTopSecondCount(item);
            return FormatRebar(count, item.TopRebar.Diameter);
        }

        public string FormatBottom(ScheduleItem item)
        {
            if (item == null || item.BottomRebar == null)
            {
                return string.Empty;
            }

            var count = GetBottomFirstCount(item) + GetBottomSecondCount(item);
            return FormatRebar(count, item.BottomRebar.Diameter);
        }

        private static string FormatRebar(int count, double diameter)
        {
            if (count <= 0 || diameter <= 0)
            {
                return string.Empty;
            }

            return count + " - HD " + FormatNumber(diameter);
        }

        public string FormatStirrup(ScheduleItem item)
        {
            if (item == null || item.Stirrup == null)
            {
                return string.Empty;
            }

            if (item.Stirrup.Legs <= 0 || item.Stirrup.Diameter <= 0 || item.Stirrup.Spacing <= 0)
            {
                return string.Empty;
            }

            return item.Stirrup.Legs + "- HD" + FormatNumber(item.Stirrup.Diameter) + "@" + FormatNumber(item.Stirrup.Spacing);
        }

        public string FormatSkinRebar(ScheduleItem item)
        {
            if (item == null || item.SkinRebar == null)
            {
                return string.Empty;
            }

            if (item.SkinRebar.Diameter <= 0 || item.SkinRebar.Spacing <= 0)
            {
                return string.Empty;
            }

            return "HD" + FormatNumber(item.SkinRebar.Diameter) + "@" + FormatNumber(item.SkinRebar.Spacing);
        }

        private static ScheduleItem GetFirstEnabledItem(ScheduleSet set)
        {
            if (set == null)
            {
                return null;
            }

            if (set.Left != null && set.Left.IsSectionEnabled && set.Left.Section != null)
            {
                return set.Left;
            }

            if (set.Center != null && set.Center.IsSectionEnabled && set.Center.Section != null)
            {
                return set.Center;
            }

            if (set.Right != null && set.Right.IsSectionEnabled && set.Right.Section != null)
            {
                return set.Right;
            }

            return null;
        }

        private static int GetTopFirstCount(ScheduleItem item)
        {
            return item == null || item.TopRebar == null || item.TopRebar.FirstLayer == null ? 0 : item.TopRebar.FirstLayer.Count;
        }

        private static int GetTopSecondCount(ScheduleItem item)
        {
            return item == null || item.TopRebar == null || item.TopRebar.SecondLayer == null ? 0 : item.TopRebar.SecondLayer.Count;
        }

        private static int GetBottomFirstCount(ScheduleItem item)
        {
            return item == null || item.BottomRebar == null || item.BottomRebar.FirstLayer == null ? 0 : item.BottomRebar.FirstLayer.Count;
        }

        private static int GetBottomSecondCount(ScheduleItem item)
        {
            return item == null || item.BottomRebar == null || item.BottomRebar.SecondLayer == null ? 0 : item.BottomRebar.SecondLayer.Count;
        }

        private static string FormatValue(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return string.Format(CultureInfo.InvariantCulture, "{0:0}", value);
        }

        private static string FormatNumber(double value)
        {
            return value.ToString("0", CultureInfo.InvariantCulture);
        }
    }
}
