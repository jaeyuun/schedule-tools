using GirderSchedule.App.Commands;
using GirderSchedule.App.Services;
using GirderSchedule.App.Views;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Services;
using GirderSchedule.Domain.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly ProjectFileService _fileService = new ProjectFileService();
        private ScheduleProject _project;
        private FloorNodeViewModel _selectedFloor;
        private ScheduleSetNodeViewModel _selectedSet;
        private string _currentFilePath = string.Empty;
        private bool _isDirty;

        public ObservableCollection<FloorNodeViewModel> Floors { get; private set; }
        public ScheduleViewModel Editor { get; private set; }

        public ScheduleProject Project
        {
            get { return _project; }
        }

        public string ProjectName
        {
            get { return _project.ProjectName; }
            set
            {
                if (_project.ProjectName == value)
                {
                    return;
                }

                _project.ProjectName = value;
                IsDirty = true;
                OnPropertyChanged(nameof(ProjectName));
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public string ScheduleTitle
        {
            get { return _project.ScheduleTitle; }
            set
            {
                if (_project.ScheduleTitle == value)
                {
                    return;
                }

                _project.ScheduleTitle = value;
                IsDirty = true;
                OnPropertyChanged(nameof(ScheduleTitle));
            }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            private set
            {
                if (_currentFilePath == value)
                {
                    return;
                }

                _currentFilePath = value;
                OnPropertyChanged(nameof(CurrentFilePath));
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {
                if (_isDirty == value)
                {
                    return;
                }

                _isDirty = value;
                OnPropertyChanged(nameof(IsDirty));
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public string WindowTitle
        {
            get
            {
                var name = string.IsNullOrWhiteSpace(ProjectName) ? "새 프로젝트" : ProjectName;
                var dirty = IsDirty ? " *" : string.Empty;
                return name + dirty + " - 보 일람표 툴";
            }
        }

        public FloorNodeViewModel SelectedFloor
        {
            get { return _selectedFloor; }
            set
            {
                if (_selectedFloor == value)
                {
                    return;
                }

                _selectedFloor = value;
                OnPropertyChanged(nameof(SelectedFloor));

                if (_selectedFloor == null)
                {
                    SelectedSet = null;
                    return;
                }

                if (_selectedFloor.Sets.Count > 0)
                {
                    SelectedSet = _selectedFloor.Sets[0];
                }
                else
                {
                    ClearTreeSelection();
                    _selectedFloor.IsSelected = true;
                    SelectedSet = null;
                }
            }
        }

        public ScheduleSetNodeViewModel SelectedSet
        {
            get { return _selectedSet; }
            set
            {
                if (_selectedSet == value)
                {
                    return;
                }

                ClearTreeSelection();

                _selectedSet = value;
                OnPropertyChanged(nameof(SelectedSet));

                if (_selectedSet == null)
                {
                    return;
                }

                var parentFloor = FindFloorBySet(_selectedSet);

                if (parentFloor != null)
                {
                    _selectedFloor = parentFloor;
                    OnPropertyChanged(nameof(SelectedFloor));

                    parentFloor.IsChildSelected = true;
                }

                _selectedSet.IsSelected = true;
                Editor.LoadSet(_selectedSet.Model);
            }
        }

        public ICommand NewProjectCommand { get; private set; }
        public ICommand OpenProjectCommand { get; private set; }
        public ICommand SaveProjectCommand { get; private set; }
        public ICommand SaveAsProjectCommand { get; private set; }
        public ICommand EditFloorSettingCommand { get; private set; }
        public ICommand AddSetCommand { get; private set; }
        public ICommand RemoveSetCommand { get; private set; }
        public ICommand ExportCurrentSetCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }

        public MainWindowViewModel()
        {
            Editor = new ScheduleViewModel();
            Editor.CurrentSetChanged += Editor_CurrentSetChanged;
            Floors = new ObservableCollection<FloorNodeViewModel>();

            NewProjectCommand = new RelayCommand(p => NewProject());
            OpenProjectCommand = new RelayCommand(p => OpenProject());
            SaveProjectCommand = new RelayCommand(p => SaveProject());
            SaveAsProjectCommand = new RelayCommand(p => SaveAsProject());
            EditFloorSettingCommand = new RelayCommand(p => EditFloorSetting());
            AddSetCommand = new RelayCommand(p => AddSet(), p => SelectedFloor != null);
            RemoveSetCommand = new RelayCommand(p => RemoveSet(), p => SelectedFloor != null && SelectedSet != null);
            ExportCurrentSetCommand = new RelayCommand(p => Editor.ExportDxfCommand.Execute(null));
            HelpCommand = new RelayCommand(p => ShowHelp());

            LoadProject(CreateNewProject());
            IsDirty = false;
        }

        private ScheduleProject CreateNewProject()
        {
            var project = new ScheduleProject();
            project.ProjectName = "새 프로젝트";
            project.ScheduleTitle = "보 일람표";

            var floor = new ScheduleFloor();
            floor.Name = "지상 1층";
            floor.Setting.FloorName = floor.Name;
            floor.Setting.MainRebarDiameter = 19;
            floor.Setting.StirrupDiameter = 10;
            floor.Setting.SkinRebarDiameter = 10;

            floor.Sets.Add(CreateScheduleSet("G1", floor.Setting));

            project.Floors.Add(floor);
            return project;
        }

        private ScheduleSet CreateScheduleSet(string memberName, FloorSetting setting)
        {
            var service = new ScheduleBuildService();
            var sheet = service.CreateSample();
            var set = sheet.Rows[0].Sets[0];

            set.MemberName = memberName;

            set.Left.Name = memberName;
            set.Center.Name = memberName;
            set.Right.Name = memberName;

            ApplySetting(set.Left, setting);
            ApplySetting(set.Center, setting);
            ApplySetting(set.Right, setting);

            return set;
        }

        private void ApplySetting(ScheduleItem item, FloorSetting setting)
        {
            if (setting == null)
            {
                return;
            }

            if (setting.MainRebarDiameter > 0)
            {
                item.TopRebar.Diameter = setting.MainRebarDiameter;
                item.BottomRebar.Diameter = setting.MainRebarDiameter;
            }

            if (setting.StirrupDiameter > 0)
            {
                item.Stirrup.Diameter = setting.StirrupDiameter;
            }

            if (setting.SkinRebarDiameter > 0)
            {
                var spacing = string.Empty;

                if (!string.IsNullOrWhiteSpace(item.SkinRebarText) && item.SkinRebarText.Contains("@"))
                {
                    spacing = item.SkinRebarText.Substring(item.SkinRebarText.IndexOf("@") + 1).Trim();
                }

                item.SkinRebarText = string.IsNullOrWhiteSpace(spacing) ? "HD" + setting.SkinRebarDiameter : "HD" + setting.SkinRebarDiameter + "@" + spacing;
            }
        }

        private void LoadProject(ScheduleProject project)
        {
            _project = project;
            Floors.Clear();

            for (var i = 0; i < _project.Floors.Count; i++)
            {
                Floors.Add(new FloorNodeViewModel(_project.Floors[i]));
            }

            OnPropertyChanged(nameof(Project));
            OnPropertyChanged(nameof(ProjectName));
            OnPropertyChanged(nameof(ScheduleTitle));
            OnPropertyChanged(nameof(Floors));
            OnPropertyChanged(nameof(WindowTitle));

            if (Floors.Count > 0)
            {
                SelectedFloor = Floors[0];

                if (Floors[0].Sets.Count > 0)
                {
                    SelectedSet = Floors[0].Sets[0];
                }
            }
        }

        private void NewProject()
        {
            if (!ConfirmSaveIfDirty())
            {
                return;
            }

            CurrentFilePath = string.Empty;
            LoadProject(CreateNewProject());
            IsDirty = false;
        }

        private void OpenProject()
        {
            if (!ConfirmSaveIfDirty())
            {
                return;
            }

            var dialog = new OpenFileDialog();
            dialog.Title = "프로젝트 열기";
            dialog.Filter = "GirderSchedule 프로젝트 (*.gsp)|*.gsp";
            dialog.DefaultExt = ".gsp";

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var project = _fileService.Load(dialog.FileName);
                CurrentFilePath = dialog.FileName;
                LoadProject(project);
                IsDirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("프로젝트 파일을 열 수 없습니다.\r\n\r\n" + ex.Message, "열기 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveProject()
        {
            if (string.IsNullOrWhiteSpace(CurrentFilePath))
            {
                SaveAsProject();
                return;
            }

            try
            {
                _fileService.Save(CurrentFilePath, _project);
                IsDirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("프로젝트를 저장할 수 없습니다.\r\n\r\n" + ex.Message, "저장 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAsProject()
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "프로젝트 저장";
            dialog.Filter = "GirderSchedule 프로젝트 (*.gsp)|*.gsp";
            dialog.DefaultExt = ".gsp";
            dialog.AddExtension = true;
            dialog.FileName = string.IsNullOrWhiteSpace(ProjectName) ? "새 프로젝트.gsp" : ProjectName + ".gsp";

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            CurrentFilePath = dialog.FileName;
            SaveProject();
        }

        public bool ConfirmSaveIfDirty()
        {
            if (!IsDirty)
            {
                return true;
            }

            var result = MessageBox.Show(
                "저장하지 않은 변경 내용이 있습니다.\r\n저장하시겠습니까?",
                "프로젝트 저장",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
                return false;
            }

            if (result == MessageBoxResult.Yes)
            {
                SaveProject();
                return !IsDirty;
            }

            return true;
        }

        private void EditFloorSetting()
        {
            var viewModel = new FloorSettingWindowViewModel(Floors);
            viewModel.SelectedFloor = SelectedFloor;

            var window = new FloorSettingWindow();
            window.Owner = Application.Current.MainWindow;
            window.DataContext = viewModel;
            window.ShowDialog();

            SyncProjectFloors();
            EnsureEachFloorHasSet();
            SyncProjectFloors();

            for (var i = 0; i < Floors.Count; i++)
            {
                Floors[i].RefreshAll();
            }

            if (Floors.Count == 0)
            {
                SelectedFloor = null;
                SelectedSet = null;
                IsDirty = true;
                return;
            }

            if (SelectedFloor == null || !Floors.Contains(SelectedFloor))
            {
                SelectedFloor = Floors[0];
            }

            if (SelectedFloor != null && SelectedFloor.Sets.Count > 0)
            {
                if (SelectedSet == null || !SelectedFloor.Sets.Contains(SelectedSet))
                {
                    SelectedSet = SelectedFloor.Sets[0];
                }
            }
            else
            {
                SelectedSet = null;
            }

            IsDirty = true;
        }

        private void EnsureEachFloorHasSet()
        {
            for (var i = 0; i < Floors.Count; i++)
            {
                var floor = Floors[i];

                if (floor.Sets.Count > 0)
                {
                    continue;
                }

                var set = CreateScheduleSet("G1", floor.Setting);
                floor.AddSet(set);
            }
        }

        private void SyncProjectFloors()
        {
            _project.Floors.Clear();

            for (var i = 0; i < Floors.Count; i++)
            {
                _project.Floors.Add(Floors[i].Model);
            }
        }

        private void AddSet()
        {
            if (SelectedFloor == null)
            {
                return;
            }

            var memberName = "G" + (SelectedFloor.Sets.Count + 1);
            var set = CreateScheduleSet(memberName, SelectedFloor.Setting);
            var node = SelectedFloor.AddSet(set);

            SelectedSet = node;
            IsDirty = true;
        }

        private void RemoveSet()
        {
            if (SelectedFloor == null || SelectedSet == null)
            {
                return;
            }

            var result = MessageBox.Show(
                "선택한 부재를 삭제하시겠습니까?",
                "부재 삭제",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var index = SelectedFloor.Sets.IndexOf(SelectedSet);
            SelectedFloor.RemoveSet(SelectedSet);

            if (SelectedFloor.Sets.Count == 0)
            {
                SelectedSet = null;
                IsDirty = true;
                return;
            }

            if (index >= SelectedFloor.Sets.Count)
            {
                index = SelectedFloor.Sets.Count - 1;
            }

            SelectedSet = SelectedFloor.Sets[index];
            IsDirty = true;
        }

        private void Editor_CurrentSetChanged(object sender, EventArgs e)
        {
            if (SelectedSet != null)
            {
                SelectedSet.RefreshAll();
            }

            IsDirty = true;
        }

        private void ShowHelp()
        {
            MessageBox.Show(
                "1. 층을 추가합니다.\r\n" +
                "2. 층별 철근 직경을 설정합니다.\r\n" +
                "3. 층을 선택한 뒤 부재를 추가합니다.\r\n" +
                "4. 부재를 선택하면 왼쪽 표에서 편집합니다.\r\n" +
                "5. 프로젝트는 .gsp 파일로 저장합니다.",
                "사용방법",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ClearTreeSelection()
        {
            for (var i = 0; i < Floors.Count; i++)
            {
                Floors[i].IsSelected = false;
                Floors[i].IsChildSelected = false;

                for (var j = 0; j < Floors[i].Sets.Count; j++)
                {
                    Floors[i].Sets[j].IsSelected = false;
                }
            }
        }

        private FloorNodeViewModel FindFloorBySet(ScheduleSetNodeViewModel set)
        {
            if (set == null)
            {
                return null;
            }

            for (var i = 0; i < Floors.Count; i++)
            {
                if (Floors[i].Sets.Contains(set))
                {
                    return Floors[i];
                }
            }

            return null;
        }
    }
}