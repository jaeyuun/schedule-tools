using GirderSchedule.App.ViewModels.Schedule;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GirderSchedule.App.Services.Previews
{
    public sealed class SchedulePreviewChangeWatcher
    {
        private ObservableCollection<FloorNodeViewModel> _floors;
        private Action _changed;

        public void Attach(ObservableCollection<FloorNodeViewModel> floors, Action changed)
        {
            Detach();

            _floors = floors;
            _changed = changed;

            if (_floors == null)
            {
                return;
            }

            _floors.CollectionChanged += Floors_CollectionChanged;

            for (var i = 0; i < _floors.Count; i++)
            {
                AttachFloor(_floors[i]);
            }
        }

        public void Detach()
        {
            if (_floors == null)
            {
                _changed = null;
                return;
            }

            _floors.CollectionChanged -= Floors_CollectionChanged;

            for (var i = 0; i < _floors.Count; i++)
            {
                DetachFloor(_floors[i]);
            }

            _floors = null;
            _changed = null;
        }

        private void AttachFloor(FloorNodeViewModel floor)
        {
            if (floor == null)
            {
                return;
            }

            floor.PropertyChanged += Floor_PropertyChanged;

            if (floor.Sets == null)
            {
                return;
            }

            floor.Sets.CollectionChanged += Sets_CollectionChanged;

            for (var i = 0; i < floor.Sets.Count; i++)
            {
                AttachSet(floor.Sets[i]);
            }
        }

        private void DetachFloor(FloorNodeViewModel floor)
        {
            if (floor == null)
            {
                return;
            }

            floor.PropertyChanged -= Floor_PropertyChanged;

            if (floor.Sets == null)
            {
                return;
            }

            floor.Sets.CollectionChanged -= Sets_CollectionChanged;

            for (var i = 0; i < floor.Sets.Count; i++)
            {
                DetachSet(floor.Sets[i]);
            }
        }

        private void AttachSet(ScheduleSetNodeViewModel set)
        {
            if (set == null)
            {
                return;
            }

            set.PropertyChanged += Set_PropertyChanged;
        }

        private void DetachSet(ScheduleSetNodeViewModel set)
        {
            if (set == null)
            {
                return;
            }

            set.PropertyChanged -= Set_PropertyChanged;
        }

        private void Floors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                for (var i = 0; i < e.OldItems.Count; i++)
                {
                    DetachFloor(e.OldItems[i] as FloorNodeViewModel);
                }
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    AttachFloor(e.NewItems[i] as FloorNodeViewModel);
                }
            }

            RaiseChanged();
        }

        private void Sets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                for (var i = 0; i < e.OldItems.Count; i++)
                {
                    DetachSet(e.OldItems[i] as ScheduleSetNodeViewModel);
                }
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    AttachSet(e.NewItems[i] as ScheduleSetNodeViewModel);
                }
            }

            RaiseChanged();
        }

        private void Floor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FloorNodeViewModel.IsChecked) || e.PropertyName == nameof(FloorNodeViewModel.DisplayName) || e.PropertyName == nameof(FloorNodeViewModel.Name))
            {
                RaiseChanged();
            }
        }

        private void Set_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScheduleSetNodeViewModel.IsChecked) || e.PropertyName == nameof(ScheduleSetNodeViewModel.DisplayName) || e.PropertyName == nameof(ScheduleSetNodeViewModel.MemberName))
            {
                RaiseChanged();
            }
        }

        private void RaiseChanged()
        {
            if (_changed == null)
            {
                return;
            }

            _changed();
        }
    }
}