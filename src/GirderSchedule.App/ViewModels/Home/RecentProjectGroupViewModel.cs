using System.Collections.ObjectModel;
using GirderSchedule.App.ViewModels;

namespace GirderSchedule.App.ViewModels.Home
{
    public sealed class RecentProjectGroupViewModel : ViewModelBase
    {
        private bool _isExpanded = true;

        public string Title { get; private set; }
        public ObservableCollection<RecentProjectViewModel> Items { get; private set; }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded == value)
                {
                    return;
                }

                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        public RecentProjectGroupViewModel(string title)
        {
            Title = title;
            Items = new ObservableCollection<RecentProjectViewModel>();
        }
    }
}