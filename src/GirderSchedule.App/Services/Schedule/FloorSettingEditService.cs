using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Settings;
using System.Collections.ObjectModel;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class FloorSettingEditService
    {
        private readonly ScheduleSetFactory _setFactory;
        private readonly FloorSettingApplyService _floorSettingApplyService;

        public FloorSettingEditService(ScheduleSetFactory setFactory, FloorSettingApplyService floorSettingApplyService)
        {
            _setFactory = setFactory;
            _floorSettingApplyService = floorSettingApplyService;
        }

        public void SyncProjectFloors(ScheduleProject project, ObservableCollection<FloorNodeViewModel> floors)
        {
            if (project == null || floors == null)
            {
                return;
            }

            project.Floors.Clear();

            for (var i = 0; i < floors.Count; i++)
            {
                project.Floors.Add(floors[i].Model);
            }
        }

        public void EnsureEachFloorHasSet(ObservableCollection<FloorNodeViewModel> floors)
        {
            if (floors == null)
            {
                return;
            }

            for (var i = 0; i < floors.Count; i++)
            {
                var floor = floors[i];

                if (floor.Sets.Count > 0)
                {
                    continue;
                }

                var set = _setFactory.Create("G1", floor.Setting);
                floor.AddSet(set);
            }
        }

        public void ApplyFloorSettingToSets(FloorNodeViewModel floor)
        {
            if (floor == null)
            {
                return;
            }

            for (var i = 0; i < floor.Sets.Count; i++)
            {
                ApplyFloorSettingToSet(floor.Sets[i].Model, floor.Setting);
                floor.Sets[i].RefreshAll();
            }

            floor.RefreshAll();
        }

        private void ApplyFloorSettingToSet(ScheduleSet set, FloorSetting setting)
        {
            _floorSettingApplyService.Apply(set, setting);
        }
    }
}