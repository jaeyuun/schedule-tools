using ScheduleTools.Wpf.Home.Models;
using ScheduleTools.Wpf.Home.ViewModels;
using System.Windows;

namespace ScheduleTools.Wpf.Home.Views
{
    public partial class ProjectCreateWindow : Window
    {
        public ProjectCreateResult? Result { get; private set; }

        public ProjectCreateWindow()
        {
            InitializeComponent();
            DataContextChanged += Window_DataContextChanged;
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ProjectCreateWindowViewModel oldViewModel)
                oldViewModel.CloseRequested -= ViewModel_CloseRequested;

            if (e.NewValue is ProjectCreateWindowViewModel newViewModel)
                newViewModel.CloseRequested += ViewModel_CloseRequested;
        }

        private void ViewModel_CloseRequested(object? sender, bool confirmed)
        {
            if (confirmed && DataContext is ProjectCreateWindowViewModel viewModel)
                Result = viewModel.CreateResult();

            DialogResult = confirmed;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is ProjectCreateWindowViewModel viewModel)
                viewModel.CloseRequested -= ViewModel_CloseRequested;

            DataContextChanged -= Window_DataContextChanged;

            base.OnClosed(e);
        }
    }
}