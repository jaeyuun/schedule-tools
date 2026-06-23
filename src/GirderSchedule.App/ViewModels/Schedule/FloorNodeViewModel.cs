using GirderSchedule.Domain.Models;
using System.Collections.ObjectModel;

namespace GirderSchedule.App.ViewModels.Schedule
{
    public sealed class FloorNodeViewModel : ViewModelBase
    {
        private readonly ScheduleFloor _model;
        private bool _isChecked = true;
        private bool _isSelected;
        private bool _isChildSelected;
        private bool _isInternalChanging;

        public ScheduleFloor Model
        {
            get { return _model; }
        }

        public ObservableCollection<ScheduleSetNodeViewModel> Sets { get; private set; }

        public string FloorPrefix
        {
            get { return _model.Setting.FloorPrefix; }
            set
            {
                if (_model.Setting.FloorPrefix == value)
                {
                    return;
                }

                _model.Setting.FloorPrefix = value;
                OnPropertyChanged(nameof(FloorPrefix));
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string Name
        {
            get { return _model.Name; }
            set
            {
                if (_model.Name == value)
                {
                    return;
                }

                _model.Name = value;

                if (_model.Setting != null)
                {
                    _model.Setting.FloorName = value;
                }

                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return "(층 이름 없음)";
                }

                return Name;
            }
        }

        public FloorSetting Setting
        {
            get { return _model.Setting; }
        }

        public int MainRebarDiameter
        {
            get { return _model.Setting.MainRebarDiameter; }
            set
            {
                if (_model.Setting.MainRebarDiameter == value)
                {
                    return;
                }

                _model.Setting.MainRebarDiameter = value;
                OnPropertyChanged(nameof(MainRebarDiameter));
                OnPropertyChanged(nameof(MainRebarText));
            }
        }

        public int StirrupDiameter
        {
            get { return _model.Setting.StirrupDiameter; }
            set
            {
                if (_model.Setting.StirrupDiameter == value)
                {
                    return;
                }

                _model.Setting.StirrupDiameter = value;
                OnPropertyChanged(nameof(StirrupDiameter));
                OnPropertyChanged(nameof(StirrupText));
            }
        }

        public int SkinRebarDiameter
        {
            get { return _model.Setting.SkinRebarDiameter; }
            set
            {
                if (_model.Setting.SkinRebarDiameter == value)
                {
                    return;
                }

                _model.Setting.SkinRebarDiameter = value;
                OnPropertyChanged(nameof(SkinRebarDiameter));
                OnPropertyChanged(nameof(SkinRebarText));
            }
        }

        public string MainRebarText
        {
            get { return FormatDiameter(MainRebarDiameter); }
        }

        public string StirrupText
        {
            get { return FormatDiameter(StirrupDiameter); }
        }

        public string SkinRebarText
        {
            get { return FormatDiameter(SkinRebarDiameter); }
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

                if (_isInternalChanging)
                {
                    return;
                }

                _isInternalChanging = true;

                for (var i = 0; i < Sets.Count; i++)
                {
                    Sets[i].SetCheckedFromParent(value);
                }

                _isInternalChanging = false;
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

        public bool IsChildSelected
        {
            get { return _isChildSelected; }
            set
            {
                if (_isChildSelected == value)
                {
                    return;
                }

                _isChildSelected = value;
                OnPropertyChanged(nameof(IsChildSelected));
            }
        }

        public FloorNodeViewModel(ScheduleFloor model)
        {
            _model = model;

            if (_model.Setting == null)
            {
                _model.Setting = new FloorSetting();
            }

            if (string.IsNullOrWhiteSpace(_model.Setting.FloorName))
            {
                _model.Setting.FloorName = _model.Name;
            }

            if (_model.Sets == null)
            {
                _model.Sets = new System.Collections.Generic.List<ScheduleSet>();
            }

            Sets = new ObservableCollection<ScheduleSetNodeViewModel>();

            for (var i = 0; i < _model.Sets.Count; i++)
            {
                var node = new ScheduleSetNodeViewModel(_model.Sets[i]);
                node.Parent = this;
                Sets.Add(node);
            }
        }

        public ScheduleSetNodeViewModel AddSet(ScheduleSet set)
        {
            var node = new ScheduleSetNodeViewModel(set);
            node.Parent = this;

            Sets.Add(node);
            Model.Sets.Add(set);

            UpdateCheckedFromChildren();

            return node;
        }

        public void UpdateCheckedFromChildren()
        {
            if (_isInternalChanging)
            {
                return;
            }

            var hasCheckedChild = false;

            for (var i = 0; i < Sets.Count; i++)
            {
                if (Sets[i].IsChecked)
                {
                    hasCheckedChild = true;
                    break;
                }
            }

            if (_isChecked == hasCheckedChild)
            {
                return;
            }

            _isInternalChanging = true;
            _isChecked = hasCheckedChild;
            OnPropertyChanged(nameof(IsChecked));
            _isInternalChanging = false;
        }

        public void RemoveSet(ScheduleSetNodeViewModel node)
        {
            if (node == null)
            {
                return;
            }

            _model.Sets.Remove(node.Model);
            Sets.Remove(node);

            UpdateCheckedFromChildren();
        }

        public void RefreshAll()
        {
            OnPropertyChanged(nameof(FloorPrefix));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(MainRebarDiameter));
            OnPropertyChanged(nameof(StirrupDiameter));
            OnPropertyChanged(nameof(SkinRebarDiameter));
            OnPropertyChanged(nameof(MainRebarText));
            OnPropertyChanged(nameof(StirrupText));
            OnPropertyChanged(nameof(SkinRebarText));
            OnPropertyChanged(nameof(IsChecked));
            OnPropertyChanged(nameof(IsSelected));
            OnPropertyChanged(nameof(IsChildSelected));

            for (var i = 0; i < Sets.Count; i++)
            {
                Sets[i].RefreshAll();
            }
        }

        private string FormatDiameter(int diameter)
        {
            if (diameter <= 0)
            {
                return "-";
            }

            return "HD" + diameter;
        }
    }
}