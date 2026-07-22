using GirderSchedule.App.ViewModels.Schedule;
using System;
using System.Collections.ObjectModel;

namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleTreeMoveService
    {
        public bool MoveFloor(ObservableCollection<FloorNodeViewModel> floors, FloorNodeViewModel source, FloorNodeViewModel target, bool isAfter)
        {
            if (floors == null || source == null || target == null || ReferenceEquals(source, target))
            {
                return false;
            }

            var oldIndex = floors.IndexOf(source);
            var insertIndex = floors.IndexOf(target);

            if (oldIndex < 0 || insertIndex < 0)
            {
                return false;
            }

            if (isAfter)
            {
                insertIndex++;
            }

            if (insertIndex > oldIndex)
            {
                insertIndex--;
            }

            insertIndex = Clamp(insertIndex, 0, floors.Count - 1);

            if (oldIndex == insertIndex)
            {
                return false;
            }

            floors.Move(oldIndex, insertIndex);
            return true;
        }

        public MoveSetResult MoveSet(ObservableCollection<FloorNodeViewModel> floors, ScheduleSetNodeViewModel source, ScheduleSetNodeViewModel target, bool isAfter)
        {
            if (floors == null || source == null || target == null || ReferenceEquals(source, target))
            {
                return MoveSetResult.Fail();
            }

            var sourceFloor = FindFloorBySet(floors, source);
            var targetFloor = FindFloorBySet(floors, target);

            if (sourceFloor == null || targetFloor == null)
            {
                return MoveSetResult.Fail();
            }

            var insertIndex = targetFloor.Sets.IndexOf(target);

            if (insertIndex < 0)
            {
                return MoveSetResult.Fail();
            }

            if (isAfter)
            {
                insertIndex++;
            }

            return MoveSetToFloorIndex(source, sourceFloor, targetFloor, insertIndex);
        }

        public MoveSetResult MoveSetToFloorAroundFloor(ObservableCollection<FloorNodeViewModel> floors, ScheduleSetNodeViewModel source, FloorNodeViewModel targetFloor, bool isAfter)
        {
            if (floors == null || source == null || targetFloor == null)
            {
                return MoveSetResult.Fail();
            }

            var sourceFloor = FindFloorBySet(floors, source);

            if (sourceFloor == null)
            {
                return MoveSetResult.Fail();
            }

            var insertIndex = isAfter ? targetFloor.Sets.Count : 0;
            return MoveSetToFloorIndex(source, sourceFloor, targetFloor, insertIndex);
        }

        public MoveSetResult MoveSetToFloor(ObservableCollection<FloorNodeViewModel> floors, ScheduleSetNodeViewModel source, FloorNodeViewModel targetFloor)
        {
            if (floors == null || source == null || targetFloor == null)
            {
                return MoveSetResult.Fail();
            }

            var sourceFloor = FindFloorBySet(floors, source);

            if (sourceFloor == null)
            {
                return MoveSetResult.Fail();
            }

            return MoveSetToFloorIndex(source, sourceFloor, targetFloor, targetFloor.Sets.Count);
        }

        private MoveSetResult MoveSetToFloorIndex(ScheduleSetNodeViewModel source, FloorNodeViewModel sourceFloor, FloorNodeViewModel targetFloor, int insertIndex)
        {
            var oldIndex = sourceFloor.Sets.IndexOf(source);

            if (oldIndex < 0)
            {
                return MoveSetResult.Fail();
            }

            if (ReferenceEquals(sourceFloor, targetFloor) && insertIndex > oldIndex)
            {
                insertIndex--;
            }

            insertIndex = Clamp(insertIndex, 0, targetFloor.Sets.Count);

            if (ReferenceEquals(sourceFloor, targetFloor) && oldIndex == insertIndex)
            {
                return MoveSetResult.NotChanged(source, targetFloor);
            }

            sourceFloor.Sets.Remove(source);
            sourceFloor.Model.Sets.Remove(source.Model);

            insertIndex = Clamp(insertIndex, 0, targetFloor.Sets.Count);

            source.Parent = targetFloor;
            targetFloor.Sets.Insert(insertIndex, source);
            targetFloor.Model.Sets.Insert(insertIndex, source.Model);

            sourceFloor.UpdateCheckedFromChildren();
            targetFloor.UpdateCheckedFromChildren();

            return MoveSetResult.Success(source, targetFloor);
        }

        private FloorNodeViewModel? FindFloorBySet(ObservableCollection<FloorNodeViewModel> floors, ScheduleSetNodeViewModel? set)
        {
            if (set == null)
            {
                return null;
            }

            for (var i = 0; i < floors.Count; i++)
            {
                if (floors[i].Sets.Contains(set))
                {
                    return floors[i];
                }
            }

            return null;
        }

        private int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }
    }

    public sealed class MoveSetResult
    {
        public bool IsSuccess { get; private set; }
        public bool IsChanged { get; private set; }
        public ScheduleSetNodeViewModel? SelectedSet { get; private set; }
        public FloorNodeViewModel? SelectedFloor { get; private set; }

        private MoveSetResult()
        {
        }

        public static MoveSetResult Success(ScheduleSetNodeViewModel selectedSet, FloorNodeViewModel selectedFloor)
        {
            return new MoveSetResult
            {
                IsSuccess = true,
                IsChanged = true,
                SelectedSet = selectedSet,
                SelectedFloor = selectedFloor
            };
        }

        public static MoveSetResult NotChanged(ScheduleSetNodeViewModel selectedSet, FloorNodeViewModel selectedFloor)
        {
            return new MoveSetResult
            {
                IsSuccess = true,
                IsChanged = false,
                SelectedSet = selectedSet,
                SelectedFloor = selectedFloor
            };
        }

        public static MoveSetResult Fail()
        {
            return new MoveSetResult();
        }
    }
}
