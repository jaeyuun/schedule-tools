using GirderSchedule.App.ViewModels.Export;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Models;
using GirderSchedule.Dxf.Export;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleExportPageBuilder
    {
        private const int SetsPerPage = 9;
        private const int SetsPerRow = 3;

        public List<ScheduleExportPage> Build(string scheduleTitle, ObservableCollection<FloorNodeViewModel> floors, DxfExportSetting setting)
        {
            if (setting == null)
            {
                setting = new DxfExportSetting();
            }

            if (setting.ContinueAcrossFloors)
            {
                return BuildContinuous(scheduleTitle, floors);
            }

            return BuildFloorSeparated(scheduleTitle, floors);
        }

        private List<ScheduleExportPage> BuildContinuous(string scheduleTitle, ObservableCollection<FloorNodeViewModel> floors)
        {
            var sets = new List<ScheduleSet>();

            if (floors == null)
            {
                return BuildPages(scheduleTitle, sets);
            }

            for (var i = 0; i < floors.Count; i++)
            {
                AddCheckedSets(sets, floors[i]);
            }

            return BuildPages(scheduleTitle, sets);
        }

        private List<ScheduleExportPage> BuildFloorSeparated(string scheduleTitle, ObservableCollection<FloorNodeViewModel> floors)
        {
            var sets = new List<ScheduleSet>();

            if (floors == null)
            {
                return BuildPages(scheduleTitle, sets);
            }

            for (var i = 0; i < floors.Count; i++)
            {
                var countBefore = sets.Count;
                AddCheckedSets(sets, floors[i]);
                var floorSetCount = sets.Count - countBefore;

                if (floorSetCount == 0)
                {
                    continue;
                }

                AddBlankSetsToCompleteRow(sets);
            }

            RemoveTrailingBlankSets(sets);
            return BuildPages(scheduleTitle, sets);
        }

        private void AddCheckedSets(List<ScheduleSet> sets, FloorNodeViewModel floor)
        {
            if (sets == null || floor == null || !floor.IsChecked)
            {
                return;
            }

            for (var i = 0; i < floor.Sets.Count; i++)
            {
                var setNode = floor.Sets[i];

                if (setNode == null || !setNode.IsChecked)
                {
                    continue;
                }

                sets.Add(setNode.Model);
            }
        }

        private void AddBlankSetsToCompleteRow(List<ScheduleSet> sets)
        {
            var remainder = sets.Count % SetsPerRow;

            if (remainder == 0)
            {
                return;
            }

            var blankCount = SetsPerRow - remainder;

            for (var i = 0; i < blankCount; i++)
            {
                sets.Add(null);
            }
        }

        private void RemoveTrailingBlankSets(List<ScheduleSet> sets)
        {
            while (sets.Count > 0 && sets[sets.Count - 1] == null)
            {
                sets.RemoveAt(sets.Count - 1);
            }
        }

        private List<ScheduleExportPage> BuildPages(string scheduleTitle, List<ScheduleSet> sets)
        {
            var pages = new List<ScheduleExportPage>();

            if (sets == null || sets.Count == 0)
            {
                return pages;
            }

            var title = string.IsNullOrWhiteSpace(scheduleTitle) ? "보 일람표" : scheduleTitle;
            var pageNumber = 1;

            for (var start = 0; start < sets.Count; start += SetsPerPage)
            {
                var page = new ScheduleExportPage();
                page.SheetTitle = title + "-" + pageNumber;
                page.FloorName = string.Empty;
                page.SheetTitleName = title + "-" + pageNumber;

                var count = Math.Min(SetsPerPage, sets.Count - start);

                for (var i = 0; i < count; i++)
                {
                    page.Sets.Add(sets[start + i]);
                }

                pages.Add(page);
                pageNumber++;
            }

            return pages;
        }
    }
}
