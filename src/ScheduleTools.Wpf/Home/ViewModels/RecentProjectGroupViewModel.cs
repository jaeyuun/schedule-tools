using ScheduleTools.Wpf.Mvvm;
using System.Collections.ObjectModel;

namespace ScheduleTools.Wpf.Home.ViewModels
{
    public sealed class RecentProjectGroupViewModel : ViewModelBase
    {
        private bool _isExpanded = true;

        public RecentProjectGroupViewModel(string title)
        {
            Title = title ?? string.Empty;
            Projects = new ObservableCollection<RecentProjectViewModel>();
        }

        public string Title { get; }

        public ObservableCollection<RecentProjectViewModel> Projects { get; }

        public bool HasProjects => Projects.Count > 0;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }
    }
}