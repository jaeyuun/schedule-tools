using ScheduleTools.Wpf.Settings.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace ScheduleTools.Wpf.Settings.Views
{
    public partial class AppSettingWindow : Window
    {
        private AppSettingWindowViewModel? _viewModel;
        private bool _isClosingByCommand;

        public AppSettingWindow()
        {
            InitializeComponent();

            DataContextChanged += AppSettingWindow_DataContextChanged;
            Closing += AppSettingWindow_Closing;
            Closed += AppSettingWindow_Closed;
        }

        private void AppSettingWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UnsubscribeViewModel();

            _viewModel = e.NewValue as AppSettingWindowViewModel;

            if (_viewModel != null)
            {
                _viewModel.CloseRequested += ViewModel_CloseRequested;
            }
        }

        private void ViewModel_CloseRequested(object? sender, bool dialogResult)
        {
            _isClosingByCommand = true;
            DialogResult = dialogResult;
        }

        private void AppSettingWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (_isClosingByCommand || _viewModel == null)
            {
                return;
            }

            _isClosingByCommand = true;
            _viewModel.RestoreOriginalFontSize();
        }

        private void AppSettingWindow_Closed(object? sender, EventArgs e)
        {
            UnsubscribeViewModel();
        }

        private void UnsubscribeViewModel()
        {
            if (_viewModel == null)
            {
                return;
            }

            _viewModel.CloseRequested -= ViewModel_CloseRequested;
            _viewModel = null;
        }
    }
}