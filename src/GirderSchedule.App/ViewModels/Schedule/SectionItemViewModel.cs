using GirderSchedule.App.Services.Schedule;
using GirderSchedule.Domain.Models;
using System;

namespace GirderSchedule.App.ViewModels.Schedule
{
    public sealed class SectionItemViewModel : ViewModelBase
    {
        private readonly ScheduleItem _model;
        private readonly SectionItemTextService _textService = new SectionItemTextService();

        public event EventHandler Changed;

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

        public string SectionNote
        {
            get { return _model.Section.Note; }
            set
            {
                if (_model.Section.Note == value)
                {
                    return;
                }

                _model.Section.Note = value ?? string.Empty;
                OnPropertyChanged(nameof(SectionNote));
                RaiseChanged();
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
                OnPropertyChanged(nameof(TopText1));
            }
        }

        public double TopDiameter
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
                OnPropertyChanged(nameof(TopText2));
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
                OnPropertyChanged(nameof(BottomText1));
            }
        }

        public double BottomDiameter
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
                OnPropertyChanged(nameof(BottomText2));
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
                OnPropertyChanged(nameof(StirrupText));
            }
        }

        public double StirrupDiameter
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
                OnPropertyChanged(nameof(StirrupText));
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
                OnPropertyChanged(nameof(StirrupText));
            }
        }

        public double SkinRebarDiameter
        {
            get { return _model.SkinRebar.Diameter; }
            set
            {
                if (_model.SkinRebar.Diameter == value)
                {
                    return;
                }

                _model.SkinRebar.Diameter = value;
                OnPropertyChanged(nameof(SkinRebarDiameter));
                OnPropertyChanged(nameof(SkinRebarText));
            }
        }

        public int SkinRebarSpacing
        {
            get { return _model.SkinRebar.Spacing; }
            set
            {
                if (_model.SkinRebar.Spacing == value)
                {
                    return;
                }

                _model.SkinRebar.Spacing = value;
                OnPropertyChanged(nameof(SkinRebarSpacing));
                OnPropertyChanged(nameof(SkinRebarText));
            }
        }

		public string SkinRebarNote
		{
			get { return _model.SkinRebar.Note; }
			set
			{
				if (_model.SkinRebar.Note == value)
				{
					return;
				}

				_model.SkinRebar.Note = value ?? string.Empty;
				OnPropertyChanged(nameof(SkinRebarNote));
				RaiseChanged();
			}
		}

        public double? MomentValue
        {
            get { return _model.MemberForce.Moment; }
            set
            {
                if (_model.MemberForce.Moment == value)
                {
                    return;
                }

                _model.MemberForce.Moment = value;
                OnPropertyChanged(nameof(MomentText));
            }
        }

        public double? ShearValue
        {
            get { return _model.MemberForce.Shear; }
            set
            {
                if (_model.MemberForce.Shear == value)
                {
                    return;
                }

                _model.MemberForce.Shear = value;
                OnPropertyChanged(nameof(ShearText));
            }
        }

        public string MomentText
        {
            get { return _textService.FormatMomentText(_model.MemberForce.Moment); }
        }

        public string ShearText
        {
            get { return _textService.FormatShearText(_model.MemberForce.Shear); }
        }

        public string TopText1
        {
            get { return _textService.FormatLayerText(_model.TopRebar.FirstLayer.Count, _model.TopRebar.Diameter); }
        }

        public string TopText2
        {
            get { return _textService.FormatLayerText(_model.TopRebar.SecondLayer.Count, _model.TopRebar.Diameter); }
        }

        public string BottomText1
        {
            get { return _textService.FormatLayerText(_model.BottomRebar.FirstLayer.Count, _model.BottomRebar.Diameter); }
        }

        public string BottomText2
        {
            get { return _textService.FormatLayerText(_model.BottomRebar.SecondLayer.Count, _model.BottomRebar.Diameter); }
        }

        public string StirrupText
        {
            get { return _textService.FormatStirrupText(_model.Stirrup); }
        }

        public string SkinRebarText
        {
            get { return _textService.FormatSkinRebarText(_model.SkinRebar); }
        }

        public bool IsSectionEnabled
        {
            get { return _model.IsSectionEnabled; }
            set
            {
                if (_model.IsSectionEnabled == value)
                {
                    return;
                }

                _model.IsSectionEnabled = value;
                OnPropertyChanged(nameof(IsSectionEnabled));
                RaiseChanged();
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
            OnPropertyChanged(nameof(TopTotalCount));
            OnPropertyChanged(nameof(BottomCount1));
            OnPropertyChanged(nameof(BottomCount2));
            OnPropertyChanged(nameof(BottomDiameter));
            OnPropertyChanged(nameof(BottomTotalCount));
            OnPropertyChanged(nameof(StirrupLegs));
            OnPropertyChanged(nameof(StirrupDiameter));
            OnPropertyChanged(nameof(StirrupSpacing));
            OnPropertyChanged(nameof(SkinRebarDiameter));
            OnPropertyChanged(nameof(SkinRebarSpacing));
            OnPropertyChanged(nameof(MomentValue));
            OnPropertyChanged(nameof(ShearValue));
            OnPropertyChanged(nameof(MomentText));
            OnPropertyChanged(nameof(ShearText));
            OnPropertyChanged(nameof(TopText1));
            OnPropertyChanged(nameof(TopText2));
            OnPropertyChanged(nameof(BottomText1));
            OnPropertyChanged(nameof(BottomText2));
            OnPropertyChanged(nameof(StirrupText));
            OnPropertyChanged(nameof(SkinRebarText));
            OnPropertyChanged(nameof(IsSectionEnabled));
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