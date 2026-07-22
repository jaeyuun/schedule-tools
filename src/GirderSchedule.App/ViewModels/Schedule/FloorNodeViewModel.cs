using GirderSchedule.App.Services.Schedule;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Settings;
using ScheduleTools.Wpf.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace GirderSchedule.App.ViewModels.Schedule
{
    public sealed class FloorNodeViewModel : ViewModelBase
    {
        private readonly ScheduleTextFormatService _textService = new ScheduleTextFormatService();

        private readonly ScheduleFloor _model;
        private bool _isChecked = true;
        private bool _isSelected;
        private bool _isChildSelected;
        private bool _isInternalChanging;

        public event EventHandler? Changed;

        public ScheduleFloor Model
        {
            get { return _model; }
        }

        public ObservableCollection<ScheduleSetNodeViewModel> Sets { get; private set; } = null!;

        public string FloorPrefix
        {
            get { return _model.Setting.Prefix; }
            set
            {
                if (_model.Setting.Prefix == value)
                {
                    return;
                }

                _model.Setting.Prefix = value ?? string.Empty;
                OnPropertyChanged(nameof(FloorPrefix));
                RefreshSets();
                RaiseChanged();
            }
        }

        public string FloorName
        {
            get { return _model.Setting.Name; }
            set
            {
                if (_model.Setting.Name == value)
                {
                    return;
                }

                _model.Setting.Name = value ?? string.Empty;
                OnPropertyChanged(nameof(FloorName));
                RaiseChanged();
            }
        }

        public FloorSetting Setting
        {
            get { return _model.Setting; }
        }

        public double MainRebarDiameter
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
                OnPropertyChanged(nameof(MainRebarDiameterText));
                OnPropertyChanged(nameof(MainRebarText));
                RefreshSets();
                RaiseChanged();
            }
        }

        public double StirrupDiameter
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
                OnPropertyChanged(nameof(StirrupDiameterText));
                OnPropertyChanged(nameof(StirrupText));
                RefreshSets();
                RaiseChanged();
            }
        }

        public double SkinRebarDiameter
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
                OnPropertyChanged(nameof(SkinRebarDiameterText));
                OnPropertyChanged(nameof(SkinRebarText));
                RefreshSets();
                RaiseChanged();
            }
        }

        public string MainRebarDiameterText
        {
            get { return _textService.FormatDiameterInput(MainRebarDiameter); }
            set { MainRebarDiameter = _textService.ParseDiameter(value); }
        }

        public string StirrupDiameterText
        {
            get { return _textService.FormatDiameterInput(StirrupDiameter); }
            set { StirrupDiameter = _textService.ParseDiameter(value); }
        }

        public string SkinRebarDiameterText
        {
            get { return _textService.FormatDiameterInput(SkinRebarDiameter); }
            set { SkinRebarDiameter = _textService.ParseDiameter(value); }
        }

        public string MainRebarText
        {
            get { return _textService.FormatFloorSettingDiameter(MainRebarDiameter); }
        }

        public string StirrupText
        {
            get { return _textService.FormatFloorSettingDiameter(StirrupDiameter); }
        }

        public string SkinRebarText
        {
            get { return _textService.FormatFloorSettingDiameter(SkinRebarDiameter); }
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

                SetChildrenChecked(value);
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
            EnsureModel();
            InitializeSets();
        }

        public ScheduleSetNodeViewModel? AddSet(ScheduleSet? set)
        {
            if (set == null)
            {
                return null;
            }

            var node = CreateSetNode(set);

            Sets.Add(node);
            Model.Sets.Add(set);

            UpdateCheckedFromChildren();

            return node;
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

        public void UpdateCheckedFromChildren()
        {
            if (_isInternalChanging)
            {
                return;
            }

            var hasCheckedChild = HasCheckedChild();

            if (_isChecked == hasCheckedChild)
            {
                return;
            }

            _isInternalChanging = true;
            _isChecked = hasCheckedChild;
            OnPropertyChanged(nameof(IsChecked));
            _isInternalChanging = false;
        }

        public void RefreshAll()
        {
            OnPropertyChanged(nameof(FloorPrefix));
            OnPropertyChanged(nameof(FloorName));

            OnPropertyChanged(nameof(MainRebarDiameter));
            OnPropertyChanged(nameof(MainRebarDiameterText));
            OnPropertyChanged(nameof(MainRebarText));

            OnPropertyChanged(nameof(StirrupDiameter));
            OnPropertyChanged(nameof(StirrupDiameterText));
            OnPropertyChanged(nameof(StirrupText));

            OnPropertyChanged(nameof(SkinRebarDiameter));
            OnPropertyChanged(nameof(SkinRebarDiameterText));
            OnPropertyChanged(nameof(SkinRebarText));

            OnPropertyChanged(nameof(IsChecked));
            OnPropertyChanged(nameof(IsSelected));
            OnPropertyChanged(nameof(IsChildSelected));

            RefreshSets();
        }

        private void EnsureModel()
        {
            if (_model.Setting == null)
            {
                _model.Setting = new FloorSetting();
            }

            if (_model.Sets == null)
            {
                _model.Sets = new System.Collections.Generic.List<ScheduleSet>();
            }
        }

        private void InitializeSets()
        {
            Sets = new ObservableCollection<ScheduleSetNodeViewModel>();

            for (var i = 0; i < _model.Sets.Count; i++)
            {
                Sets.Add(CreateSetNode(_model.Sets[i]));
            }
        }

        private ScheduleSetNodeViewModel CreateSetNode(ScheduleSet set)
        {
            var node = new ScheduleSetNodeViewModel(set);
            node.Parent = this;

            return node;
        }

        private void SetChildrenChecked(bool isChecked)
        {
            _isInternalChanging = true;

            for (var i = 0; i < Sets.Count; i++)
            {
                Sets[i].SetCheckedFromParent(isChecked);
            }

            _isInternalChanging = false;
        }

        private bool HasCheckedChild()
        {
            for (var i = 0; i < Sets.Count; i++)
            {
                if (Sets[i].IsChecked)
                {
                    return true;
                }
            }

            return false;
        }

        private void RefreshSets()
        {
            for (var i = 0; i < Sets.Count; i++)
            {
                Sets[i].RefreshAll();
            }
        }

        private void RaiseChanged()
        {
            var handler = Changed;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
