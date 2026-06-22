using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;
using System.Collections.ObjectModel;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleExcelProjectBuilder
    {
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

                var sourceFloor = floorNode.Model;
                var exportFloor = new ScheduleFloor();
                exportFloor.Name = sourceFloor.Name;
                exportFloor.Setting = sourceFloor.Setting;

                for (var j = 0; j < floorNode.Sets.Count; j++)
                {
                    var setNode = floorNode.Sets[j];

                    if (setNode == null || !setNode.IsChecked || setNode.Model == null)
                    {
                        continue;
                    }

                    exportFloor.Sets.Add(setNode.Model);
                }

                if (exportFloor.Sets.Count == 0)
                {
                    continue;
                }

                exportProject.Floors.Add(exportFloor);
            }

            return exportProject;
        }
    }
}
