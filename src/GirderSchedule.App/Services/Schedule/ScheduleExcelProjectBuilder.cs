using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Settings;
using System.Collections.ObjectModel;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleExcelProjectBuilder
    {
        private readonly ScheduleDisplayService _displayService = new ScheduleDisplayService();
        private readonly ScheduleSetCopyService _copyService = new ScheduleSetCopyService();

        public ScheduleProject Build(ScheduleProject sourceProject, ObservableCollection<FloorNodeViewModel> floors)
        {
            if (sourceProject == null)
            {
                return null;
            }

            var exportProject = new ScheduleProject();
            exportProject.ProjectName = sourceProject.ProjectName;
            exportProject.ScheduleTitle = sourceProject.ScheduleTitle;

            if (floors == null)
            {
                return exportProject;
            }

            for (var i = 0; i < floors.Count; i++)
            {
                var floorNode = floors[i];

                if (floorNode == null || !floorNode.IsChecked || floorNode.Model == null)
                {
                    continue;
                }

                var exportFloor = new ScheduleFloor();
                exportFloor.Setting = CloneFloorSetting(floorNode.Setting);

                for (var j = 0; j < floorNode.Sets.Count; j++)
                {
                    var setNode = floorNode.Sets[j];

                    if (setNode == null || !setNode.IsChecked || setNode.Model == null)
                    {
                        continue;
                    }

                    var exportSet = _copyService.Clone(setNode.Model);
                    exportSet.MemberName = _displayService.GetMemberName(floorNode.FloorPrefix, setNode.Model.MemberName);

                    exportFloor.Sets.Add(exportSet);
                }

                if (exportFloor.Sets.Count == 0)
                {
                    continue;
                }

                exportProject.Floors.Add(exportFloor);
            }

            return exportProject;
        }

        private FloorSetting CloneFloorSetting(FloorSetting source)
        {
            if (source == null)
            {
                return new FloorSetting();
            }

            var setting = new FloorSetting();
            setting.Prefix = source.Prefix;
            setting.Name = source.Name;
            setting.MainRebarDiameter = source.MainRebarDiameter;
            setting.StirrupDiameter = source.StirrupDiameter;
            setting.SkinRebarDiameter = source.SkinRebarDiameter;

            return setting;
        }
    }
}