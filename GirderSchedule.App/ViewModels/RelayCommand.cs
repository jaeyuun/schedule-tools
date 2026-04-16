using System;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels
{
	public class RelayCommand : ICommand
	{
		private readonly Action _execute;
		private readonly Func<bool> _canExecute;

		public event EventHandler CanExecuteChanged;

		public RelayCommand(Action execute, Func<bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			if (_canExecute == null)
			{
				return true;
			}

			return _canExecute();
		}

		public void Execute(object parameter)
		{
			_execute();
		}

		public void RaiseCanExecuteChanged()
		{
			var handler = CanExecuteChanged;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}
	}
}