using GirderSchedule.App.Infrastructure;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.App.ViewModels.Settings;
using GirderSchedule.App.Views.Settings;
using ScheduleTools.Wpf.Commands;
using ScheduleTools.Wpf.Services;
using ScheduleTools.Wpf.Settings.Views;
using System;
using System.Windows;

namespace GirderSchedule.App.ViewModels.Main
{
    public sealed partial class MainWindowViewModel
    {
        private void InitializeCommands()
        {
            NewProjectCommand = new RelayCommand(_ => NewProject());
            OpenProjectCommand = new RelayCommand(_ => OpenProject());
            SaveProjectCommand = new RelayCommand(_ => SaveProject());
            SaveAsProjectCommand = new RelayCommand(_ => SaveAsProject());
            EditFloorSettingCommand = new RelayCommand(_ => EditFloorSetting());
            EditDxfLayerSettingCommand = new RelayCommand(_ => EditDxfLayerSetting());
            EditAppSettingCommand = new RelayCommand(_ => EditAppSetting());
            AddSetCommand = new RelayCommand(_ => AddSet(), _ => SelectedFloor != null);
            RemoveSetCommand = new RelayCommand(_ => RemoveSet(), _ => SelectedFloor != null && SelectedSet != null);
            ExportCurrentSetCommand = new RelayCommand(_ => ExportProjectDxf());
            ExportExcelCommand = new RelayCommand(_ => ExportProjectExcel());
            CopyScheduleSetCommand = new RelayCommand(_ => CopyScheduleSet(), _ => CanCopyScheduleSet());
            PasteScheduleSetCommand = new RelayCommand(_ => PasteScheduleSet(), _ => CanPasteScheduleSet());
        }

        private void AddSet()
        {
            var node = _scheduleSetEditService.AddSet(_selectedFloor);

            if (node == null)
                return;

            SelectedSet = node;
            IsDirty = true;
        }

        private void RemoveSet()
        {
            if (SelectedFloor == null || SelectedSet == null)
                return;

            var result = DialogService.ShowConfirm(
                Properties.Resources.Title_DeleteMember,
                Properties.Resources.Content_DeleteSelectedMemberConfirm);

            if (result != MessageBoxResult.Yes)
                return;

            SelectedSet = _scheduleSetEditService.RemoveSet(
                SelectedFloor,
                SelectedSet);

            IsDirty = true;
        }

        private void EditFloorSetting()
        {
            var viewModel = new FloorSettingWindowViewModel(Floors)
            {
                SelectedFloor = SelectedFloor
            };

            viewModel.FloorSettingChanged += FloorSettingWindow_FloorSettingChanged;

            try
            {
                var window = new FloorSettingWindow
                {
                    DataContext = viewModel
                };

                AppServices.WindowService.ShowDialog(window);
            }
            finally
            {
                viewModel.FloorSettingChanged -= FloorSettingWindow_FloorSettingChanged;
            }

            _floorSettingEditService.SyncProjectFloors(Project, Floors);
            _floorSettingEditService.EnsureEachFloorHasSet(Floors);

            if (Floors.Count > 0 && SelectedFloor == null)
                SelectedFloor = Floors[0];
        }

        private void EditDxfLayerSetting()
        {
            var originalSettingName = Project.DxfSettingName;

            var viewModel = new DxfLayerSettingWindowViewModel(Project);

            var window = new DxfLayerSettingWindow
            {
                DataContext = viewModel
            };

            if (AppServices.WindowService.ShowDialog(window) == true && Project.DxfSettingName != originalSettingName)
                IsDirty = true;
        }

        private void EditAppSetting()
        {
            var window = new AppSettingWindow
            {
                DataContext = AppServices.CreateAppSettingWindowViewModel()
            };

            AppServices.WindowService.ShowDialog(window);
        }

        private void FloorSettingWindow_FloorSettingChanged(object? sender, EventArgs e)
        {
            if (sender is not FloorSettingWindowViewModel viewModel || viewModel.SelectedFloor == null)
                return;

            _floorSettingEditService.SyncProjectFloors(Project, Floors);
            _floorSettingEditService.EnsureEachFloorHasSet(Floors);
            ApplyFloorSettingToSets(viewModel.SelectedFloor);

            if (Floors.Count > 0 && SelectedFloor == null)
                SelectedFloor = Floors[0];

            IsDirty = true;
        }

        private void ApplyFloorSettingToSets(FloorNodeViewModel floor)
        {
            ArgumentNullException.ThrowIfNull(floor);

            _floorSettingEditService.ApplyFloorSettingToSets(floor);

            if (SelectedSet == null || !floor.Sets.Contains(SelectedSet))
                return;

            SelectedSet.RefreshAll();
            Editor.LoadSet(floor.Model, SelectedSet.Model);
        }

        public void MoveFloor(FloorNodeViewModel source, FloorNodeViewModel target, bool isAfter)
        {
            var isMoved = _scheduleTreeMoveService.MoveFloor(
                Floors,
                source,
                target,
                isAfter);

            SelectedFloor = source;

            if (!isMoved)
                return;

            _floorSettingEditService.SyncProjectFloors(Project, Floors);
            IsDirty = true;
        }

        public void MoveSet(ScheduleSetNodeViewModel source, ScheduleSetNodeViewModel target, bool isAfter)
        {
            var result = _scheduleTreeMoveService.MoveSet(
                Floors,
                source,
                target,
                isAfter);

            ApplyMoveSetResult(result);
        }

        public void MoveSetToFloorAroundFloor(ScheduleSetNodeViewModel source, FloorNodeViewModel targetFloor, bool isAfter)
        {
            var result = _scheduleTreeMoveService.MoveSetToFloorAroundFloor(
                Floors,
                source,
                targetFloor,
                isAfter);

            ApplyMoveSetResult(result);
        }

        public void MoveSetToFloor(ScheduleSetNodeViewModel source, FloorNodeViewModel targetFloor)
        {
            var result = _scheduleTreeMoveService.MoveSetToFloor(
                Floors,
                source,
                targetFloor);

            ApplyMoveSetResult(result);
        }

        private void ApplyMoveSetResult(MoveSetResult result)
        {
            if (result == null || !result.IsSuccess)
                return;

            SelectedFloor = result.SelectedFloor;
            SelectedSet = result.SelectedSet;

            if (result.IsChanged)
                IsDirty = true;
        }

        private void ExportProjectDxf()
        {
            if (!SaveProjectBeforeExport())
                return;

            _exportService.ExportDxf(Project, Floors);
        }

        private void ExportProjectExcel()
        {
            if (!SaveProjectBeforeExport())
                return;

            _exportService.ExportExcel(Project, Floors);
        }

        private bool CanCopyScheduleSet()
        {
            return _scheduleSetEditService.CanCopy(SelectedSet);
        }

        private bool CanPasteScheduleSet()
        {
            return _scheduleSetEditService.CanPaste(SelectedSet);
        }

        private void CopyScheduleSet()
        {
            _scheduleSetEditService.Copy(SelectedSet);
        }

        private void PasteScheduleSet()
        {
            if (!_scheduleSetEditService.Paste(SelectedSet))
                return;

            var parentFloor = FindFloorBySet(SelectedSet);

            if (parentFloor != null)
                Editor.LoadSet(parentFloor.Model, SelectedSet.Model);

            IsDirty = true;
        }

        private void Editor_CurrentSetChanged(object? sender, EventArgs e)
        {
            SelectedSet?.RefreshAll();
            IsDirty = true;
        }
    }
}