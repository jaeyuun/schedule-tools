using System.Windows.Input;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Girder.Services;

namespace GirderSchedule.App.ViewModels.Girder
{
    public sealed class ScheduleViewModel : ViewModelBase
    {
        private readonly ScheduleSheet _sheet;
        private bool _isWidthOver;
        private bool _isHeightOver;

        public SectionItemViewModel Left { get; private set; }
        public SectionItemViewModel Center { get; private set; }
        public SectionItemViewModel Right { get; private set; }

        public ICommand RedrawCommand { get; private set; }

        public ScheduleSheet Sheet
        {
            get { return _sheet; }
        }

        public string MemberName
        {
            get { return Left.Name; }
            set
            {
                if (Left.Name == value)
                {
                    return;
                }

                Left.Name = value;
                Center.Name = value;
                Right.Name = value;

                OnPropertyChanged(nameof(MemberName));
                RefreshItems();
            }
        }

        public double WidthValue
        {
            get { return Left.WidthValue; }
            set
            {
                Left.WidthValue = value;
                Center.WidthValue = value;
                Right.WidthValue = value;

                OnPropertyChanged(nameof(WidthValue));
                RefreshItems();
            }
        }

        public double HeightValue
        {
            get { return Left.HeightValue; }
            set
            {
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

            Left = new SectionItemViewModel(_sheet.Rows[0].Items[0]);
            Center = new SectionItemViewModel(_sheet.Rows[0].Items[1]);
            Right = new SectionItemViewModel(_sheet.Rows[0].Items[2]);

            RedrawCommand = new RelayCommand(p => RefreshItems());
        }

        public void RefreshItems()
        {
            Left.RefreshAll();
            Center.RefreshAll();
            Right.RefreshAll();

            OnPropertyChanged(nameof(Left));
            OnPropertyChanged(nameof(Center));
            OnPropertyChanged(nameof(Right));
        }
    }
}