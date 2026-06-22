using GirderSchedule.App.Commands;
using GirderSchedule.App.Services.Project;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.Utils;
using GirderSchedule.App.ViewModels.Export;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.App.ViewModels.Settings;
using GirderSchedule.App.Views.Export;
using GirderSchedule.App.Views.Settings;
using GirderSchedule.Domain.Models;
using GirderSchedule.Dxf.Export;
using GirderSchedule.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels.Main
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly ProjectFileService _fileService = new ProjectFileService();
        private readonly RecentProjectService _recentProjectService = new RecentProjectService();
        private readonly ScheduleProjectFactory _projectFactory = new ScheduleProjectFactory();
        private readonly ScheduleSetFactory _setFactory = new ScheduleSetFactory();
        private readonly ScheduleExportPageBuilder _exportPageBuilder = new ScheduleExportPageBuilder();
        private readonly ScheduleExcelProjectBuilder _excelProjectBuilder = new ScheduleExcelProjectBuilder();

        private ScheduleProject _project;
        private FloorNodeViewModel _selectedFloor;
        private ScheduleSetNodeViewModel _selectedSet;
        private string _currentFilePath = string.Empty;
        private bool _isDirty;
        private ScheduleSet _copiedScheduleSet;

        public ObservableCollection<FloorNodeViewModel> Floors { get; private set; }
        public ScheduleViewModel Editor { get; private set; }

        public ScheduleProject Project
        {
            get { return _project; }
        }

        public string ProjectName
        {
            get { return _project == null ? string.Empty : _project.ProjectName; }
            set
            {
                if (_project == null || _project.ProjectName == value)
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
            get { return _project == null ? string.Empty : _project.ScheduleTitle; }
            set
            {
                if (_project == null || _project.ScheduleTitle == value)
                {
                    return;
                }

                _project.ScheduleTitle = value;
                IsDirty = true;
                OnPropertyChanged(nameof(ScheduleTitle));
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set
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
            set
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
                var scheduleTitle = _project == null || string.IsNullOrWhiteSpace(_project.ScheduleTitle) ? "보 일람표" : _project.ScheduleTitle;
                var projectName = _project == null || string.IsNullOrWhiteSpace(_project.ProjectName) ? "새 프로젝트" : _project.ProjectName;
                var dirty = IsDirty || string.IsNullOrWhiteSpace(CurrentFilePath) ? " *" : string.Empty;

                return scheduleTitle + " (" + projectName + ")" + dirty;
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
                    return;
                }

                ClearTreeSelection();
                _selectedFloor.IsSelected = true;
                SelectedSet = null;
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
        public ICommand ExportExcelCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        public ICommand CopyScheduleSetCommand { get; private set; }
        public ICommand PasteScheduleSetCommand { get; private set; }

        public MainWindowViewModel()
        {
            Initialize();
            CurrentFilePath = string.Empty;
            LoadProject(CreateNewProject("새 프로젝트"));
            IsDirty = false;
        }

        public MainWindowViewModel(ScheduleProject project, string filePath)
        {
            Initialize();
            CurrentFilePath = filePath;
            LoadProject(project);
            IsDirty = false;
        }

        public static ScheduleProject CreateNewProject(string projectName)
        {
            return new ScheduleProjectFactory().Create(projectName);
        }

        private void Initialize()
        {
            Editor = new ScheduleViewModel();
            Editor.CurrentSetChanged += Editor_CurrentSetChanged;
            Floors = new ObservableCollection<FloorNodeViewModel>();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            NewProjectCommand = new RelayCommand(p => NewProject());
            OpenProjectCommand = new RelayCommand(p => OpenProject());
            SaveProjectCommand = new RelayCommand(p => SaveProject());
            SaveAsProjectCommand = new RelayCommand(p => SaveAsProject());
            EditFloorSettingCommand = new RelayCommand(p => EditFloorSetting());
            AddSetCommand = new RelayCommand(p => AddSet(), p => SelectedFloor != null);
            RemoveSetCommand = new RelayCommand(p => RemoveSet(), p => SelectedFloor != null && SelectedSet != null);
            ExportCurrentSetCommand = new RelayCommand(p => ExportProjectDxf());
            ExportExcelCommand = new RelayCommand(p => ExportProjectExcel());
            HelpCommand = new RelayCommand(p => ShowHelp());
            CopyScheduleSetCommand = new RelayCommand(p => CopyScheduleSet(), p => CanCopyScheduleSet());
            PasteScheduleSetCommand = new RelayCommand(p => PasteScheduleSet(), p => CanPasteScheduleSet());
        }

        private void LoadProject(ScheduleProject project)
        {
            _project = project;
            Floors.Clear();

            if (_project != null)
            {
                for (var i = 0; i < _project.Floors.Count; i++)
                {
                    Floors.Add(new FloorNodeViewModel(_project.Floors[i]));
                }
            }

            SelectedFloor = Floors.Count > 0 ? Floors[0] : null;
            NotifyProjectChanged();
        }

        private void NotifyProjectChanged()
        {
            OnPropertyChanged(nameof(Project));
            OnPropertyChanged(nameof(ProjectName));
            OnPropertyChanged(nameof(ScheduleTitle));
            OnPropertyChanged(nameof(WindowTitle));
        }

        private void NewProject()
        {
            if (!ConfirmSaveIfDirty())
            {
                return;
            }

            CurrentFilePath = string.Empty;
            LoadProject(_projectFactory.Create("새 프로젝트"));
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
                _recentProjectService.AddOrUpdate(dialog.FileName, project.ProjectName);
                IsDirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("프로젝트 파일을 열 수 없습니다.\r\n\r\n" + ex.Message, "열기 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool SaveProject()
        {
            if (string.IsNullOrWhiteSpace(CurrentFilePath))
            {
                return SaveAsProject();
            }

            try
            {
                _fileService.Save(CurrentFilePath, _project);
                _recentProjectService.AddOrUpdate(CurrentFilePath, _project.ProjectName);
                IsDirty = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("프로젝트를 저장할 수 없습니다.\r\n\r\n" + ex.Message, "저장 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private bool SaveAsProject()
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "프로젝트 저장";
            dialog.Filter = "GirderSchedule 프로젝트 (*.gsp)|*.gsp";
            dialog.DefaultExt = ".gsp";
            dialog.AddExtension = true;
            dialog.FileName = string.IsNullOrWhiteSpace(ProjectName) ? "새 프로젝트.gsp" : ProjectName + ".gsp";

            if (dialog.ShowDialog() != true)
            {
                return false;
            }

            try
            {
                CurrentFilePath = dialog.FileName;
                _fileService.Save(CurrentFilePath, _project);
                _recentProjectService.AddOrUpdate(CurrentFilePath, _project.ProjectName);
                IsDirty = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("프로젝트를 저장할 수 없습니다.\r\n\r\n" + ex.Message, "저장 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool ConfirmSaveIfDirty()
        {
            if (!IsDirty)
            {
                return true;
            }

            var result = MessageBox.Show("저장하지 않은 변경 내용이 있습니다.\r\n저장하시겠습니까?", "프로젝트 저장", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
                return false;
            }

            if (result == MessageBoxResult.Yes)
            {
                return SaveProject();
            }

            return true;
        }

        private void EditFloorSetting()
        {
            var viewModel = new FloorSettingWindowViewModel(Floors);
            viewModel.SelectedFloor = SelectedFloor;
            viewModel.FloorSettingChanged += FloorSettingWindow_FloorSettingChanged;

            var window = new FloorSettingWindow();
            window.Owner = Application.Current.MainWindow;
            window.DataContext = viewModel;
            window.ShowDialog();

            viewModel.FloorSettingChanged -= FloorSettingWindow_FloorSettingChanged;

            SyncProjectFloors();
            EnsureEachFloorHasSet();

            if (Floors.Count > 0 && SelectedFloor == null)
            {
                SelectedFloor = Floors[0];
            }
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

                var set = _setFactory.Create("G1", floor.Setting);
                floor.AddSet(set);
            }
        }

        private void SyncProjectFloors()
        {
            if (_project == null)
            {
                return;
            }

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

            var memberName = SelectedFloor.FloorNumber + "G" + (SelectedFloor.Sets.Count + 1);
            var set = _setFactory.Create(memberName, SelectedFloor.Setting);
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

            var result = MessageBox.Show("선택한 부재를 삭제하시겠습니까?", "부재 삭제", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var index = SelectedFloor.Sets.IndexOf(SelectedSet);
            SelectedFloor.RemoveSet(SelectedSet);

            if (SelectedFloor.Sets.Count == 0)
            {
                SelectedSet = null;
            }
            else
            {
                if (index >= SelectedFloor.Sets.Count)
                {
                    index = SelectedFloor.Sets.Count - 1;
                }

                SelectedSet = SelectedFloor.Sets[index];
            }

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
            MessageBox.Show("GirderSchedule\r\n보 일람표 작성 도구", "도움말", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ExportProjectDxf()
        {
            if (!SaveProjectBeforeExport())
            {
                return;
            }

            var setting = ShowDxfExportSettingWindow();

            if (setting == null)
            {
                return;
            }

            var pages = _exportPageBuilder.Build(ScheduleTitle, Floors, setting);

            if (pages.Count == 0)
            {
                MessageBox.Show("내보낼 층 또는 부재가 선택되지 않았습니다.", "DXF 내보내기", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.Title = "DXF 저장";
            dialog.Filter = "DXF 파일 (*.dxf)|*.dxf";
            dialog.FileName = BuildDxfFileName();
            dialog.DefaultExt = ".dxf";
            dialog.AddExtension = true;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            ExportPages(dialog.FileName, pages, setting);
        }

        private DxfExportSetting ShowDxfExportSettingWindow()
        {
            var viewModel = new DxfExportSettingWindowViewModel();

            var window = new DxfExportSettingWindow();
            window.Owner = Application.Current.MainWindow;
            window.DataContext = viewModel;

            if (window.ShowDialog() != true)
            {
                return null;
            }

            return viewModel.CreateSetting();
        }

        private void ExportPages(string filePath, List<ScheduleExportPage> pages, DxfExportSetting setting)
        {
            try
            {
                var exporter = new DxfExporter();

                var options = new DxfExportOptions();
                options.IncludeLeft = true;
                options.IncludeCenter = true;
                options.IncludeRight = true;
                options.FormColumnCount = setting == null ? 3 : setting.FormColumnCount;
                options.TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "GirderTemplate.dxf");

                exporter.Export(pages, filePath, options);
                MessageBox.Show("DXF 파일을 저장했습니다.", "DXF 저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException)
            {
                MessageBox.Show("DXF 파일을 저장할 수 없습니다.\r\n\r\n저장하려는 파일이 AutoCAD, GstarCAD, 뷰어 또는 다른 프로그램에서 열려 있을 수 있습니다.\r\n파일을 닫은 뒤 다시 저장해 주세요.\r\n\r\n파일 경로: " + filePath, "DXF 저장 실패", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("DXF 파일을 저장할 권한이 없습니다.\r\n\r\n쓰기 권한이 있는 폴더인지 확인하거나 다른 위치에 저장해 주세요.\r\n\r\n파일 경로: " + filePath, "DXF 저장 실패", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("DXF 저장 중 오류가 발생했습니다.\r\n\r\n" + ex.Message, "DXF 저장 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportProjectExcel()
        {
            var exportProject = _excelProjectBuilder.Build(Project, Floors);

            if (exportProject == null || exportProject.Floors.Count == 0)
            {
                MessageBox.Show("내보낼 층 또는 부재가 선택되지 않았습니다.", "Excel 내보내기", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.Title = "Excel 저장";
            dialog.Filter = "Excel 파일 (*.xlsx)|*.xlsx";
            dialog.InitialDirectory = PathUtil.GetDownloadsDirectory();
            dialog.FileName = PathUtil.GetDefaultExcelFileName(exportProject.ProjectName);
            dialog.DefaultExt = ".xlsx";
            dialog.AddExtension = true;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var exporter = new ExcelExporter();

                var options = new ExcelExportOptions();
                options.SheetName = "보 일람표";
                options.DefaultCover = 40;
                options.ContinueAcrossFloors = false;

                exporter.Export(exportProject, dialog.FileName, options);
                MessageBox.Show("Excel 파일을 저장했습니다.", "Excel 저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException)
            {
                MessageBox.Show("Excel 파일을 저장할 수 없습니다.\r\n\r\n저장하려는 파일이 Excel 또는 다른 프로그램에서 열려 있을 수 있습니다.\r\n파일을 닫은 뒤 다시 저장해 주세요.\r\n\r\n파일 경로: " + dialog.FileName, "Excel 저장 실패", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Excel 파일을 저장할 권한이 없습니다.\r\n\r\n쓰기 권한이 있는 폴더인지 확인하거나 다른 위치에 저장해 주세요.\r\n\r\n파일 경로: " + dialog.FileName, "Excel 저장 실패", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel 저장 중 오류가 발생했습니다.\r\n\r\n" + ex.Message, "Excel 저장 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool SaveProjectBeforeExport()
        {
            if (string.IsNullOrWhiteSpace(CurrentFilePath))
            {
                var result = MessageBox.Show("프로젝트 파일이 아직 저장되지 않았습니다.\r\nDXF 내보내기 전에 프로젝트를 먼저 저장해야 합니다.\r\n\r\n프로젝트를 저장하시겠습니까?", "프로젝트 저장 필요", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (result != MessageBoxResult.Yes)
                {
                    return false;
                }

                return SaveAsProject();
            }

            return SaveProject();
        }

        private string BuildDxfFileName()
        {
            var projectName = FileNameUtil.Sanitize(ProjectName);
            var scheduleTitle = FileNameUtil.Sanitize(ScheduleTitle);

            if (string.IsNullOrWhiteSpace(projectName))
            {
                projectName = "프로젝트";
            }

            if (string.IsNullOrWhiteSpace(scheduleTitle))
            {
                scheduleTitle = "보일람표";
            }

            return projectName + "_" + scheduleTitle + ".dxf";
        }

        public void MoveFloor(FloorNodeViewModel source, FloorNodeViewModel target, bool isAfter)
        {
            if (source == null || target == null || ReferenceEquals(source, target))
            {
                return;
            }

            var oldIndex = Floors.IndexOf(source);
            var insertIndex = Floors.IndexOf(target);

            if (oldIndex < 0 || insertIndex < 0)
            {
                return;
            }

            if (isAfter)
            {
                insertIndex++;
            }

            if (insertIndex > oldIndex)
            {
                insertIndex--;
            }

            if (insertIndex < 0)
            {
                insertIndex = 0;
            }

            if (insertIndex >= Floors.Count)
            {
                insertIndex = Floors.Count - 1;
            }

            if (oldIndex == insertIndex)
            {
                SelectedFloor = source;
                return;
            }

            Floors.Move(oldIndex, insertIndex);
            SyncProjectFloors();
            SelectedFloor = source;
            IsDirty = true;
        }

        public void MoveSet(ScheduleSetNodeViewModel source, ScheduleSetNodeViewModel target, bool isAfter)
        {
            if (source == null || target == null || ReferenceEquals(source, target))
            {
                return;
            }

            var sourceFloor = FindFloorBySet(source);
            var targetFloor = FindFloorBySet(target);

            if (sourceFloor == null || targetFloor == null)
            {
                return;
            }

            var insertIndex = targetFloor.Sets.IndexOf(target);

            if (insertIndex < 0)
            {
                return;
            }

            if (isAfter)
            {
                insertIndex++;
            }

            MoveSetToFloorIndex(source, sourceFloor, targetFloor, insertIndex);
        }

        public void MoveSetToFloorAroundFloor(ScheduleSetNodeViewModel source, FloorNodeViewModel targetFloor, bool isAfter)
        {
            if (source == null || targetFloor == null)
            {
                return;
            }

            var sourceFloor = FindFloorBySet(source);

            if (sourceFloor == null)
            {
                return;
            }

            var insertIndex = isAfter ? targetFloor.Sets.Count : 0;
            MoveSetToFloorIndex(source, sourceFloor, targetFloor, insertIndex);
        }

        public void MoveSetToFloor(ScheduleSetNodeViewModel source, FloorNodeViewModel targetFloor)
        {
            if (source == null || targetFloor == null)
            {
                return;
            }

            var sourceFloor = FindFloorBySet(source);

            if (sourceFloor == null)
            {
                return;
            }

            MoveSetToFloorIndex(source, sourceFloor, targetFloor, targetFloor.Sets.Count);
        }

        private void MoveSetToFloorIndex(ScheduleSetNodeViewModel source, FloorNodeViewModel sourceFloor, FloorNodeViewModel targetFloor, int insertIndex)
        {
            if (source == null || sourceFloor == null || targetFloor == null)
            {
                return;
            }

            var oldIndex = sourceFloor.Sets.IndexOf(source);

            if (oldIndex < 0)
            {
                return;
            }

            if (ReferenceEquals(sourceFloor, targetFloor) && insertIndex > oldIndex)
            {
                insertIndex--;
            }

            if (insertIndex < 0)
            {
                insertIndex = 0;
            }

            if (insertIndex > targetFloor.Sets.Count)
            {
                insertIndex = targetFloor.Sets.Count;
            }

            if (ReferenceEquals(sourceFloor, targetFloor) && oldIndex == insertIndex)
            {
                SelectedSet = source;
                return;
            }

            sourceFloor.Sets.Remove(source);
            sourceFloor.Model.Sets.Remove(source.Model);

            if (insertIndex > targetFloor.Sets.Count)
            {
                insertIndex = targetFloor.Sets.Count;
            }

            source.Parent = targetFloor;
            targetFloor.Sets.Insert(insertIndex, source);
            targetFloor.Model.Sets.Insert(insertIndex, source.Model);

            sourceFloor.UpdateCheckedFromChildren();
            targetFloor.UpdateCheckedFromChildren();

            SelectedFloor = targetFloor;
            SelectedSet = source;
            IsDirty = true;
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

        private void ApplyFloorSettingToSets(FloorNodeViewModel floor)
        {
            if (floor == null)
            {
                return;
            }

            for (var i = 0; i < floor.Sets.Count; i++)
            {
                ApplyFloorSettingToSet(floor.Sets[i].Model, floor.Setting);
                floor.Sets[i].RefreshAll();
            }

            floor.RefreshAll();

            if (SelectedSet != null && floor.Sets.Contains(SelectedSet))
            {
                SelectedSet.RefreshAll();
                Editor.LoadSet(SelectedSet.Model);
            }
        }

        private void ApplyFloorSettingToSet(ScheduleSet set, FloorSetting setting)
        {
            if (set == null || setting == null)
            {
                return;
            }

            ApplyFloorSettingToItem(set.Left, setting);
            ApplyFloorSettingToItem(set.Center, setting);
            ApplyFloorSettingToItem(set.Right, setting);
        }

        private void ApplyFloorSettingToItem(ScheduleItem item, FloorSetting setting)
        {
            if (item == null || setting == null)
            {
                return;
            }

            ApplyMainRebarDiameter(item, setting.MainRebarDiameter);
            ApplyStirrupDiameter(item, setting.StirrupDiameter);
            ApplySkinRebarDiameter(item, setting.SkinRebarDiameter);
        }

        private void ApplyMainRebarDiameter(ScheduleItem item, int diameter)
        {
            if (diameter <= 0)
            {
                return;
            }

            if (item.TopRebar != null)
            {
                item.TopRebar.Diameter = diameter;
            }

            if (item.BottomRebar != null)
            {
                item.BottomRebar.Diameter = diameter;
            }
        }

        private void ApplyStirrupDiameter(ScheduleItem item, int diameter)
        {
            if (diameter <= 0)
            {
                return;
            }

            if (item.Stirrup != null)
            {
                item.Stirrup.Diameter = diameter;
            }
        }

        private void ApplySkinRebarDiameter(ScheduleItem item, int diameter)
        {
            if (diameter <= 0)
            {
                return;
            }

            if (item.SkinRebar != null)
            {
                item.SkinRebar.Diameter = diameter;
            }
        }

        private void FloorSettingWindow_FloorSettingChanged(object sender, EventArgs e)
        {
            var viewModel = sender as FloorSettingWindowViewModel;

            if (viewModel == null || viewModel.SelectedFloor == null)
            {
                return;
            }

            SyncProjectFloors();
            EnsureEachFloorHasSet();
            ApplyFloorSettingToSets(viewModel.SelectedFloor);

            if (Floors.Count > 0 && SelectedFloor == null)
            {
                SelectedFloor = Floors[0];
            }

            IsDirty = true;
        }

        private bool CanCopyScheduleSet()
        {
            return SelectedSet != null && SelectedSet.Model != null;
        }

        private bool CanPasteScheduleSet()
        {
            return _copiedScheduleSet != null && SelectedSet != null && SelectedSet.Model != null;
        }

        private void CopyScheduleSet()
        {
            if (SelectedSet == null || SelectedSet.Model == null)
            {
                return;
            }

            _copiedScheduleSet = CloneScheduleSet(SelectedSet.Model);
        }

        private void PasteScheduleSet()
        {
            if (_copiedScheduleSet == null || SelectedSet == null || SelectedSet.Model == null)
            {
                return;
            }

            ApplyScheduleSetSettings(_copiedScheduleSet, SelectedSet.Model);

            SelectedSet.RefreshAll();
            Editor.LoadSet(SelectedSet.Model);

            IsDirty = true;
        }

        private ScheduleSet CloneScheduleSet(ScheduleSet source)
        {
            if (source == null)
            {
                return null;
            }

            var clone = new ScheduleSet();
            clone.MemberName = source.MemberName;
            clone.Left = CloneScheduleItem(source.Left);
            clone.Center = CloneScheduleItem(source.Center);
            clone.Right = CloneScheduleItem(source.Right);

            return clone;
        }

        private void ApplyScheduleSetSettings(ScheduleSet source, ScheduleSet target)
        {
            if (source == null || target == null)
            {
                return;
            }

            CopyScheduleItem(source.Left, target.Left);
            CopyScheduleItem(source.Center, target.Center);
            CopyScheduleItem(source.Right, target.Right);
        }

        private ScheduleItem CloneScheduleItem(ScheduleItem source)
        {
            if (source == null)
            {
                return null;
            }

            var target = new ScheduleItem();
            CopyScheduleItem(source, target);

            return target;
        }

        private void CopyScheduleItem(ScheduleItem source, ScheduleItem target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Name = source.Name;
            target.Position = source.Position;
            target.IsSectionEnabled = source.IsSectionEnabled;
            target.Note = source.Note;

            CopySection(source.Section, target.Section);
            CopyRebarSet(source.TopRebar, target.TopRebar);
            CopyRebarSet(source.BottomRebar, target.BottomRebar);
            CopyStirrup(source.Stirrup, target.Stirrup);
            CopyMemberForce(source.MemberForce, target.MemberForce);
            CopySkinRebar(source.SkinRebar, target.SkinRebar);
        }

        private void CopySection(SectionData source, SectionData target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Width = source.Width;
            target.Height = source.Height;
        }

        private void CopyRebarSet(RebarSet source, RebarSet target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Diameter = source.Diameter;
            CopyRebarLayer(source.FirstLayer, target.FirstLayer);
            CopyRebarLayer(source.SecondLayer, target.SecondLayer);
        }

        private void CopyRebarLayer(RebarLayer source, RebarLayer target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Count = source.Count;
        }

        private void CopyStirrup(StirrupData source, StirrupData target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Legs = source.Legs;
            target.Diameter = source.Diameter;
            target.Spacing = source.Spacing;
            target.ExtraText = source.ExtraText;
        }

        private void CopyMemberForce(MemberForceData source, MemberForceData target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Moment = source.Moment;
            target.Shear = source.Shear;
        }

        private void CopySkinRebar(SkinRebarData source, SkinRebarData target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.Diameter = source.Diameter;
            target.Spacing = source.Spacing;
            target.ExtraText = source.ExtraText;
        }
    }
}
