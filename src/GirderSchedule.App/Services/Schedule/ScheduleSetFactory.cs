using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Settings;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleSetFactory
    {
        private readonly FloorSettingApplyService _floorSettingApplyService = new FloorSettingApplyService();

        public ScheduleSet Create(string memberName, FloorSetting setting)
        {
            var set = CreateDefaultSet();
            ApplyMemberName(set, memberName);
            _floorSettingApplyService.Apply(set, setting);

            return set;
        }

        private ScheduleSet CreateDefaultSet()
        {
            var set = new ScheduleSet();

            set.MemberName = "G1";
            set.Left = CreateItem("END");
            set.Center = CreateItem("CEN");
            set.Right = CreateItem("END");

            return set;
        }

        private ScheduleItem CreateItem(string position)
        {
            var item = new ScheduleItem();

            item.Position = position;
            item.IsSectionEnabled = true;

            item.Section.Width = 400.0;
            item.Section.Height = 600.0;

            item.MainRebar.Diameter = 0.0;
            item.MainRebar.Top.FirstCount = 0;
            item.MainRebar.Top.SecondCount = 0;

            item.MainRebar.Diameter = 0.0;
            item.MainRebar.Bottom.FirstCount = 0;
            item.MainRebar.Bottom.SecondCount = 0;

            item.Stirrup.Legs = 2;
            item.Stirrup.Diameter = 0.0;
            item.Stirrup.Spacing = 0;

            item.SkinRebar.Diameter = 0.0;
            item.SkinRebar.Spacing = 0;

            return item;
        }

        private void ApplyMemberName(ScheduleSet set, string memberName)
        {
            if (set == null)
            {
                return;
            }

            set.MemberName = string.IsNullOrWhiteSpace(memberName) ? "G1" : memberName;
        }
    }
}