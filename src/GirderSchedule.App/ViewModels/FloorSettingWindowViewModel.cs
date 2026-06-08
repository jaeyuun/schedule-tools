using System.Collections.ObjectModel;
using System.Windows.Input;
using GirderSchedule.App.Commands;

namespace GirderSchedule.App.ViewModels
{
    public sealed class FloorSettingWindowViewModel : ViewModelBase
    {
        private FloorNodeViewModel _selectedFloor;
        private string _inputFloorName = string.Empty;
        private string _inputMainRebarDiameter = string.Empty;
        private string _inputStirrupDiameter = string.Empty;
        private string _inputSkinRebarDiameter = string.Empty;

        public ObservableCollection<FloorNodeViewModel> Floors { get; private set; }

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

        public ICommand UpdateCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        public FloorSettingWindowViewModel(ObservableCollection<FloorNodeViewModel> floors)
        {
            Floors = floors;
            UpdateCommand = new RelayCommand(p => Update(), p => SelectedFloor != null);
            ClearCommand = new RelayCommand(p => ClearInput());
        }

        private void Update()
        {
            if (SelectedFloor == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(InputFloorName))
            {
                SelectedFloor.Name = InputFloorName.Trim();
            }

            SelectedFloor.MainRebarDiameter = ParseInt(InputMainRebarDiameter);
            SelectedFloor.StirrupDiameter = ParseInt(InputStirrupDiameter);
            SelectedFloor.SkinRebarDiameter = ParseInt(InputSkinRebarDiameter);
            SelectedFloor.RefreshAll();
        }

        private void LoadSelectedFloor()
        {
            if (SelectedFloor == null)
            {
                return;
            }

            InputFloorName = SelectedFloor.Name;
            InputMainRebarDiameter = ToInputText(SelectedFloor.MainRebarDiameter);
            InputStirrupDiameter = ToInputText(SelectedFloor.StirrupDiameter);
            InputSkinRebarDiameter = ToInputText(SelectedFloor.SkinRebarDiameter);
        }

        private void ClearInput()
        {
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

        private string ToInputText(int value)
        {
            if (value <= 0)
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }
}