using GirderSchedule.App.Services.Export;
using GirderSchedule.App.Services.Project;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels.Main
{
    public sealed partial class MainWindowViewModel : ViewModelBase
    {
        private readonly ScheduleSetFactory _setFactory = new ScheduleSetFactory();
        private readonly ScheduleExportService _exportService = new ScheduleExportService();
        private readonly ScheduleTreeMoveService _scheduleTreeMoveService = new ScheduleTreeMoveService();
        private readonly ProjectDisplayNameService _projectDisplayNameService = new ProjectDisplayNameService();

        private ProjectService _projectService;
        private FloorSettingEditService _floorSettingEditService;
        private ScheduleSetEditService _scheduleSetEditService;

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
                OnPropertyChanged(nameof(ProjectDisplayName));
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
                OnPropertyChanged(nameof(ProjectDisplayName));
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
                OnPropertyChanged(nameof(ProjectDisplayName));
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public string WindowTitle
        {
            get { return _projectDisplayNameService.GetWindowTitle(Project, IsDirty, CurrentFilePath); }
        }

        public string ProjectDisplayName
        {
            get { return _projectDisplayNameService.GetProjectDisplayName(Project, IsDirty); }
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

                if (parentFloor != null)
                {
                    Editor.LoadSet(parentFloor.Model, _selectedSet.Model);
                }
            }
        }

        public ICommand NewProjectCommand { get; private set; }
        public ICommand OpenProjectCommand { get; private set; }
        public ICommand SaveProjectCommand { get; private set; }
        public ICommand SaveAsProjectCommand { get; private set; }
        public ICommand EditFloorSettingCommand { get; private set; }
        public ICommand EditDxfLayerSettingCommand { get; private set; }
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
            _projectService = new ProjectService(new ProjectFileService(), new RecentProjectService());
            _floorSettingEditService = new FloorSettingEditService(_setFactory, new FloorSettingApplyService());
            _scheduleSetEditService = new ScheduleSetEditService(_setFactory, new ScheduleSetCopyService());

            Editor = new ScheduleViewModel();
            Editor.CurrentSetChanged += Editor_CurrentSetChanged;
            Floors = new ObservableCollection<FloorNodeViewModel>();

            InitializeCommands();
        }
    }
}