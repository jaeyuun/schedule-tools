using GirderSchedule.App.Commands;
using GirderSchedule.App.Services;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.App.ViewModels.Settings;
using GirderSchedule.App.Views.Settings;
using System;
using System.Windows;

namespace GirderSchedule.App.ViewModels.Main
{
    public sealed partial class MainWindowViewModel
    {
        private void InitializeCommands()
        {
            NewProjectCommand = new RelayCommand(p => NewProject());
            OpenProjectCommand = new RelayCommand(p => OpenProject());
            SaveProjectCommand = new RelayCommand(p => SaveProject());
            SaveAsProjectCommand = new RelayCommand(p => SaveAsProject());
            EditFloorSettingCommand = new RelayCommand(p => EditFloorSetting());
            AddSetCommand = new RelayCommand(p => AddSet(), p => SelectedFloor != null);
            RemoveSetCommand = new RelayCommand(p => RemoveSet(), p => SelectedFloor != null && SelectedSet != null);
            ExportCurrentSetCommand = new RelayCommand(p => ExportProjectDxf());
            ExportExcelCommand = new RelayCommand(p => ExportProjectExcel());
            HelpCommand = new RelayCommand(p => ShowHelp());
            CopyScheduleSetCommand = new RelayCommand(p => CopyScheduleSet(), p => CanCopyScheduleSet());
            PasteScheduleSetCommand = new RelayCommand(p => PasteScheduleSet(), p => CanPasteScheduleSet());
        }

        private void AddSet()
        {
            var node = _scheduleSetEditService.AddSet(_selectedFloor);
            if (node == null)
            {
                return;
            }

            SelectedSet = node;
            IsDirty = true;
        }

        private void RemoveSet()
        {
            if (SelectedFloor == null || SelectedSet == null)
            {
                return;
            }

            var result = AppDialogService.ShowConfirm(
                Properties.Resources.Title_DeleteMember,
                Properties.Resources.Content_DeleteSelectedMemberConfirm);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            SelectedSet = _scheduleSetEditService.RemoveSet(SelectedFloor, SelectedSet);
            IsDirty = true;
        }

        private void EditFloorSetting()
        {
            var viewModel = new FloorSettingWindowViewModel(Floors);
            viewModel.SelectedFloor = SelectedFloor;
            viewModel.FloorSettingChanged += FloorSettingWindow_FloorSettingChanged;

            var window = new FloorSettingWindow();
            window.Owner = Application.Current.MainWindow;
            window.DataContext = viewModel;
            window.ShowDialog();

            viewModel.FloorSettingChanged -= FloorSettingWindow_FloorSettingChanged;

            _floorSettingEditService.SyncProjectFloors(Project, Floors);
            _floorSettingEditService.EnsureEachFloorHasSet(Floors);

            if (Floors.Count > 0 && SelectedFloor == null)
            {
                SelectedFloor = Floors[0];
            }
        }

        private void FloorSettingWindow_FloorSettingChanged(object sender, EventArgs e)
        {
            var viewModel = sender as FloorSettingWindowViewModel;

            if (viewModel == null || viewModel.SelectedFloor == null)
            {
                return;
            }

            _floorSettingEditService.SyncProjectFloors(Project, Floors);
            _floorSettingEditService.EnsureEachFloorHasSet(Floors);
            ApplyFloorSettingToSets(viewModel.SelectedFloor);

            if (Floors.Count > 0 && SelectedFloor == null)
            {
                SelectedFloor = Floors[0];
            }

            IsDirty = true;
        }

        private void ApplyFloorSettingToSets(FloorNodeViewModel floor)
        {
            if (floor == null)
            {
                return;
            }

            _floorSettingEditService.ApplyFloorSettingToSets(floor);

            if (SelectedSet != null && floor.Sets.Contains(SelectedSet))
            {
                SelectedSet.RefreshAll();
                Editor.LoadSet(floor.Model, SelectedSet.Model);
            }
        }

        public void MoveFloor(FloorNodeViewModel source, FloorNodeViewModel target, bool isAfter)
        {
            var isMoved = _scheduleTreeMoveService.MoveFloor(Floors, source, target, isAfter);

            SelectedFloor = source;

            if (!isMoved)
            {
                return;
            }

            _floorSettingEditService.SyncProjectFloors(Project, Floors);
            IsDirty = true;
        }

        public void MoveSet(ScheduleSetNodeViewModel source, ScheduleSetNodeViewModel target, bool isAfter)
        {
            var result = _scheduleTreeMoveService.MoveSet(Floors, source, target, isAfter);
            ApplyMoveSetResult(result);
        }

        public void MoveSetToFloorAroundFloor(ScheduleSetNodeViewModel source, FloorNodeViewModel targetFloor, bool isAfter)
        {
            var result = _scheduleTreeMoveService.MoveSetToFloorAroundFloor(Floors, source, targetFloor, isAfter);
            ApplyMoveSetResult(result);
        }

        public void MoveSetToFloor(ScheduleSetNodeViewModel source, FloorNodeViewModel targetFloor)
        {
            var result = _scheduleTreeMoveService.MoveSetToFloor(Floors, source, targetFloor);
            ApplyMoveSetResult(result);
        }

        private void ApplyMoveSetResult(MoveSetResult result)
        {
            if (result == null || !result.IsSuccess)
            {
                return;
            }

            SelectedFloor = result.SelectedFloor;
            SelectedSet = result.SelectedSet;

            if (result.IsChanged)
            {
                IsDirty = true;
            }
        }

        private void ExportProjectDxf()
        {
            if (!SaveProjectBeforeExport())
            {
                return;
            }

            _exportService.ExportDxf(ProjectName, ScheduleTitle, Floors);
        }

        private void ExportProjectExcel()
        {
            if (!SaveProjectBeforeExport())
            {
                return;
            }

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
            {
                return;
            }

            var parentFloor = FindFloorBySet(SelectedSet);

            if (parentFloor != null)
            {
                Editor.LoadSet(parentFloor.Model, SelectedSet.Model);
            }

            IsDirty = true;
        }

        private void Editor_CurrentSetChanged(object sender, EventArgs e)
        {
            if (SelectedSet != null)
            {
                SelectedSet.RefreshAll();
            }

            IsDirty = true;
        }

        private void ShowHelp()
        {
            AppDialogService.ShowNotice(
                Properties.Resources.Title_Help,
                Properties.Resources.Title_Help);
        }
    }
}