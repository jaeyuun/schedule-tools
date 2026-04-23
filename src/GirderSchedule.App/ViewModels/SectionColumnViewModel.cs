using System.ComponentModel;

namespace GirderSchedule.App.ViewModels
{
	public class SectionColumnViewModel : INotifyPropertyChanged
	{
		private string _m = string.Empty;
		private string _v = string.Empty;

		private string _top1Count = string.Empty;
		private string _top1Dia = string.Empty;
		private string _top2Count = string.Empty;
		private string _top2Dia = string.Empty;

		private string _bottom1Count = string.Empty;
		private string _bottom1Dia = string.Empty;
		private string _bottom2Count = string.Empty;
		private string _bottom2Dia = string.Empty;

		private string _stirrupCount = string.Empty;
		private string _stirrupDia = string.Empty;
		private string _stirrupSpacing = string.Empty;

		private string _skinRebar = string.Empty;

		public event PropertyChangedEventHandler PropertyChanged;

		public string M
		{
			get { return _m; }
			set
			{
				if (_m == value)
				{
					return;
				}

				_m = value;
				OnPropertyChanged(nameof(M));
			}
		}

		public string V
		{
			get { return _v; }
			set
			{
				if (_v == value)
				{
					return;
				}

				_v = value;
				OnPropertyChanged(nameof(V));
			}
		}

		public string Top1Count
		{
			get { return _top1Count; }
			set
			{
				if (_top1Count == value)
				{
					return;
				}

				_top1Count = value;
				OnPropertyChanged(nameof(Top1Count));
			}
		}

		public string Top1Dia
		{
			get { return _top1Dia; }
			set
			{
				if (_top1Dia == value)
				{
					return;
				}

				_top1Dia = value;
				OnPropertyChanged(nameof(Top1Dia));
			}
		}

		public string Top2Count
		{
			get { return _top2Count; }
			set
			{
				if (_top2Count == value)
				{
					return;
				}

				_top2Count = value;
				OnPropertyChanged(nameof(Top2Count));
			}
		}

		public string Top2Dia
		{
			get { return _top2Dia; }
			set
			{
				if (_top2Dia == value)
				{
					return;
				}

				_top2Dia = value;
				OnPropertyChanged(nameof(Top2Dia));
			}
		}

		public string Bottom1Count
		{
			get { return _bottom1Count; }
			set
			{
				if (_bottom1Count == value)
				{
					return;
				}

				_bottom1Count = value;
				OnPropertyChanged(nameof(Bottom1Count));
			}
		}

		public string Bottom1Dia
		{
			get { return _bottom1Dia; }
			set
			{
				if (_bottom1Dia == value)
				{
					return;
				}

				_bottom1Dia = value;
				OnPropertyChanged(nameof(Bottom1Dia));
			}
		}

		public string Bottom2Count
		{
			get { return _bottom2Count; }
			set
			{
				if (_bottom2Count == value)
				{
					return;
				}

				_bottom2Count = value;
				OnPropertyChanged(nameof(Bottom2Count));
			}
		}

		public string Bottom2Dia
		{
			get { return _bottom2Dia; }
			set
			{
				if (_bottom2Dia == value)
				{
					return;
				}

				_bottom2Dia = value;
				OnPropertyChanged(nameof(Bottom2Dia));
			}
		}

		public string StirrupCount
		{
			get { return _stirrupCount; }
			set
			{
				if (_stirrupCount == value)
				{
					return;
				}

				_stirrupCount = value;
				OnPropertyChanged(nameof(StirrupCount));
			}
		}

		public string StirrupDia
		{
			get { return _stirrupDia; }
			set
			{
				if (_stirrupDia == value)
				{
					return;
				}

				_stirrupDia = value;
				OnPropertyChanged(nameof(StirrupDia));
			}
		}

		public string StirrupSpacing
		{
			get { return _stirrupSpacing; }
			set
			{
				if (_stirrupSpacing == value)
				{
					return;
				}

				_stirrupSpacing = value;
				OnPropertyChanged(nameof(StirrupSpacing));
			}
		}

		public string SkinRebar
		{
			get { return _skinRebar; }
			set
			{
				if (_skinRebar == value)
				{
					return;
				}

				_skinRebar = value;
				OnPropertyChanged(nameof(SkinRebar));
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler == null)
			{
				return;
			}

			handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}