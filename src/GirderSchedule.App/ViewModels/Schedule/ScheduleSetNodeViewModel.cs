using GirderSchedule.App.Services.Schedule;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.ViewModels.Schedule
{
    public sealed class ScheduleSetNodeViewModel : ViewModelBase
    {
        private readonly ScheduleDisplayService _displayService = new ScheduleDisplayService();
        private readonly ScheduleSet _model;
        private FloorNodeViewModel _parent;
        private bool _isChecked = true;
        private bool _isSelected;
        private bool _isInternalChanging;

        public ScheduleSet Model
        {
            get { return _model; }
        }

        public FloorNodeViewModel Parent
        {
            get { return _parent; }
            set
            {
                if (_parent == value)
                {
                    return;
                }

                _parent = value;
                OnPropertyChanged(nameof(Parent));
                OnPropertyChanged(nameof(DisplayMemberName));
            }
        }

        public string MemberName
        {
            get { return _model.MemberName; }
            set
            {
                if (_model.MemberName == value)
                {
                    return;
                }

                _model.MemberName = value;
                OnPropertyChanged(nameof(MemberName));
                OnPropertyChanged(nameof(DisplayMemberName));
            }
        }

        public string DisplayMemberName
        {
            get
            {
                if (Parent == null)
                {
                    return MemberName;
                }

                return _displayService.GetMemberName(Parent.FloorPrefix, MemberName);
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked == value)
                {
                    return;
                }

                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));

                if (_isInternalChanging || Parent == null)
                {
                    return;
                }

                Parent.UpdateCheckedFromChildren();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value)
                {
                    return;
                }

                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public ScheduleSetNodeViewModel(ScheduleSet model)
        {
            _model = model;
        }

        public void SetCheckedFromParent(bool value)
        {
            if (_isChecked == value)
            {
                return;
            }

            _isInternalChanging = true;
            _isChecked = value;
            OnPropertyChanged(nameof(IsChecked));
            _isInternalChanging = false;
        }

        public void RefreshAll()
        {
            OnPropertyChanged(nameof(MemberName));
            OnPropertyChanged(nameof(DisplayMemberName));
            OnPropertyChanged(nameof(IsChecked));
            OnPropertyChanged(nameof(IsSelected));
        }
    }
}