using GirderSchedule.App.ViewModels.Settings;
using System;
using System.Windows;

namespace GirderSchedule.App.Views.Settings
{
    public partial class DxfExportSettingWindow : Window
    {
        public DxfExportSettingWindow()
        {
            InitializeComponent();
            DataContextChanged += DxfExportSettingWindow_DataContextChanged;
        }

        private void DxfExportSettingWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldViewModel = e.OldValue as DxfExportSettingWindowViewModel;

            if (oldViewModel != null)
            {
                oldViewModel.Accepted -= ViewModel_Accepted;
                oldViewModel.Cancelled -= ViewModel_Cancelled;
            }

            var newViewModel = e.NewValue as DxfExportSettingWindowViewModel;

            if (newViewModel != null)
            {
                newViewModel.Accepted += ViewModel_Accepted;
                newViewModel.Cancelled += ViewModel_Cancelled;
            }
        }

        private void ViewModel_Accepted(object? sender, EventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ViewModel_Cancelled(object? sender, EventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
