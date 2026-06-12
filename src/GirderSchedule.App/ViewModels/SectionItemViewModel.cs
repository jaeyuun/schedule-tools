using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Models;
using System;

namespace GirderSchedule.App.ViewModels
{
    public sealed class SectionItemViewModel : ViewModelBase
    {
        private readonly ScheduleItem _model;
        private bool _isSectionEnabled = true;
        private int _skinRebarDiameter;
        private int _skinRebarSpacing;

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

        public int SkinRebarDiameter
        {
            get { return _skinRebarDiameter; }
            set
            {
                if (_skinRebarDiameter == value)
                {
                    return;
                }

                _skinRebarDiameter = value;
                UpdateSkinRebarText();
                OnPropertyChanged(nameof(SkinRebarDiameter));
                OnPropertyChanged(nameof(SkinRebarText));
            }
        }

        public int SkinRebarSpacing
        {
            get { return _skinRebarSpacing; }
            set
            {
                if (_skinRebarSpacing == value)
                {
                    return;
                }

                _skinRebarSpacing = value;
                UpdateSkinRebarText();
                OnPropertyChanged(nameof(SkinRebarSpacing));
                OnPropertyChanged(nameof(SkinRebarText));
            }
        }

        public string SkinRebarText
        {
            get { return BuildSkinRebarText(); }
            set
            {
                ApplySkinRebarText(value);
                UpdateSkinRebarText();
                OnPropertyChanged(nameof(SkinRebarDiameter));
                OnPropertyChanged(nameof(SkinRebarSpacing));
                OnPropertyChanged(nameof(SkinRebarText));
            }
        }

        public string MomentValue
        {
            get
            {
                if (!_model.MemberForce.Moment.HasValue)
                {
                    return string.Empty;
                }

                return _model.MemberForce.Moment.Value.ToString("0");
            }
            set
            {
                var number = ParseNullableDouble(value);

                if (_model.MemberForce.Moment == number)
                {
                    return;
                }

                _model.MemberForce.Moment = number;
                OnPropertyChanged(nameof(MomentValue));
                OnPropertyChanged(nameof(MomentText));
            }
        }

        public string ShearValue
        {
            get
            {
                if (!_model.MemberForce.Shear.HasValue)
                {
                    return string.Empty;
                }

                return _model.MemberForce.Shear.Value.ToString("0");
            }
            set
            {
                var number = ParseNullableDouble(value);

                if (_model.MemberForce.Shear == number)
                {
                    return;
                }

                _model.MemberForce.Shear = number;
                OnPropertyChanged(nameof(ShearValue));
                OnPropertyChanged(nameof(ShearText));
            }
        }

        public string MomentText
        {
            get
            {
                if (!_model.MemberForce.Moment.HasValue)
                {
                    return string.Empty;
                }

                return "M = " + _model.MemberForce.Moment.Value.ToString("0");
            }
        }

        public string ShearText
        {
            get
            {
                if (!_model.MemberForce.Shear.HasValue)
                {
                    return string.Empty;
                }

                return "V = " + _model.MemberForce.Shear.Value.ToString("0");
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
            get { return FormatStirrupText(); }
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
            ApplySkinRebarText(_model.SkinRebarText);
            UpdateSkinRebarText();
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
            OnPropertyChanged(nameof(StirrupText));
            OnPropertyChanged(nameof(SkinRebarDiameter));
            OnPropertyChanged(nameof(SkinRebarSpacing));
            OnPropertyChanged(nameof(SkinRebarText));
            OnPropertyChanged(nameof(MomentValue));
            OnPropertyChanged(nameof(ShearValue));
            OnPropertyChanged(nameof(MomentText));
            OnPropertyChanged(nameof(ShearText));
            OnPropertyChanged(nameof(TopText1));
            OnPropertyChanged(nameof(TopText2));
            OnPropertyChanged(nameof(BottomText1));
            OnPropertyChanged(nameof(BottomText2));
            OnPropertyChanged(nameof(IsSectionEnabled));
        }

        private double? ParseNullableDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            double result;

            if (!double.TryParse(value, out result))
            {
                return null;
            }

            return result;
        }

        private string FormatLayerText(int count, int diameter)
        {
            if (count <= 0 || diameter <= 0)
            {
                return "-";
            }

            return count + "-HD" + diameter;
        }

        private string FormatStirrupText()
        {
            if (_model.Stirrup.Legs <= 0 || _model.Stirrup.Diameter <= 0 || _model.Stirrup.Spacing <= 0)
            {
                return "-";
            }

            return _model.Stirrup.Legs + "-HD" + _model.Stirrup.Diameter + "@" + _model.Stirrup.Spacing;
        }

        private void UpdateSkinRebarText()
        {
            _model.SkinRebarText = BuildSkinRebarText();
        }

        private string BuildSkinRebarText()
        {
            if (_skinRebarDiameter <= 0 || _skinRebarSpacing <= 0)
            {
                return "-";
            }

            return "HD" + _skinRebarDiameter + "@" + _skinRebarSpacing;
        }

        private void ApplySkinRebarText(string value)
        {
            _skinRebarDiameter = 0;
            _skinRebarSpacing = 0;

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var text = value.Trim();

            if (text == "-")
            {
                return;
            }

            if (text.StartsWith("HD", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(2);
            }

            var parts = text.Split('@');

            if (parts.Length != 2)
            {
                int diameter;
                if (int.TryParse(text, out diameter))
                {
                    _skinRebarDiameter = diameter;
                }

                return;
            }

            int parsedDiameter;
            int parsedSpacing;

            if (int.TryParse(parts[0].Trim(), out parsedDiameter))
            {
                _skinRebarDiameter = parsedDiameter;
            }

            if (int.TryParse(parts[1].Trim(), out parsedSpacing))
            {
                _skinRebarSpacing = parsedSpacing;
            }
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