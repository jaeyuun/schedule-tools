using GirderSchedule.App.Commands;
using GirderSchedule.App.Models;
using GirderSchedule.App.Services;
using GirderSchedule.App.Views;
using GirderSchedule.App.Views.Start;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels.Start
{
    public sealed class StartWindowViewModel : ViewModelBase
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

                _selectedRecentProject = value;
                OnPropertyChanged(nameof(SelectedRecentProject));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand CreateProjectCommand { get; private set; }
        public ICommand OpenProjectCommand { get; private set; }
        public ICommand OpenRecentProjectCommand { get; private set; }
        public ICommand StartSelectedProjectCommand { get; private set; }
        public ICommand CheckRecentProjectsCommand { get; private set; }

        public StartWindowViewModel()
        {
            RecentGroups = new ObservableCollection<RecentProjectGroupViewModel>();

            CreateProjectCommand = new RelayCommand(p => CreateProject());
            OpenProjectCommand = new RelayCommand(p => OpenProject());
            OpenRecentProjectCommand = new RelayCommand(p => OpenRecentProject(p as RecentProjectViewModel));
            StartSelectedProjectCommand = new RelayCommand(p => StartSelectedProject());
            CheckRecentProjectsCommand = new RelayCommand(p => CheckRecentProjects());

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

        private void CreateProject()
        {
            var window = new ProjectSettingWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();

            LoadRecentProjects();
        }

        public void StartSelectedProject()
        {
            if (SelectedRecentProject == null)
            {
                MessageBox.Show("시작할 프로젝트를 선택하세요.", "기존 프로젝트 시작", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            OpenRecentProject(SelectedRecentProject);
        }

        private void OpenProject()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "프로젝트 열기";
            dialog.Filter = "GirderSchedule 프로젝트 (*.gsp)|*.gsp";
            dialog.DefaultExt = ".gsp";

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

                MessageBox.Show(
                    "프로젝트 파일이 존재하지 않습니다.\r\n최근 목록에서 제거했습니다.\r\n\r\n" + recentProject.FilePath,
                    "파일 없음",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

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
            catch (Exception ex)
            {
                MessageBox.Show(
                    "프로젝트 파일을 열 수 없습니다.\r\n\r\n" + ex.Message,
                    "열기 실패",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CheckRecentProjects()
        {
            var before = _recentProjectService.Load().Count;
            _recentProjectService.RemoveMissingFiles();
            var after = _recentProjectService.Load().Count;

            LoadRecentProjects();

            var removed = before - after;

            if (removed <= 0)
            {
                MessageBox.Show("최근 프로젝트 목록을 검사했습니다.\r\n삭제된 항목은 없습니다.", "최근 항목 검사", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBox.Show("최근 프로젝트 목록을 검사했습니다.\r\n존재하지 않는 파일 " + removed + "개를 목록에서 제거했습니다.", "최근 항목 검사", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void CloseCurrentWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);

            if (window != null)
            {
                window.Close();
            }
        }
    }
}