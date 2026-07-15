using GirderSchedule.App.Utils;
using GirderSchedule.App.ViewModels.Settings;
using System.ComponentModel;
using System.Windows;

namespace GirderSchedule.App.Views.Settings
{
    public partial class AppSettingWindow : Window
    {
        private AppSettingWindowViewModel _viewModel;
        private bool _isClosingByCommand;

        public AppSettingWindow()
        {
            InitializeComponent();

            DataContextChanged += AppSettingWindow_DataContextChanged;
            Loaded += Window_Loaded;
            Closing += AppSettingWindow_Closing;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowSizeHelper.FitToWorkArea(this);
        }

        private void AppSettingWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CloseRequested -= ViewModel_CloseRequested;
            }

            _viewModel = e.NewValue as AppSettingWindowViewModel;

            if (_viewModel != null)
            {
                _viewModel.CloseRequested += ViewModel_CloseRequested;
            }
        }

        private void ViewModel_CloseRequested(object sender, bool dialogResult)
        {
            _isClosingByCommand = true;
            DialogResult = dialogResult;
        }

        private void AppSettingWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_isClosingByCommand || _viewModel == null)
            {
                return;
            }

            _isClosingByCommand = true;
            _viewModel.CancelCommand.Execute(null);
        }
    }
}