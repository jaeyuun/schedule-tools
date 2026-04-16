using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GirderSchedule.App.ViewModels
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(field, value))
			{
				return;
			}

			field = value;
			OnPropertyChanged(propertyName);
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}