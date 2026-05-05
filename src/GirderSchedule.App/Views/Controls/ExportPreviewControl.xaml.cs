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
			Unloaded += ExportPreviewControl_Unloaded;
			DataContextChanged += ExportPreviewControl_DataContextChanged;
		}

		private void ExportPreviewControl_Loaded(object sender, RoutedEventArgs e)
		{
			Render();
		}

		private void ExportPreviewControl_Unloaded(object sender, RoutedEventArgs e)
		{
			DetachNotify();
		}

		private void ExportPreviewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			DetachNotify();

			_notify = e.NewValue as INotifyPropertyChanged;
			if (_notify != null)
			{
				_notify.PropertyChanged += Notify_PropertyChanged;
			}

			Render();
		}

		private void DetachNotify()
		{
			if (_notify == null)
			{
				return;
			}

			_notify.PropertyChanged -= Notify_PropertyChanged;
			_notify = null;
		}

		private void Notify_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(Render);
				return;
			}

			Render();
		}

		private void Render()
		{
			if (PART_Canvas == null)
			{
				return;
			}

			var vm = DataContext as ExportPreviewViewModel;
			if (vm == null || vm.Scene == null)
			{
				PART_Canvas.Children.Clear();
				return;
			}

			ExportPreviewCanvasRenderer.Render(PART_Canvas, vm.Scene, vm.Zoom);
		}
	}
}