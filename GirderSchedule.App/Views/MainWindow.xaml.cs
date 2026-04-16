using System.Windows;
using GirderSchedule.App.ViewModels;

namespace GirderSchedule.App.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new MainViewModel();
		}
	}
}