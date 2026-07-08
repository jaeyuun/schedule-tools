using GirderSchedule.App.Commands;
using GirderSchedule.App.Constants;
using GirderSchedule.App.Services;
using GirderSchedule.App.Services.Project;
using GirderSchedule.App.Views.Home;
using GirderSchedule.App.Views.Main;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels.Home
{
    public sealed class HomeStartViewModel : ViewModelBase
    {
        private readonly ProjectFileService _projectFileService = new ProjectFileService();
        private readonly RecentProjectService _recentProjectService = new RecentProjectService();

        private string _searchText = string.Empty;
        private RecentProjectViewModel _selectedRecentProject;

        public ObservableCollection<RecentProjectGroupViewModel> RecentGroups { get; private set; }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText == value)
                {
                    return;
                }

                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                LoadRecentProjects();
            }
        }

        public RecentProjectViewModel SelectedRecentProject
        {
            get { return _selectedRecentProject; }
            set
            {
                if (_selectedRecentProject == value)
                {
                    return;
                }

                ClearRecentProjectSelection();

                _selectedRecentProject = value;

                if (_selectedRecentProject != null)
                {
                    _selectedRecentProject.IsSelected = true;
                }

                OnPropertyChanged(nameof(SelectedRecentProject));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand CreateProjectCommand { get; private set; }
        public ICommand OpenProjectCommand { get; private set; }
        public ICommand OpenRecentProjectCommand { get; private set; }
        public ICommand StartSelectedProjectCommand { get; private set; }
        public ICommand CheckRecentProjectsCommand { get; private set; }

        public HomeStartViewModel()
        {
            RecentGroups = new ObservableCollection<RecentProjectGroupViewModel>();

            CreateProjectCommand = new RelayCommand(p => CreateProject());
            OpenProjectCommand = new RelayCommand(p => OpenProject());
            OpenRecentProjectCommand = new RelayCommand(p => OpenRecentProject(p as RecentProjectViewModel));
            StartSelectedProjectCommand = new RelayCommand(p => StartSelectedProject());
            CheckRecentProjectsCommand = new RelayCommand(p => CheckRecentProjects());

            LoadRecentProjects();
        }

        public void StartSelectedProject()
        {
            if (SelectedRecentProject == null)
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_OpenProjectFailed,
                    Properties.Resources.Content_SelectProjectOpen);
                return;
            }

            OpenRecentProject(SelectedRecentProject);
        }

        private void CreateProject()
        {
            var window = new ProjectCreateWindow();
            window.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            window.ShowDialog();

            LoadRecentProjects();
        }

        private void OpenProject()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = FileConstants.ProjectDialogTitle;
            dialog.Filter = FileConstants.ProjectFilter;
            dialog.DefaultExt = FileConstants.ProjectFilter;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            OpenProjectFile(dialog.FileName);
        }

        private void OpenRecentProject(RecentProjectViewModel recentProject)
        {
            if (recentProject == null)
            {
                return;
            }

            if (!File.Exists(recentProject.FilePath))
            {
                _recentProjectService.Remove(recentProject.FilePath);
                LoadRecentProjects();

                AppDialogService.ShowNotice(
                    Properties.Resources.Title_FileNotFound,
                    string.Format(Properties.Resources.Content_RecentProjectFileMissing, recentProject.FilePath));

                return;
            }

            OpenProjectFile(recentProject.FilePath);
        }

        private void OpenProjectFile(string filePath)
        {
            try
            {
                var project = _projectFileService.Load(filePath);
                _recentProjectService.AddOrUpdate(filePath, project.ProjectName);

                var mainWindow = new MainWindow(project, filePath);
                ShowMainWindowOnly(mainWindow);
            }
            catch
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_OpenProjectFailed, 
                    Properties.Resources.Content_FileOpenFailedWithError);
            }
        }

        private void CheckRecentProjects()
        {
            _recentProjectService.RemoveMissingFiles();
            LoadRecentProjects();
        }

        private void LoadRecentProjects()
        {
            RecentGroups.Clear();

            var items = _recentProjectService.Load();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                items = items
                    .Where(x => ContainsText(x.ProjectName, SearchText) || ContainsText(x.FilePath, SearchText))
                    .ToList();
            }

            var thisWeek = new RecentProjectGroupViewModel("이번 주");
            var old = new RecentProjectGroupViewModel("오래됨");
            var weekStart = GetWeekStart(DateTime.Today);

            for (var i = 0; i < items.Count; i++)
            {
                var viewModel = new RecentProjectViewModel(items[i]);

                if (items[i].LastOpenedAt.Date >= weekStart)
                {
                    thisWeek.Items.Add(viewModel);
                }
                else
                {
                    old.Items.Add(viewModel);
                }
            }

            if (thisWeek.Items.Count > 0)
            {
                RecentGroups.Add(thisWeek);
            }

            if (old.Items.Count > 0)
            {
                RecentGroups.Add(old);
            }
        }

        private void ClearRecentProjectSelection()
        {
            for (var groupIndex = 0; groupIndex < RecentGroups.Count; groupIndex++)
            {
                var group = RecentGroups[groupIndex];

                for (var itemIndex = 0; itemIndex < group.Items.Count; itemIndex++)
                {
                    group.Items[itemIndex].IsSelected = false;
                }
            }
        }

        private bool ContainsText(string source, string keyword)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            return source.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private DateTime GetWeekStart(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        private void ShowMainWindowOnly(MainWindow mainWindow)
        {
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            var windows = Application.Current.Windows.Cast<Window>().Where(x => x != mainWindow).ToList();

            for (var i = 0; i < windows.Count; i++)
            {
                windows[i].Close();
            }
        }
    }
}