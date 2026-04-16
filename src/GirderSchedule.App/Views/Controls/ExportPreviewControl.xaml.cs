using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GirderSchedule.App.ViewModels.Export;
using GirderSchedule.Renderer.Wpf;

namespace GirderSchedule.App.Views.Controls
{
	public partial class ExportPreviewControl : UserControl
	{
		private INotifyPropertyChanged _notify;

		public ExportPreviewControl()
		{
			InitializeComponent();
			Loaded += ExportPreviewControl_Loaded;
			DataContextChanged += ExportPreviewControl_DataContextChanged;
		}

		private void ExportPreviewControl_Loaded(object sender, RoutedEventArgs e)
		{
			Render();
		}

		private void ExportPreviewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_notify != null)
			{
				_notify.PropertyChanged -= Notify_PropertyChanged;
				_notify = null;
			}

			_notify = e.NewValue as INotifyPropertyChanged;
			if (_notify != null)
			{
				_notify.PropertyChanged += Notify_PropertyChanged;
			}

			Render();
		}

		private void Notify_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Render();
		}

		private void Render()
		{
			var vm = DataContext as ExportPreviewViewModel;
			if (vm == null || vm.Scene == null)
			{
				return;
			}

			ExportPreviewCanvasRenderer.Render(PART_Canvas, vm.Scene, vm.Zoom);
		}
	}
}