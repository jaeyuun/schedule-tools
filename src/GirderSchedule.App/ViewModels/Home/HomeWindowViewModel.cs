namespace GirderSchedule.App.ViewModels.Home
{
    public sealed class HomeWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private HomeStartViewModel _homeStartViewModel;

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            private set
            {
                if (_currentViewModel == value)
                {
                    return;
                }

                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public HomeWindowViewModel()
        {
            ShowHome();
        }

        private void ShowHome()
        {
            _homeStartViewModel = new HomeStartViewModel(ShowProjectSetting);
            CurrentViewModel = _homeStartViewModel;
        }

        private void ShowProjectSetting()
        {
            var viewModel = new ProjectCreateWindowViewModel(false);
            viewModel.BackRequested = ShowHome;
            viewModel.ProjectCreated = ShowHome;

            CurrentViewModel = viewModel;
        }
    }
}