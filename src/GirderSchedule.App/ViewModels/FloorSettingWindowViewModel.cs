using GirderSchedule.App.Commands;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

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
            floor.Name = InputFloorName.Trim();
            floor.Setting.FloorName = floor.Name;
            floor.Setting.MainRebarDiameter = ParseInt(InputMainRebarDiameter);
            floor.Setting.StirrupDiameter = ParseInt(InputStirrupDiameter);
            floor.Setting.SkinRebarDiameter = ParseInt(InputSkinRebarDiameter);

            var set = CreateScheduleSet("G1", floor.Setting);
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

            SelectedFloor.Name = InputFloorName.Trim();
            SelectedFloor.MainRebarDiameter = ParseInt(InputMainRebarDiameter);
            SelectedFloor.StirrupDiameter = ParseInt(InputStirrupDiameter);
            SelectedFloor.SkinRebarDiameter = ParseInt(InputSkinRebarDiameter);
            SelectedFloor.RefreshAll();
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

        private ScheduleSet CreateScheduleSet(string memberName, FloorSetting setting)
        {
            var service = new ScheduleBuildService();
            var sheet = service.CreateSample();
            var set = sheet.Rows[0].Sets[0];

            set.MemberName = memberName;

            set.Left.Name = memberName;
            set.Center.Name = memberName;
            set.Right.Name = memberName;

            ApplySetting(set.Left, setting);
            ApplySetting(set.Center, setting);
            ApplySetting(set.Right, setting);

            return set;
        }

        private void ApplySetting(ScheduleItem item, FloorSetting setting)
        {
            if (setting == null)
            {
                return;
            }

            if (setting.MainRebarDiameter > 0)
            {
                item.TopRebar.Diameter = setting.MainRebarDiameter;
                item.BottomRebar.Diameter = setting.MainRebarDiameter;
            }

            if (setting.StirrupDiameter > 0)
            {
                item.Stirrup.Diameter = setting.StirrupDiameter;
            }

            if (setting.SkinRebarDiameter > 0)
            {
                var spacing = string.Empty;

                if (!string.IsNullOrWhiteSpace(item.SkinRebarText) && item.SkinRebarText.Contains("@"))
                {
                    spacing = item.SkinRebarText.Substring(item.SkinRebarText.IndexOf("@") + 1).Trim();
                }

                item.SkinRebarText = string.IsNullOrWhiteSpace(spacing)
                    ? "HD" + setting.SkinRebarDiameter
                    : "HD" + setting.SkinRebarDiameter + "@" + spacing;
            }
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