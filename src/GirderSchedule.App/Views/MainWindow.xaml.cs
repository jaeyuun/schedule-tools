using System.ComponentModel;
using System.Windows;
using GirderSchedule.App.ViewModels;

namespace GirderSchedule.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;

            if (viewModel == null)
            {
                return;
            }

            if (!viewModel.ConfirmSaveIfDirty())
            {
                e.Cancel = true;
            }
        }
    }
}