using System.ComponentModel;
using System.Windows;
using GirderSchedule.App.ViewModels.Main;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.App.Views.Main
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        public MainWindow(ScheduleProject project, string filePath)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(project, filePath);
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