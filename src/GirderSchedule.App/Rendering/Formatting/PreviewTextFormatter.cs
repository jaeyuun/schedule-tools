using GirderSchedule.Domain.Formatting;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.Rendering.Formatting
{
    public sealed class PreviewTextFormatter
    {
        private readonly ScheduleValueFormatter _formatter = new ScheduleValueFormatter();

        public string FormatMemberName(ScheduleSet? set)
        {
            var memberName = _formatter.FormatMemberName(set);
            var sizeText = _formatter.FormatSetSectionSize(set);

            return string.IsNullOrWhiteSpace(sizeText) ? memberName : memberName + "\r\n" + sizeText;
        }

        public string FormatSetSectionSize(ScheduleSet? set)
        {
            return _formatter.FormatSetSectionSize(set);
        }

        public string FormatSectionNote(ScheduleItem? item)
        {
            return _formatter.FormatSectionNote(item);
        }

        public string FormatMomentForce(ScheduleItem? item)
        {
            return _formatter.FormatMomentForce(item);
        }

        public string FormatShearForce(ScheduleItem? item)
        {
            return _formatter.FormatShearForce(item);
        }

        public string FormatTop(ScheduleItem? item)
        {
            return _formatter.FormatTopRebar(item);
        }

        public string FormatBottom(ScheduleItem? item)
        {
            return _formatter.FormatBottomRebar(item);
        }

        public string FormatStirrup(ScheduleItem? item)
        {
            return _formatter.FormatStirrup(item);
        }

        public string FormatSkinRebar(ScheduleItem? item)
        {
            return _formatter.FormatSkinRebar(item);
        }
    }
}
