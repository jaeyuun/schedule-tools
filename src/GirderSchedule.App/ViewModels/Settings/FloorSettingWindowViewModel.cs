using GirderSchedule.App.Commands;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Xml.XPath;

namespace GirderSchedule.App.ViewModels.Settings
{
    public sealed class FloorSettingWindowViewModel : ViewModelBase
    {
        private readonly ScheduleSetFactory _setFactory = new ScheduleSetFactory();
        private FloorNodeViewModel _selectedFloor;

        private string _inputFloorNumber = string.Empty;
        private string _inputFloorName = string.Empty;
        private string _inputMainRebarDiameter = string.Empty;
        private string _inputStirrupDiameter = string.Empty;
        private string _inputSkinRebarDiameter = string.Empty;

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

            AddCommand = new RelayCommand(p => Add());
            UpdateCommand = new RelayCommand(p => Update(), p => SelectedFloor != null);
            RemoveCommand = new RelayCommand(p => Remove(), p => SelectedFloor != null);
        }

        private void Add()
        {
            if (string.IsNullOrWhiteSpace(InputFloorName))
            {
                MessageBox.Show("층 이름을 입력해 주세요.", "층 추가", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var floor = new ScheduleFloor();
            floor.Setting.Prefix = InputFloorPrefix.Trim();
            floor.Setting.Name = InputFloorName.Trim();
            floor.Setting.MainRebarDiameter = ParseDouble(InputMainRebarDiameter);
            floor.Setting.StirrupDiameter = ParseDouble(InputStirrupDiameter);
            floor.Setting.SkinRebarDiameter = ParseDouble(InputSkinRebarDiameter);

            var set = _setFactory.Create("G1", floor.Setting);
            floor.Sets.Add(set);

            var node = new FloorNodeViewModel(floor);
            Floors.Add(node);
            SelectedFloor = node;
        }

        private void Update()
        {
            if (SelectedFloor == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(InputFloorName))
            {
                MessageBox.Show("층 이름을 입력해 주세요.", "층 수정", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SelectedFloor.FloorPrefix = InputFloorPrefix.Trim();
            SelectedFloor.FloorName = InputFloorName.Trim();
            SelectedFloor.MainRebarDiameter = ParseDouble(InputMainRebarDiameter);
            SelectedFloor.StirrupDiameter = ParseDouble(InputStirrupDiameter);
            SelectedFloor.SkinRebarDiameter = ParseDouble(InputSkinRebarDiameter);
            SelectedFloor.RefreshAll();

            RaiseFloorSettingChanged();
        }

        private void Remove()
        {
            if (SelectedFloor == null)
            {
                return;
            }

            var result = MessageBox.Show(
                "선택한 층과 하위 부재가 모두 삭제됩니다.\r\n삭제하시겠습니까?",
                "층 삭제",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var index = Floors.IndexOf(SelectedFloor);
            Floors.Remove(SelectedFloor);

            if (Floors.Count == 0)
            {
                SelectedFloor = null;
                ClearInput();
                return;
            }

            if (index >= Floors.Count)
            {
                index = Floors.Count - 1;
            }

            SelectedFloor = Floors[index];
        }

        private void LoadSelectedFloor()
        {
            if (SelectedFloor == null)
            {
                return;
            }

            InputFloorPrefix = SelectedFloor.FloorPrefix.ToString();
            InputFloorName = SelectedFloor.FloorName;
            InputMainRebarDiameter = ToInputText(SelectedFloor.MainRebarDiameter);
            InputStirrupDiameter = ToInputText(SelectedFloor.StirrupDiameter);
            InputSkinRebarDiameter = ToInputText(SelectedFloor.SkinRebarDiameter);
        }

        private void ClearInput()
        {
            InputFloorPrefix = string.Empty;
            InputFloorName = string.Empty;
            InputMainRebarDiameter = string.Empty;
            InputStirrupDiameter = string.Empty;
            InputSkinRebarDiameter = string.Empty;
        }

        private int ParseInt(string value)
        {
            int result;

            if (int.TryParse(value, out result))
            {
                return result;
            }

            return 0;
        }

        private double ParseDouble(string value)
        {
            double result;

            if (double.TryParse(value, out result))
            {
                return result;
            }

            return 0.0;
        }

        private string ToInputText(int value)
        {
            if (value <= 0)
            {
                return string.Empty;
            }

            return value.ToString();
        }

        private string ToInputText(double value)
        {
            if (value <= 0.0)
            {
                return string.Empty;
            }

            return value.ToString();
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