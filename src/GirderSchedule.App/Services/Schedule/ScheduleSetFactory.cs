using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;
using System;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleSetFactory
    {
        private readonly FloorSettingApplyService _floorSettingApplyService = new FloorSettingApplyService();

        public ScheduleSet Create(string memberName, FloorSetting setting)
        {
            var service = new ScheduleBuildService();
            var sheet = service.CreateSample();

            if (sheet == null || sheet.Rows == null || sheet.Rows.Count == 0 || sheet.Rows[0].Sets == null || sheet.Rows[0].Sets.Count == 0)
            {
                throw new InvalidOperationException("기본 보 일람표 샘플을 생성할 수 없습니다.");
            }

            var set = sheet.Rows[0].Sets[0];

            ApplyMemberName(set, memberName);
            _floorSettingApplyService.Apply(set, setting);

            return set;
        }

        private void ApplyMemberName(ScheduleSet set, string memberName)
        {
            if (set == null)
            {
                return;
            }

            var name = string.IsNullOrWhiteSpace(memberName) ? "1G1" : memberName;
            set.MemberName = name;

            if (set.Left != null)
            {
                set.Left.Name = name;
                set.Left.IsSectionEnabled = true;
            }

            if (set.Center != null)
            {
                set.Center.Name = name;
                set.Center.IsSectionEnabled = true;
            }

            if (set.Right != null)
            {
                set.Right.Name = name;
                set.Right.IsSectionEnabled = true;
            }
        }
    }
}