using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class FloorSettingApplyService
    {
        public void Apply(ScheduleSet set, FloorSetting setting)
        {
            if (set == null || setting == null)
            {
                return;
            }

            Apply(set.Left, setting);
            Apply(set.Center, setting);
            Apply(set.Right, setting);
        }

        public void Apply(ScheduleItem item, FloorSetting setting)
        {
            if (item == null || setting == null)
            {
                return;
            }

            ApplyMainRebarDiameter(item, setting.MainRebarDiameter);
            ApplyStirrupDiameter(item, setting.StirrupDiameter);
            ApplySkinRebarDiameter(item, setting.SkinRebarDiameter);
        }

        public void Apply(SectionItemViewModel item, FloorSetting setting)
        {
            if (item == null || setting == null)
            {
                return;
            }

            if (setting.MainRebarDiameter > 0)
            {
                item.TopDiameter = setting.MainRebarDiameter;
                item.BottomDiameter = setting.MainRebarDiameter;
            }

            if (setting.StirrupDiameter > 0)
            {
                item.StirrupDiameter = setting.StirrupDiameter;
            }

            if (setting.SkinRebarDiameter > 0)
            {
                item.SkinRebarDiameter = setting.SkinRebarDiameter;
            }
        }

        private void ApplyMainRebarDiameter(ScheduleItem item, double diameter)
        {
            if (diameter <= 0)
            {
                return;
            }

            if (item.MainRebar != null)
            {
                item.MainRebar.Diameter = diameter;
            }
        }

        private void ApplyStirrupDiameter(ScheduleItem item, double diameter)
        {
            if (diameter <= 0)
            {
                return;
            }

            if (item.Stirrup != null)
            {
                item.Stirrup.Diameter = diameter;
            }
        }

        private void ApplySkinRebarDiameter(ScheduleItem item, double diameter)
        {
            if (diameter <= 0)
            {
                return;
            }

            if (item.SkinRebar != null)
            {
                item.SkinRebar.Diameter = diameter;
            }
        }
    }
}