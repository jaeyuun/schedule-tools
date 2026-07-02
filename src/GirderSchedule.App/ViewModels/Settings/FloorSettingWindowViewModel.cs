using GirderSchedule.App.Commands;
using GirderSchedule.App.Services;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels.Settings
{
    public sealed class FloorSettingWindowViewModel : ViewModelBase
    {
        private readonly ScheduleSetFactory _setFactory = new ScheduleSetFactory();
        private readonly ScheduleTextFormatService _textService = new ScheduleTextFormatService();
        private FloorNodeViewModel _selectedFloor;

        private string _inputFloorNumber = string.Empty;
        private string _inputFloorName = string.Empty;
        private string _inputMainRebarDiameter = string.Empty;
        private string _inputStirrupDiameter = string.Empty;
        private string _inputSkinRebarDiameter = string.Empty;
        private bool _isUpdatingSelectedFloor;

        public ObservableCollection<FloorNodeViewModel> Floors { get; private set; }
        public event EventHandler FloorSettingChanged;

        public FloorNodeViewModel SelectedFloor
        {
            get { return _selectedFloor; }
            set
            {
                if (_selectedFloor == value)
                {
                    return;
                }

                _selectedFloor = value;
                OnPropertyChanged(nameof(SelectedFloor));
                LoadSelectedFloor();
            }
        }

        public string InputFloorPrefix
        {
            get { return _inputFloorNumber; }
            set
            {
                if (_inputFloorNumber == value)
                {
                    return;
                }

                _inputFloorNumber = value;
                OnPropertyChanged(nameof(InputFloorPrefix));
            }
        }

        public string InputFloorName
        {
            get { return _inputFloorName; }
            set
            {
                if (_inputFloorName == value)
                {
                    return;
                }

                _inputFloorName = value;
                OnPropertyChanged(nameof(InputFloorName));
            }
        }

        public string InputMainRebarDiameter
        {
            get { return _inputMainRebarDiameter; }
            set
            {
                if (_inputMainRebarDiameter == value)
                {
                    return;
                }

                _inputMainRebarDiameter = value;
                OnPropertyChanged(nameof(InputMainRebarDiameter));
            }
        }

        public string InputStirrupDiameter
        {
            get { return _inputStirrupDiameter; }
            set
            {
                if (_inputStirrupDiameter == value)
                {
                    return;
                }

                _inputStirrupDiameter = value;
                OnPropertyChanged(nameof(InputStirrupDiameter));
            }
        }

        public string InputSkinRebarDiameter
        {
            get { return _inputSkinRebarDiameter; }
            set
            {
                if (_inputSkinRebarDiameter == value)
                {
                    return;
                }

                _inputSkinRebarDiameter = value;
                OnPropertyChanged(nameof(InputSkinRebarDiameter));
            }
        }

        public ICommand AddCommand { get; private set; }
        public ICommand UpdateCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }

        public FloorSettingWindowViewModel(ObservableCollection<FloorNodeViewModel> floors)
        {
            Floors = floors;
            SubscribeFloors();

            AddCommand = new RelayCommand(p => Add());
            UpdateCommand = new RelayCommand(p => Update(), p => SelectedFloor != null);
            RemoveCommand = new RelayCommand(p => Remove(), p => SelectedFloor != null);
        }

        private void Add()
        {
            if (string.IsNullOrWhiteSpace(InputFloorName))
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_FloorSetting,
                    Properties.Resources.Content_InputFloorName);
                return;
            }

            var floor = new ScheduleFloor();
            floor.Setting.Prefix = InputFloorPrefix.Trim();
            floor.Setting.Name = InputFloorName.Trim();
            floor.Setting.MainRebarDiameter = _textService.ParseDiameter(InputMainRebarDiameter);
            floor.Setting.StirrupDiameter = _textService.ParseDiameter(InputStirrupDiameter);
            floor.Setting.SkinRebarDiameter = _textService.ParseDiameter(InputSkinRebarDiameter);

            var set = _setFactory.Create("G1", floor.Setting);
            floor.Sets.Add(set);

            var node = new FloorNodeViewModel(floor);
            SubscribeFloor(node);

            Floors.Add(node);
            SelectedFloor = node;

            RaiseFloorSettingChanged();
        }

        private void Update()
        {
            if (SelectedFloor == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(InputFloorName))
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_FloorSetting,
                    Properties.Resources.Content_InputFloorName);
                return;
            }

            _isUpdatingSelectedFloor = true;

            try
            {
                SelectedFloor.FloorPrefix = InputFloorPrefix.Trim();
                SelectedFloor.FloorName = InputFloorName.Trim();
                SelectedFloor.MainRebarDiameter = _textService.ParseDiameter(InputMainRebarDiameter);
                SelectedFloor.StirrupDiameter = _textService.ParseDiameter(InputStirrupDiameter);
                SelectedFloor.SkinRebarDiameter = _textService.ParseDiameter(InputSkinRebarDiameter);
                SelectedFloor.RefreshAll();
            }
            finally
            {
                _isUpdatingSelectedFloor = false;
            }

            RaiseFloorSettingChanged();
        }

        private void Remove()
        {
            if (SelectedFloor == null)
            {
                return;
            }

            var result = AppDialogService.ShowConfirm(
                Properties.Resources.Title_DeleteFloor,
                Properties.Resources.Content_DeleteSelectedFloorWithMembersConfirm);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var removedFloor = SelectedFloor;
            var index = Floors.IndexOf(removedFloor);

            UnsubscribeFloor(removedFloor);
            Floors.Remove(removedFloor);

            if (Floors.Count == 0)
            {
                SelectedFloor = null;
                ClearInput();
                RaiseFloorSettingChanged();
                return;
            }

            if (index >= Floors.Count)
            {
                index = Floors.Count - 1;
            }

            SelectedFloor = Floors[index];
            RaiseFloorSettingChanged();
        }

        private void LoadSelectedFloor()
        {
            if (SelectedFloor == null)
            {
                return;
            }

            InputFloorPrefix = SelectedFloor.FloorPrefix;
            InputFloorName = SelectedFloor.FloorName;
            InputMainRebarDiameter = _textService.FormatDiameterInput(SelectedFloor.MainRebarDiameter);
            InputStirrupDiameter = _textService.FormatDiameterInput(SelectedFloor.StirrupDiameter);
            InputSkinRebarDiameter = _textService.FormatDiameterInput(SelectedFloor.SkinRebarDiameter);
        }

        private void ClearInput()
        {
            InputFloorPrefix = string.Empty;
            InputFloorName = string.Empty;
            InputMainRebarDiameter = string.Empty;
            InputStirrupDiameter = string.Empty;
            InputSkinRebarDiameter = string.Empty;
        }

        private void SubscribeFloors()
        {
            if (Floors == null)
            {
                return;
            }

            for (var i = 0; i < Floors.Count; i++)
            {
                SubscribeFloor(Floors[i]);
            }
        }

        private void SubscribeFloor(FloorNodeViewModel floor)
        {
            if (floor == null)
            {
                return;
            }

            floor.Changed -= Floor_Changed;
            floor.Changed += Floor_Changed;
        }

        private void UnsubscribeFloor(FloorNodeViewModel floor)
        {
            if (floor == null)
            {
                return;
            }

            floor.Changed -= Floor_Changed;
        }

        private void Floor_Changed(object sender, EventArgs e)
        {
            if (_isUpdatingSelectedFloor)
            {
                return;
            }

            var floor = sender as FloorNodeViewModel;

            if (floor != null && ReferenceEquals(floor, SelectedFloor))
            {
                LoadSelectedFloor();
            }

            RaiseFloorSettingChanged();
        }

        private void RaiseFloorSettingChanged()
        {
            var handler = FloorSettingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}