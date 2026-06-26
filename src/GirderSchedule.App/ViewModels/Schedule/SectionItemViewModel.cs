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
                OnPropertyChanged(nameof(IsSkinRebarInputEnabled));
            }
        }

        public bool IsSkinRebarInputEnabled
        {
            get { return _model.Section.Height > 900.0; }
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
            get { return _model.MainRebar.Top.FirstCount; }
            set
            {
                if (_model.MainRebar.Top.FirstCount == value)
                {
                    return;
                }

                _model.MainRebar.Top.FirstCount = value;
                OnPropertyChanged(nameof(TopCount1));
                OnPropertyChanged(nameof(TopTotalCount));
                OnPropertyChanged(nameof(TopText1));
            }
        }

        public double TopDiameter
        {
            get { return _model.MainRebar.Diameter; }
            set
            {
                if (_model.MainRebar.Diameter == value)
                {
                    return;
                }

                _model.MainRebar.Diameter = value;
                OnPropertyChanged(nameof(TopDiameter));
                OnPropertyChanged(nameof(TopText1));
                OnPropertyChanged(nameof(TopText2));
            }
        }

        public int TopCount2
        {
            get { return _model.MainRebar.Top.SecondCount; }
            set
            {
                if (_model.MainRebar.Top.SecondCount == value)
                {
                    return;
                }

                _model.MainRebar.Top.SecondCount = value;
                OnPropertyChanged(nameof(TopCount2));
                OnPropertyChanged(nameof(TopTotalCount));
                OnPropertyChanged(nameof(TopText2));
            }
        }

        public int TopTotalCount
        {
            get { return _model.MainRebar.Top.FirstCount + _model.MainRebar.Top.SecondCount; }
        }

        public int BottomCount1
        {
            get { return _model.MainRebar.Bottom.FirstCount; }
            set
            {
                if (_model.MainRebar.Bottom.FirstCount == value)
                {
                    return;
                }

                _model.MainRebar.Bottom.FirstCount = value;
                OnPropertyChanged(nameof(BottomCount1));
                OnPropertyChanged(nameof(BottomTotalCount));
                OnPropertyChanged(nameof(BottomText1));
            }
        }

        public double BottomDiameter
        {
            get { return _model.MainRebar.Diameter; }
            set
            {
                if (_model.MainRebar.Diameter == value)
                {
                    return;
                }

                _model.MainRebar.Diameter = value;
                OnPropertyChanged(nameof(BottomDiameter));
                OnPropertyChanged(nameof(BottomText1));
                OnPropertyChanged(nameof(BottomText2));
            }
        }

        public int BottomCount2
        {
            get { return _model.MainRebar.Bottom.SecondCount; }
            set
            {
                if (_model.MainRebar.Bottom.SecondCount == value)
                {
                    return;
                }

                _model.MainRebar.Bottom.SecondCount = value;
                OnPropertyChanged(nameof(BottomCount2));
                OnPropertyChanged(nameof(BottomTotalCount));
                OnPropertyChanged(nameof(BottomText2));
            }
        }

        public int BottomTotalCount
        {
            get { return _model.MainRebar.Bottom.FirstCount + _model.MainRebar.Bottom.SecondCount; }
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
            get { return _textService.FormatLayerText(_model.MainRebar.Top.FirstCount, _model.MainRebar.Diameter); }
        }

        public string TopText2
        {
            get { return _textService.FormatLayerText(_model.MainRebar.Top.SecondCount, _model.MainRebar.Diameter); }
        }

        public string BottomText1
        {
            get { return _textService.FormatLayerText(_model.MainRebar.Bottom.FirstCount, _model.MainRebar.Diameter); }
        }

        public string BottomText2
        {
            get { return _textService.FormatLayerText(_model.MainRebar.Bottom.SecondCount, _model.MainRebar.Diameter); }
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
            OnPropertyChanged(nameof(Position));
            OnPropertyChanged(nameof(WidthValue));
            OnPropertyChanged(nameof(HeightValue));
            OnPropertyChanged(nameof(IsSkinRebarInputEnabled));
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