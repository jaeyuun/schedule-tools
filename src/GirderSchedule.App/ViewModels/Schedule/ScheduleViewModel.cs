using GirderSchedule.App.Commands;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Services;
using System;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels.Schedule
{
    public sealed class ScheduleViewModel : ViewModelBase
    {
        private ScheduleSheet _sheet;
        private ScheduleSet _set;
        private bool _isWidthOver;
        private bool _isHeightOver;

        public SectionItemViewModel Left { get; private set; }
        public SectionItemViewModel Center { get; private set; }
        public SectionItemViewModel Right { get; private set; }

        public ICommand RedrawCommand { get; private set; }

        public event EventHandler CurrentSetChanged;

        public ScheduleSheet Sheet
        {
            get { return _sheet; }
        }

        public ScheduleSet CurrentSet
        {
            get { return _set; }
        }

        public string MemberName
        {
            get
            {
                if (_set == null)
                {
                    return string.Empty;
                }

                return _set.MemberName;
            }
            set
            {
                if (_set == null || _set.MemberName == value)
                {
                    return;
                }

                _set.MemberName = value;

                if (Left != null)
                {
                    Left.Name = value;
                }

                if (Center != null)
                {
                    Center.Name = value;
                }

                if (Right != null)
                {
                    Right.Name = value;
                }

                OnPropertyChanged(nameof(MemberName));
                RefreshItems();
                RaiseCurrentSetChanged();
            }
        }

        public double WidthValue
        {
            get { return Left == null ? 0.0 : Left.WidthValue; }
            set
            {
                if (Left == null || Left.WidthValue == value)
                {
                    return;
                }

                Left.WidthValue = value;
                Center.WidthValue = value;
                Right.WidthValue = value;

                OnPropertyChanged(nameof(WidthValue));
                RefreshItems();
            }
        }

        public double HeightValue
        {
            get { return Left == null ? 0.0 : Left.HeightValue; }
            set
            {
                if (Left == null || Left.HeightValue == value)
                {
                    return;
                }

                Left.HeightValue = value;
                Center.HeightValue = value;
                Right.HeightValue = value;

                OnPropertyChanged(nameof(HeightValue));
                RefreshItems();
            }
        }

        public bool IsWidthOver
        {
            get { return _isWidthOver; }
            set
            {
                if (_isWidthOver == value)
                {
                    return;
                }

                _isWidthOver = value;
                OnPropertyChanged(nameof(IsWidthOver));
                RefreshItems();
            }
        }

        public bool IsHeightOver
        {
            get { return _isHeightOver; }
            set
            {
                if (_isHeightOver == value)
                {
                    return;
                }

                _isHeightOver = value;
                OnPropertyChanged(nameof(IsHeightOver));
                RefreshItems();
            }
        }

        public ScheduleViewModel()
        {
            var service = new ScheduleBuildService();
            _sheet = service.CreateSample();

            RedrawCommand = new RelayCommand(p => RefreshItems());

            LoadSet(_sheet.Rows[0].Sets[0]);
        }

        public void LoadSet(ScheduleSet set)
        {
            if (set == null)
            {
                return;
            }

            _set = set;

            Left = new SectionItemViewModel(_set.Left);
            Center = new SectionItemViewModel(_set.Center);
            Right = new SectionItemViewModel(_set.Right);

            Left.Changed += SectionItem_Changed;
            Center.Changed += SectionItem_Changed;
            Right.Changed += SectionItem_Changed;

            ApplySectionEnabledPositions();

            EnsureSheetForCurrentSet();

            OnPropertyChanged(nameof(CurrentSet));
            OnPropertyChanged(nameof(Sheet));
            OnPropertyChanged(nameof(MemberName));
            OnPropertyChanged(nameof(WidthValue));
            OnPropertyChanged(nameof(HeightValue));
            OnPropertyChanged(nameof(Left));
            OnPropertyChanged(nameof(Center));
            OnPropertyChanged(nameof(Right));

            RefreshItems();
        }

        public void ApplyFloorSetting(FloorSetting setting)
        {
            if (setting == null || Left == null || Center == null || Right == null)
            {
                return;
            }

            ApplyFloorSettingToItem(Left, setting);
            ApplyFloorSettingToItem(Center, setting);
            ApplyFloorSettingToItem(Right, setting);

            RefreshItems();
        }

        public void RefreshItems()
        {
            if (Left == null || Center == null || Right == null)
            {
                return;
            }

            Left.RefreshAll();
            Center.RefreshAll();
            Right.RefreshAll();

            OnPropertyChanged(nameof(Left));
            OnPropertyChanged(nameof(Center));
            OnPropertyChanged(nameof(Right));
        }

        private void RaiseCurrentSetChanged()
        {
            var handler = CurrentSetChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void SectionItem_Changed(object sender, EventArgs e)
        {
            ApplySectionEnabledPositions();
            RefreshItems();
            RaiseCurrentSetChanged();
        }

        private void ApplyFloorSettingToItem(SectionItemViewModel item, FloorSetting setting)
        {
            if (setting.MainRebarDiameter > 0)
            {
                item.TopDiameter = setting.MainRebarDiameter;
                item.BottomDiameter = setting.MainRebarDiameter;
            }

            if (setting.StirrupDiameter > 0)
            {
                item.StirrupDiameter = setting.StirrupDiameter;
            }

            if (setting.SkinRebarDiameter > 0)
            {
                item.SkinRebarDiameter = setting.SkinRebarDiameter;
            }
        }

        private void EnsureSheetForCurrentSet()
        {
            var service = new ScheduleBuildService();
            _sheet = service.CreateSample();

            if (_sheet.Rows.Count == 0 || _sheet.Rows[0].Sets.Count == 0)
            {
                return;
            }

            _sheet.Rows[0].Sets[0] = _set;
        }

        private void ApplySectionEnabledPositions()
        {
            if (Left == null || Center == null || Right == null)
            {
                return;
            }

            var isSelectedLeft = Left.IsSectionEnabled;
            var isSelectedRight = Right.IsSectionEnabled;

            if (isSelectedLeft && isSelectedRight)
            {
                Left.Position = "END";
                Center.Position = "CEN";
                Right.Position = "END";
            }
            else if (isSelectedLeft && !isSelectedRight)
            {
                Left.Position = "BOTH";
                Center.Position = "CEN";
                Right.Position = string.Empty;
            }
            else if (!isSelectedLeft && isSelectedRight)
            {
                Left.Position = string.Empty;
                Center.Position = "CEN";
                Right.Position = "BOTH";
            }
            else
            {
                Left.Position = string.Empty;
                Center.Position = "ALL";
                Right.Position = string.Empty;
            }
        }
    }
}