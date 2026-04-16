using System.Windows;
using GirderSchedule.App.ViewModels.Export;

namespace GirderSchedule.App.Views
{
	public partial class ExportWindow : Window
	{
		public ExportWindow()
		{
			InitializeComponent();
			DataContextChanged += ExportWindow_DataContextChanged;
		}

		private void ExportWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var oldVm = e.OldValue as ExportWindowViewModel;
			if (oldVm != null)
			{
				oldVm.RequestClose -= ViewModel_RequestClose;
			}

			var newVm = e.NewValue as ExportWindowViewModel;
			if (newVm != null)
			{
				newVm.RequestClose += ViewModel_RequestClose;
			}
		}

		private void ViewModel_RequestClose(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}