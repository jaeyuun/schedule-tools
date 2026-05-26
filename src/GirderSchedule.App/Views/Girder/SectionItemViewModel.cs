using GirderSchedule.Domain.Girder.Models;

namespace GirderSchedule.App.ViewModels.Girder
{
    public sealed class SectionItemViewModel : ViewModelBase
    {
        private readonly ScheduleItem _model;
        private bool _isSectionEnabled = true;

        public ScheduleItem Model
        {
            get { return _model; }
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
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Position
        {
            get { return _model.Position; }
            set
            {
                if (_model.Position == value)
                {
                    return;
                }

                _model.Position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        public double WidthValue
        {
            get { return _model.Section.Width; }
            set
            {
                if (_model.Section.Width == value)
                {
                    return;
                }

                _model.Section.Width = value;
                OnPropertyChanged(nameof(WidthValue));
            }
        }

        public double HeightValue
        {
            get { return _model.Section.Height; }
            set
            {
                if (_model.Section.Height == value)
                {
                    return;
                }

                _model.Section.Height = value;
                OnPropertyChanged(nameof(HeightValue));
            }
        }

        public int TopCount1
        {
            get { return _model.TopRebar.FirstLayer.Count; }
            set
            {
                if (_model.TopRebar.FirstLayer.Count == value)
                {
                    return;
                }

                _model.TopRebar.FirstLayer.Count = value;
                OnPropertyChanged(nameof(TopCount1));
                OnPropertyChanged(nameof(TopTotalCount));
            }
        }

        public int TopDiameter
        {
            get { return _model.TopRebar.Diameter; }
            set
            {
                if (_model.TopRebar.Diameter == value)
                {
                    return;
                }

                _model.TopRebar.Diameter = value;
                OnPropertyChanged(nameof(TopDiameter));
                OnPropertyChanged(nameof(TopText1));
                OnPropertyChanged(nameof(TopText2));
            }
        }

        public int TopCount2
        {
            get { return _model.TopRebar.SecondLayer.Count; }
            set
            {
                if (_model.TopRebar.SecondLayer.Count == value)
                {
                    return;
                }

                _model.TopRebar.SecondLayer.Count = value;
                OnPropertyChanged(nameof(TopCount2));
                OnPropertyChanged(nameof(TopTotalCount));
            }
        }

        public int TopTotalCount
        {
            get { return _model.TopRebar.FirstLayer.Count + _model.TopRebar.SecondLayer.Count; }
        }

        public int BottomCount1
        {
            get { return _model.BottomRebar.FirstLayer.Count; }
            set
            {
                if (_model.BottomRebar.FirstLayer.Count == value)
                {
                    return;
                }

                _model.BottomRebar.FirstLayer.Count = value;
                OnPropertyChanged(nameof(BottomCount1));
                OnPropertyChanged(nameof(BottomTotalCount));
            }
        }

        public int BottomDiameter
        {
            get { return _model.BottomRebar.Diameter; }
            set
            {
                if (_model.BottomRebar.Diameter == value)
                {
                    return;
                }

                _model.BottomRebar.Diameter = value;
                OnPropertyChanged(nameof(BottomDiameter));
                OnPropertyChanged(nameof(BottomText1));
                OnPropertyChanged(nameof(BottomText2));
            }
        }

        public int BottomCount2
        {
            get { return _model.BottomRebar.SecondLayer.Count; }
            set
            {
                if (_model.BottomRebar.SecondLayer.Count == value)
                {
                    return;
                }

                _model.BottomRebar.SecondLayer.Count = value;
                OnPropertyChanged(nameof(BottomCount2));
                OnPropertyChanged(nameof(BottomTotalCount));
            }
        }

        public int BottomTotalCount
        {
            get { return _model.BottomRebar.FirstLayer.Count + _model.BottomRebar.SecondLayer.Count; }
        }

        public int StirrupLegs
        {
            get { return _model.Stirrup.Legs; }
            set
            {
                if (_model.Stirrup.Legs == value)
                {
                    return;
                }

                _model.Stirrup.Legs = value;
                OnPropertyChanged(nameof(StirrupLegs));
            }
        }

        public int StirrupDiameter
        {
            get { return _model.Stirrup.Diameter; }
            set
            {
                if (_model.Stirrup.Diameter == value)
                {
                    return;
                }

                _model.Stirrup.Diameter = value;
                OnPropertyChanged(nameof(StirrupDiameter));
            }
        }

        public int StirrupSpacing
        {
            get { return _model.Stirrup.Spacing; }
            set
            {
                if (_model.Stirrup.Spacing == value)
                {
                    return;
                }

                _model.Stirrup.Spacing = value;
                OnPropertyChanged(nameof(StirrupSpacing));
            }
        }

        public string SkinRebarText
        {
            get { return _model.SkinRebarText; }
            set
            {
                if (_model.SkinRebarText == value)
                {
                    return;
                }

                _model.SkinRebarText = value;
                OnPropertyChanged(nameof(SkinRebarText));
            }
        }

        public string TopText1
        {
            get { return FormatLayerText(_model.TopRebar.FirstLayer.Count, _model.TopRebar.Diameter); }
        }

        public string TopText2
        {
            get { return FormatLayerText(_model.TopRebar.SecondLayer.Count, _model.TopRebar.Diameter); }
        }

        public string BottomText1
        {
            get { return FormatLayerText(_model.BottomRebar.FirstLayer.Count, _model.BottomRebar.Diameter); }
        }

        public string BottomText2
        {
            get { return FormatLayerText(_model.BottomRebar.SecondLayer.Count, _model.BottomRebar.Diameter); }
        }

        public string StirrupText
        {
            get { return _model.Stirrup.Legs + "-HD" + _model.Stirrup.Diameter + "@" + _model.Stirrup.Spacing; }
        }

        public bool IsSectionEnabled
        {
            get { return _isSectionEnabled; }
            set
            {
                if (_isSectionEnabled == value)
                {
                    return;
                }

                _isSectionEnabled = value;
                OnPropertyChanged(nameof(IsSectionEnabled));
            }
        }

        public SectionItemViewModel(ScheduleItem model)
        {
            _model = model;
        }

        public void RefreshAll()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Position));
            OnPropertyChanged(nameof(WidthValue));
            OnPropertyChanged(nameof(HeightValue));

            OnPropertyChanged(nameof(TopCount1));
            OnPropertyChanged(nameof(TopCount2));
            OnPropertyChanged(nameof(TopDiameter));

            OnPropertyChanged(nameof(BottomCount1));
            OnPropertyChanged(nameof(BottomCount2));
            OnPropertyChanged(nameof(BottomDiameter));

            OnPropertyChanged(nameof(StirrupLegs));
            OnPropertyChanged(nameof(StirrupDiameter));
            OnPropertyChanged(nameof(StirrupSpacing));
            OnPropertyChanged(nameof(SkinRebarText));

            OnPropertyChanged(nameof(TopText1));
            OnPropertyChanged(nameof(TopText2));
            OnPropertyChanged(nameof(BottomText1));
            OnPropertyChanged(nameof(BottomText2));
            OnPropertyChanged(nameof(StirrupText));

            OnPropertyChanged(nameof(IsSectionEnabled));
        }

        private string FormatLayerText(int count, int diameter)
        {
            if (count <= 0 || diameter <= 0)
            {
                return "-";
            }

            return count + "-HD" + diameter;
        }
    }
}