using System.Globalization;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;

namespace GirderSchedule.Domain.Formatting
{
    public sealed class ScheduleValueFormatter
    {
        private readonly ScheduleItemQuery _query = new ScheduleItemQuery();

        public string FormatNumber(double value)
        {
            return value.ToString("0", CultureInfo.InvariantCulture);
        }

        public string FormatMemberName(ScheduleSet set)
        {
            if (set == null || string.IsNullOrWhiteSpace(set.MemberName))
            {
                return string.Empty;
            }

            return set.MemberName;
        }

        public string FormatSetSectionSize(ScheduleSet set)
        {
            var item = _query.GetFirstEnabledItem(set);

            if (item == null || item.Section == null)
            {
                return string.Empty;
            }

            return "(" + FormatNumber(item.Section.Width) + " x " + FormatNumber(item.Section.Height) + ")";
        }

        public string FormatSectionNote(ScheduleItem item)
        {
            if (item == null || item.Section == null)
            {
                return string.Empty;
            }

            return string.IsNullOrWhiteSpace(item.Section.Note) ? string.Empty : item.Section.Note;
        }

        public string FormatMomentForce(ScheduleItem item)
        {
            if (item == null || item.MemberForce == null || !item.MemberForce.Moment.HasValue)
            {
                return string.Empty;
            }

            return "M = " + FormatNumber(item.MemberForce.Moment.Value);
        }

        public string FormatShearForce(ScheduleItem item)
        {
            if (item == null || item.MemberForce == null || !item.MemberForce.Shear.HasValue)
            {
                return string.Empty;
            }

            return "V = " + FormatNumber(item.MemberForce.Shear.Value);
        }

        public string FormatTopRebar(ScheduleItem item)
        {
            if (item == null || item.TopRebar == null)
            {
                return "- HD";
            }

            return FormatMainRebar(_query.GetTopTotalCount(item), item.TopRebar.Diameter);
        }

        public string FormatBottomRebar(ScheduleItem item)
        {
            if (item == null || item.BottomRebar == null)
            {
                return "- HD";
            }

            return FormatMainRebar(_query.GetBottomTotalCount(item), item.BottomRebar.Diameter);
        }

        public string FormatMainRebar(int count, double diameter)
        {
            if (count <= 0 || diameter <= 0)
            {
                return "- HD";
            }

            return count + " - HD " + FormatNumber(diameter);
        }

        public string FormatStirrup(ScheduleItem item)
        {
            if (item == null || item.Stirrup == null)
            {
                return "- HD   @";
            }

            if (item.Stirrup.Legs <= 0 || item.Stirrup.Diameter <= 0 || item.Stirrup.Spacing <= 0)
            {
                return "- HD   @";
            }

            return item.Stirrup.Legs + " - HD " + FormatNumber(item.Stirrup.Diameter) + " @ " + FormatNumber(item.Stirrup.Spacing);
        }

        public string FormatSkinRebar(ScheduleItem item)
        {
            if (item == null || item.SkinRebar == null)
            {
                return "-";
            }

            if (item.SkinRebar.Diameter <= 0 || item.SkinRebar.Spacing <= 0)
            {
                return "-";
            }

            var skinRebar = "HD " + FormatNumber(item.SkinRebar.Diameter) + " @ " + FormatNumber(item.SkinRebar.Spacing);

            if (!string.IsNullOrWhiteSpace(item.SkinRebar.Note))
            {
                skinRebar += "\n" + item.SkinRebar.Note;
            }

            return skinRebar;
        }
    }
}