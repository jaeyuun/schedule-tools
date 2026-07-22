using Microsoft.Win32;
using ScheduleTools.Wpf.ProjectCreate.Models;
using ScheduleTools.Wpf.ProjectCreate.ViewModels;
using System.Windows;

namespace ScheduleTools.Wpf.ProjectCreate.Views
{
    public partial class ProjectCreateWindow : Window
    {
        private ProjectCreateWindowViewModel? _viewModel;

        public ProjectCreateResult? Result { get; private set; }

        public ProjectCreateWindow()
        {
            InitializeComponent();

            DataContextChanged += Window_DataContextChanged;
            Closed += Window_Closed;
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UnsubscribeViewModel();

            _viewModel = e.NewValue as ProjectCreateWindowViewModel;

            if (_viewModel == null)
                return;

            _viewModel.BrowseFolderRequested += ViewModel_BrowseFolderRequested;
            _viewModel.CloseRequested += ViewModel_CloseRequested;
        }

        private void ViewModel_BrowseFolderRequested(object? sender, EventArgs e)
        {
            if (_viewModel == null)
                return;

            var dialog = new OpenFolderDialog
            {
                Title = "프로젝트 저장 폴더 선택",
                InitialDirectory = _viewModel.ProjectFolder,
                Multiselect = false
            };

            if (dialog.ShowDialog(this) == true)
                _viewModel.ProjectFolder = dialog.FolderName;
        }

        private void ViewModel_CloseRequested(object? sender, bool result)
        {
            if (result && _viewModel != null)
                Result = _viewModel.CreateResult();

            DialogResult = result;
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            UnsubscribeViewModel();
            DataContextChanged -= Window_DataContextChanged;
            Closed -= Window_Closed;
        }

        private void UnsubscribeViewModel()
        {
            if (_viewModel == null)
                return;

            _viewModel.BrowseFolderRequested -= ViewModel_BrowseFolderRequested;
            _viewModel.CloseRequested -= ViewModel_CloseRequested;
            _viewModel = null;
        }
    }
}