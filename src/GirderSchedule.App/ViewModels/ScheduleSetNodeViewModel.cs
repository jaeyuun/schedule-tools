using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.ViewModels
{
    public sealed class ScheduleSetNodeViewModel : ViewModelBase
    {
        private readonly ScheduleSet _model;
        private bool _isChecked = true;
        private bool _isSelected;

        public ScheduleSet Model
        {
            get { return _model; }
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
                if (_isChecked == value)
                {
                    return;
                }

                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
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

        public void RefreshAll()
        {
            OnPropertyChanged(nameof(MemberName));
            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(IsChecked));
            OnPropertyChanged(nameof(IsSelected));
        }
    }
}