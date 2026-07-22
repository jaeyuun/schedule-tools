using ScheduleTools.Core.RecentProjects;
using ScheduleTools.Wpf.Commands;
using ScheduleTools.Wpf.Home.Contracts;
using ScheduleTools.Wpf.Home.Models;
using ScheduleTools.Wpf.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace ScheduleTools.Wpf.Home.ViewModels
{
    public sealed class HomeStartViewModel : ViewModelBase
    {
        private readonly IProjectHomeHandler _projectHomeHandler;
        private readonly RecentProjectService _recentProjectService;
        private readonly Action<string, string> _showNotice;

        private RecentProjectViewModel? _selectedRecentProject;
        private string _searchText = string.Empty;
        
        public string WindowTitle { get; }
        public ObservableCollection<RecentProjectGroupViewModel> RecentProjectGroups { get; }
        public ICommand CreateProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand OpenSelectedProjectCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public RecentProjectViewModel? SelectedRecentProject
        {
            get => _selectedRecentProject;
            set
            {
                if (!SetProperty(ref _selectedRecentProject, value))
                    return;

                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (!SetProperty(ref _searchText, value ?? string.Empty))
                    return;

                LoadRecentProjects();
            }
        }

        public bool HasRecentProjects => RecentProjectGroups.Any(group => group.Projects.Count > 0);
        public bool HasNoRecentProjects => !HasRecentProjects;

        public HomeStartViewModel(IProjectHomeHandler projectHomeHandler, RecentProjectService recentProjectService, HomeTextOptions textOptions, Action<string, string> showNotice)
        {
            _projectHomeHandler = projectHomeHandler ?? throw new ArgumentNullException(nameof(projectHomeHandler));
            _recentProjectService = recentProjectService ?? throw new ArgumentNullException(nameof(recentProjectService));
            _showNotice = showNotice ?? throw new ArgumentNullException(nameof(showNotice));

            ArgumentNullException.ThrowIfNull(textOptions);

            WindowTitle = string.IsNullOrWhiteSpace(textOptions.WindowTitle)
                ? "Schedule Tool"
                : textOptions.WindowTitle;

            RecentProjectGroups = new ObservableCollection<RecentProjectGroupViewModel>();

            CreateProjectCommand = new RelayCommand(_ => CreateProject());
            OpenProjectCommand = new RelayCommand(_ => OpenProject());
            OpenSelectedProjectCommand = new RelayCommand(_ => OpenSelectedProject(), _ => SelectedRecentProject != null);
            RefreshCommand = new RelayCommand(_ => Refresh());
            ClearSearchCommand = new RelayCommand(_ => ClearSearch());

            Refresh();
        }


        public void OpenRecentProject(RecentProjectViewModel? recentProject)
        {
            if (recentProject == null)
                return;

            if (string.IsNullOrWhiteSpace(recentProject.FilePath) || !File.Exists(recentProject.FilePath))
            {
                _showNotice(
                    "파일을 찾을 수 없음",
                    $"최근 프로젝트 파일을 찾을 수 없습니다.\n\n{recentProject.FilePath}");

                _recentProjectService.Remove(recentProject.FilePath);
                LoadRecentProjects();
                return;
            }

            var result = _projectHomeHandler.OpenProject(recentProject.FilePath);

            if (!result.IsSuccess)
            {
                if (!result.IsCanceled)
                {
                    _showNotice(
                        "프로젝트 열기 실패",
                        string.IsNullOrWhiteSpace(result.ErrorMessage)
                            ? "프로젝트 파일을 열 수 없습니다."
                            : result.ErrorMessage);
                }

                return;
            }

            AddRecentProject(result);
            _projectHomeHandler.ShowMainWindow(result);
        }

        private void CreateProject()
        {
            var result = _projectHomeHandler.CreateProject();

            if (!result.IsSuccess)
            {
                if (!result.IsCanceled && !string.IsNullOrWhiteSpace(result.ErrorMessage))
                    _showNotice("프로젝트 생성 실패", result.ErrorMessage);

                return;
            }

            AddRecentProject(result);
            _projectHomeHandler.ShowMainWindow(result);
        }

        private void OpenProject()
        {
            var result = _projectHomeHandler.OpenProject();

            if (!result.IsSuccess)
            {
                if (!result.IsCanceled && !string.IsNullOrWhiteSpace(result.ErrorMessage))
                    _showNotice("프로젝트 열기 실패", result.ErrorMessage);

                return;
            }

            AddRecentProject(result);
            _projectHomeHandler.ShowMainWindow(result);
        }

        private void OpenSelectedProject()
        {
            if (SelectedRecentProject == null)
            {
                _showNotice("프로젝트 열기", "열 프로젝트를 선택해주세요.");
                return;
            }

            OpenRecentProject(SelectedRecentProject);
        }

        private void Refresh()
        {
            _recentProjectService.RemoveMissingFiles();
            LoadRecentProjects();
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
        }

        private void LoadRecentProjects()
        {
            var recentProjects = _recentProjectService
                .Load()
                .Where(MatchesSearch)
                .OrderByDescending(project => project.LastOpenedAt)
                .Select(project => new RecentProjectViewModel(project))
                .ToList();

            var startOfWeek = GetStartOfWeek(DateTime.Today);

            var thisWeekGroup = new RecentProjectGroupViewModel("이번 주");
            var oldGroup = new RecentProjectGroupViewModel("오래됨");

            foreach (var recentProject in recentProjects)
            {
                if (recentProject.LastOpenedAt.Date >= startOfWeek)
                    thisWeekGroup.Projects.Add(recentProject);
                else
                    oldGroup.Projects.Add(recentProject);
            }

            RecentProjectGroups.Clear();

            if (thisWeekGroup.Projects.Count > 0)
                RecentProjectGroups.Add(thisWeekGroup);

            if (oldGroup.Projects.Count > 0)
                RecentProjectGroups.Add(oldGroup);

            OnPropertyChanged(nameof(HasRecentProjects));
            OnPropertyChanged(nameof(HasNoRecentProjects));
        }

        private bool MatchesSearch(RecentProjectInfo project)
        {
            ArgumentNullException.ThrowIfNull(project);

            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            var keyword = SearchText.Trim();
            var folderPath = Path.GetDirectoryName(project.FilePath);

            return Contains(project.ProjectName, keyword)
                   || Contains(project.FilePath, keyword)
                   || Contains(folderPath, keyword);
        }

        private static bool Contains(string? source, string keyword)
        {
            return !string.IsNullOrWhiteSpace(source)
                   && source.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        private void AddRecentProject(HomeProjectLaunchResult result)
        {
            if (string.IsNullOrWhiteSpace(result.FilePath))
                return;

            _recentProjectService.AddOrUpdate(result.FilePath, result.ProjectName);
        }

        private static DateTime GetStartOfWeek(DateTime date)
        {
            var difference = (7 + date.DayOfWeek - DayOfWeek.Monday) % 7;
            return date.Date.AddDays(-difference);
        }
    }
}