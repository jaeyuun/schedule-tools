using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GirderSchedule.App.Services.Previews
{
    public sealed class SchedulePreviewPageService
    {
        private readonly List<ScheduleSet> _sets = new List<ScheduleSet>();

        public int Count
        {
            get { return _sets.Count; }
        }

        public void Rebuild(ObservableCollection<FloorNodeViewModel> floors)
        {
            _sets.Clear();

            if (floors == null)
            {
                return;
            }

            for (var i = 0; i < floors.Count; i++)
            {
                AddFloor(floors[i]);
            }
        }

        public List<ScheduleSet> GetPageSets(int pageIndex, int pageSize)
        {
            var result = new List<ScheduleSet>();

            if (_sets.Count == 0 || pageSize <= 0)
            {
                return result;
            }

            var start = pageIndex * pageSize;
            var end = Math.Min(start + pageSize, _sets.Count);

            for (var i = start; i < end; i++)
            {
                result.Add(_sets[i]);
            }

            return result;
        }

        public int GetPageCount(int pageSize)
        {
            if (_sets.Count == 0 || pageSize <= 0)
            {
                return 1;
            }

            return (_sets.Count + pageSize - 1) / pageSize;
        }

        public int NormalizePageIndex(int pageIndex, int pageSize)
        {
            var pageCount = GetPageCount(pageSize);

            if (pageIndex >= pageCount)
            {
                return pageCount - 1;
            }

            if (pageIndex < 0)
            {
                return 0;
            }

            return pageIndex;
        }

        private void AddFloor(FloorNodeViewModel floor)
        {
            if (floor == null || !floor.IsChecked || floor.Sets == null)
            {
                return;
            }

            for (var i = 0; i < floor.Sets.Count; i++)
            {
                AddSet(floor.Sets[i]);
            }
        }

        private void AddSet(ScheduleSetNodeViewModel setNode)
        {
            if (setNode == null || !setNode.IsChecked || setNode.Model == null)
            {
                return;
            }

            _sets.Add(setNode.Model);
        }
    }
}