using GirderSchedule.App.Commands;
using GirderSchedule.Domain.Models;
using System;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels.Schedule
{
    public sealed class ScheduleViewModel : ViewModelBase
    {
        private ScheduleFloor _floor;
        private ScheduleSet _set;
        private bool _isWidthOver;
        private bool _isHeightOver;

        public SectionItemViewModel Left { get; private set; }
        public SectionItemViewModel Center { get; private set; }
        public SectionItemViewModel Right { get; private set; }

        public ICommand RedrawCommand { get; private set; }

        public event EventHandler CurrentSetChanged;

        public ScheduleSet CurrentSet
        {
            get { return _set; }
        }

        public string FloorPrefix
        {
            get
            {
                if (_floor == null || _floor.Setting == null)
                {
                    return string.Empty;
                }

                return _floor.Setting.Prefix;
            }
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

                _set.MemberName = value ?? string.Empty;

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
                if (Left == null || Center == null || Right == null || Left.WidthValue == value)
                {
                    return;
                }

                Left.WidthValue = value;
                Center.WidthValue = value;
                Right.WidthValue = value;

                OnPropertyChanged(nameof(WidthValue));
                RefreshItems();
                RaiseCurrentSetChanged();
            }
        }

        public double HeightValue
        {
            get { return Left == null ? 0.0 : Left.HeightValue; }
            set
            {
                if (Left == null || Center == null || Right == null || Left.HeightValue == value)
                {
                    return;
                }

                Left.HeightValue = value;
                Center.HeightValue = value;
                Right.HeightValue = value;

                OnPropertyChanged(nameof(HeightValue));
                RefreshItems();
                RaiseCurrentSetChanged();
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
                RaiseCurrentSetChanged();
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
                RaiseCurrentSetChanged();
            }
        }

        public ScheduleViewModel()
        {
            RedrawCommand = new RelayCommand(p => RefreshItems());
        }

        public void LoadSet(ScheduleFloor floor, ScheduleSet set)
        {
            if (floor == null || set == null)
            {
                return;
            }

            _floor = floor;
            _set = set;

            Left = new SectionItemViewModel(_set.Left);
            Center = new SectionItemViewModel(_set.Center);
            Right = new SectionItemViewModel(_set.Right);

            Left.Changed += SectionItem_Changed;
            Center.Changed += SectionItem_Changed;
            Right.Changed += SectionItem_Changed;

            ApplySectionEnabledPositions();

            OnPropertyChanged(nameof(CurrentSet));
            OnPropertyChanged(nameof(FloorPrefix));
            OnPropertyChanged(nameof(MemberName));
            OnPropertyChanged(nameof(WidthValue));
            OnPropertyChanged(nameof(HeightValue));
            OnPropertyChanged(nameof(Left));
            OnPropertyChanged(nameof(Center));
            OnPropertyChanged(nameof(Right));

            RefreshItems();
            RaiseCurrentSetChanged();
        }

        private void RefreshItems()
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