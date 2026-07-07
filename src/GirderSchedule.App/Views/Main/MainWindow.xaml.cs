using GirderSchedule.App.Utils;
using GirderSchedule.App.ViewModels.Main;
using GirderSchedule.Domain.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GirderSchedule.App.Views.Main
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            Loaded += Window_Loaded;
            UpdatePreviewToolbar();
        }

        public MainWindow(ScheduleProject project, string filePath)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(project, filePath);
            Loaded += Window_Loaded;
            UpdatePreviewToolbar();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowSizeHelper.FitToWorkArea(this);
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

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ReferenceEquals(e.OriginalSource, MainTabControl))
            {
                return;
            }

            UpdatePreviewToolbar();
        }

        private void PreviewPrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            PreviewControl.GoPreviousPage();
            UpdatePreviewToolbar();
        }

        private void PreviewNextPageButton_Click(object sender, RoutedEventArgs e)
        {
            PreviewControl.GoNextPage();
            UpdatePreviewToolbar();
        }

        private void PreviewFitButton_Click(object sender, RoutedEventArgs e)
        {
            PreviewControl.FitToScreenView();
            UpdatePreviewToolbar();
        }

        private void PreviewControl_PreviewStateChanged(object sender, System.EventArgs e)
        {
            UpdatePreviewToolbar();
        }

        private void UpdatePreviewToolbar()
        {
            if (PreviewToolbar == null || PreviewControl == null)
            {
                return;
            }

            var isPreviewSelected = ReferenceEquals(MainTabControl.SelectedItem, PreviewTabItem);

            PreviewToolbar.Visibility = isPreviewSelected ? Visibility.Visible : Visibility.Collapsed;
            EditorTabContent.Visibility = isPreviewSelected ? Visibility.Collapsed : Visibility.Visible;
            PreviewTabContent.Visibility = isPreviewSelected ? Visibility.Visible : Visibility.Collapsed;

            PreviewPrevPageButton.IsEnabled = PreviewControl.CanGoPreviousPage;
            PreviewNextPageButton.IsEnabled = PreviewControl.CanGoNextPage;
            PreviewZoomTextBlock.Text = PreviewControl.ZoomText;
            PreviewPageTextBlock.Text = PreviewControl.PageText;
        }
    }
}