using GirderSchedule.App.Services;
using GirderSchedule.App.Services.Ui;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.Domain.Models;
using System.Windows;

namespace GirderSchedule.App.ViewModels.Main
{
    public sealed partial class MainWindowViewModel
    {
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
            OnPropertyChanged(nameof(ProjectDisplayName));
            OnPropertyChanged(nameof(ScheduleTitle));
            OnPropertyChanged(nameof(WindowTitle));
        }

        private void NewProject()
        {
            UiEditCommitService.CommitAll();

            if (!ConfirmSaveIfDirty())
            {
                return;
            }

            var result = _projectService.CreateNewProject();

            if (!result.IsSuccess)
            {
                return;
            }

            CurrentFilePath = result.FilePath;
            LoadProject(result.Project);
            IsDirty = false;
        }

        private void OpenProject()
        {
            UiEditCommitService.CommitAll();

            if (!ConfirmSaveIfDirty())
            {
                return;
            }

            var result = _projectService.OpenProject();

            if (!result.IsSuccess)
            {
                return;
            }

            CurrentFilePath = result.FilePath;
            LoadProject(result.Project);
            IsDirty = false;
        }

        private bool SaveProject()
        {
            UiEditCommitService.CommitAll();

            var result = _projectService.SaveProject(Project, CurrentFilePath);

            if (!result.IsSuccess)
            {
                return false;
            }

            CurrentFilePath = result.FilePath;
            IsDirty = false;
            return true;
        }

        private bool SaveAsProject()
        {
            UiEditCommitService.CommitAll();

            var result = _projectService.SaveProjectAs(Project);

            if (!result.IsSuccess)
            {
                return false;
            }

            CurrentFilePath = result.FilePath;
            IsDirty = false;
            return true;
        }

        public bool ConfirmSaveIfDirty()
        {
            UiEditCommitService.CommitAll();

            if (!IsDirty)
            {
                return true;
            }

            var result = AppDialogService.ShowSaveConfirm(
                Properties.Resources.Title_ProjectSave, 
                Properties.Resources.Content_UnsavedChangesSaveConfirm);

            if (result == MessageBoxResult.Yes)
            {
                return SaveProject();
            }

            if (result == MessageBoxResult.No)
            {
                return true;
            }

            return false;
        }

        private bool SaveProjectBeforeExport()
        {
            if (string.IsNullOrWhiteSpace(CurrentFilePath))
            {
                var result = AppDialogService.ShowConfirm(
                    Properties.Resources.Title_ProjectSave, 
                    Properties.Resources.Content_ProjectSaveRequiredBeforeExportConfirm);

                if (result != MessageBoxResult.Yes)
                {
                    return false;
                }

                return SaveAsProject();
            }

            return SaveProject();
        }
    }
}