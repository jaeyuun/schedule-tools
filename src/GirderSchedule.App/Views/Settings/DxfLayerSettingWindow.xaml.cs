using GirderSchedule.App.ViewModels.Settings;
using System;
using System.Windows;

namespace GirderSchedule.App.Views.Settings
{
    public partial class DxfLayerSettingWindow : Window
    {
        public DxfLayerSettingWindow()
        {
            InitializeComponent();
            DataContextChanged += DxfLayerSettingWindow_DataContextChanged;
        }

        private void DxfLayerSettingWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldViewModel = e.OldValue as DxfLayerSettingWindowViewModel;

            if (oldViewModel != null)
            {
                oldViewModel.Accepted -= ViewModel_Accepted;
            }

            var newViewModel = e.NewValue as DxfLayerSettingWindowViewModel;

            if (newViewModel != null)
            {
                newViewModel.Accepted += ViewModel_Accepted;
            }
        }

        private void ViewModel_Accepted(object? sender, EventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
