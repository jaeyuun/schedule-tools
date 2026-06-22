using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;
using System;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleSetFactory
    {
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
            ApplySetting(set.Left, setting);
            ApplySetting(set.Center, setting);
            ApplySetting(set.Right, setting);

            return set;
        }

        private void ApplyMemberName(ScheduleSet set, string memberName)
        {
            if (set == null)
            {
                return;
            }

            var name = string.IsNullOrWhiteSpace(memberName) ? "G1" : memberName;
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

        private void ApplySetting(ScheduleItem item, FloorSetting setting)
        {
            if (item == null || setting == null)
            {
                return;
            }

            if (setting.MainRebarDiameter > 0)
            {
                item.TopRebar.Diameter = setting.MainRebarDiameter;
                item.BottomRebar.Diameter = setting.MainRebarDiameter;
            }

            if (setting.StirrupDiameter > 0)
            {
                item.Stirrup.Diameter = setting.StirrupDiameter;
            }

            if (setting.SkinRebarDiameter > 0)
            {
                var spacing = GetSkinRebarSpacing(item.SkinRebarText);
                item.SkinRebarText = string.IsNullOrWhiteSpace(spacing) ? "HD" + setting.SkinRebarDiameter : "HD" + setting.SkinRebarDiameter + "@" + spacing;
            }
        }

        private string GetSkinRebarSpacing(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !text.Contains("@"))
            {
                return string.Empty;
            }

            return text.Substring(text.IndexOf("@") + 1).Trim();
        }
    }
}
