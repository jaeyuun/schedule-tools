using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;
using ScheduleTools.Core.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GirderSchedule.App.Services.Previews
{
    public sealed class SchedulePreviewPageService
    {
        private readonly Paginator<ScheduleSet> _paginator = new Paginator<ScheduleSet>();

        public int Count => _paginator.Count;

        public void Rebuild(ObservableCollection<FloorNodeViewModel> floors)
        {
            var sets = new List<ScheduleSet>();

            if (floors != null)
            {
                for (var i = 0; i < floors.Count; i++)
                {
                    AddFloor(floors[i], sets);
                }
            }

            _paginator.Reset(sets);
        }

        public List<ScheduleSet> GetPageSets(int pageIndex, int pageSize)
        {
            return _paginator.GetPageItems(pageIndex, pageSize);
        }

        public int GetPageCount(int pageSize)
        {
            return _paginator.GetPageCount(pageSize);
        }

        public int NormalizePageIndex(int pageIndex, int pageSize)
        {
            return _paginator.NormalizePageIndex(pageIndex, pageSize);
        }

        private static void AddFloor(FloorNodeViewModel floor, List<ScheduleSet> sets)
        {
            if (floor == null || !floor.IsChecked || floor.Sets == null)
            {
                return;
            }

            for (var i = 0; i < floor.Sets.Count; i++)
            {
                AddSet(floor.Sets[i], sets);
            }
        }

        private static void AddSet(ScheduleSetNodeViewModel setNode, List<ScheduleSet> sets)
        {
            if (setNode == null || !setNode.IsChecked || setNode.Model == null)
            {
                return;
            }

            sets.Add(setNode.Model);
        }
    }
}
