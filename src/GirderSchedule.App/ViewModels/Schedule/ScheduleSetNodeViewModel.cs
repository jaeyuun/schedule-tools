using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.ViewModels.Schedule
{
    public sealed class ScheduleSetNodeViewModel : ViewModelBase
    {
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
            set { _parent = value; }
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
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MemberName))
                {
                    return "(부재명 없음)";
                }

                return MemberName;
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                SetChecked(value, true);
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
            SetChecked(value, false);
        }

        private void SetChecked(bool value, bool updateParent)
        {
            if (_isChecked == value)
            {
                return;
            }

            _isChecked = value;
            OnPropertyChanged(nameof(IsChecked));

            if (!updateParent || _isInternalChanging || Parent == null)
            {
                return;
            }

            _isInternalChanging = true;
            Parent.UpdateCheckedFromChildren();
            _isInternalChanging = false;
        }

        public void RefreshAll()
        {
            OnPropertyChanged(nameof(MemberName));
            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(IsChecked));
            OnPropertyChanged(nameof(IsSelected));
        }
    }
}