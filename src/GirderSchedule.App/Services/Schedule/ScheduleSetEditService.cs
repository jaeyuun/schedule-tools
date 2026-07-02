using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleSetEditService
    {
        private readonly ScheduleSetFactory _setFactory;
        private readonly ScheduleSetCopyService _copyService;

        private ScheduleSet _copiedScheduleSet;

        public ScheduleSetEditService(ScheduleSetFactory setFactory, ScheduleSetCopyService copyService)
        {
            _setFactory = setFactory;
            _copyService = copyService;
        }

        public ScheduleSetNodeViewModel AddSet(FloorNodeViewModel selectedFloor)
        {
            if (selectedFloor == null)
            {
                return null;
            }

            var memberName = "G" + (selectedFloor.Sets.Count + 1);
            var set = _setFactory.Create(memberName, selectedFloor.Setting);

            return selectedFloor.AddSet(set);
        }

        public ScheduleSetNodeViewModel RemoveSet(FloorNodeViewModel selectedFloor, ScheduleSetNodeViewModel selectedSet)
        {
            if (selectedFloor == null || selectedSet == null)
            {
                return null;
            }

            var index = selectedFloor.Sets.IndexOf(selectedSet);
            selectedFloor.RemoveSet(selectedSet);

            if (selectedFloor.Sets.Count == 0)
            {
                return null;
            }

            if (index >= selectedFloor.Sets.Count)
            {
                index = selectedFloor.Sets.Count - 1;
            }

            return selectedFloor.Sets[index];
        }

        public bool CanCopy(ScheduleSetNodeViewModel selectedSet)
        {
            return selectedSet != null && selectedSet.Model != null;
        }

        public bool CanPaste(ScheduleSetNodeViewModel selectedSet)
        {
            return _copiedScheduleSet != null && selectedSet != null && selectedSet.Model != null;
        }

        public void Copy(ScheduleSetNodeViewModel selectedSet)
        {
            if (!CanCopy(selectedSet))
            {
                return;
            }

            _copiedScheduleSet = _copyService.Clone(selectedSet.Model);
        }

        public bool Paste(ScheduleSetNodeViewModel selectedSet)
        {
            if (!CanPaste(selectedSet))
            {
                return false;
            }

            _copyService.CopyTo(_copiedScheduleSet, selectedSet.Model);
            selectedSet.RefreshAll();

            return true;
        }
    }
}