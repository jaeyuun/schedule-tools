using GirderSchedule.App.Constants;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.App.ViewModels.Settings;
using GirderSchedule.App.Views.Settings;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Export;
using GirderSchedule.Dxf.Export;
using GirderSchedule.Excel;
using Microsoft.Win32;
using ScheduleTools.Core.IO;
using ScheduleTools.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace GirderSchedule.App.Services.Export
{
    public sealed class ScheduleExportService
    {
        private readonly ScheduleExportPageBuilder _exportPageBuilder = new ScheduleExportPageBuilder();
        private readonly ScheduleExcelProjectBuilder _excelProjectBuilder = new ScheduleExcelProjectBuilder();
        private readonly DxfSettingService _dxfSettingService = new DxfSettingService();

        public List<ScheduleExportPage> BuildExportPages(string scheduleTitle, ObservableCollection<FloorNodeViewModel> floors, DxfExportSetting setting)
        {
            if (setting == null)
            {
                setting = new DxfExportSetting();
            }

            return _exportPageBuilder.Build(scheduleTitle, floors, setting);
        }

        public void ExportDxf(ScheduleProject project, ObservableCollection<FloorNodeViewModel> floors)
        {
            var projectName = project == null ? string.Empty : project.ProjectName;
            var scheduleTitle = project == null ? string.Empty : project.ScheduleTitle;
            var setting = ShowDxfExportSettingWindow();

            if (setting == null)
            {
                return;
            }

            var pages = BuildExportPages(scheduleTitle, floors, setting);

            if (pages.Count == 0)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    Properties.Resources.Content_NoExportTargetSelected);
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.Title = FileConstants.DxfSaveDialogTitle;
            dialog.Filter = FileConstants.DxfFilter;
            dialog.FileName = BuildDxfFileName(projectName, scheduleTitle);
            dialog.DefaultExt = FileConstants.DxfExtension;
            dialog.AddExtension = true;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            ExportPages(dialog.FileName, pages, setting, project);
        }

        public void ExportExcel(ScheduleProject project, ObservableCollection<FloorNodeViewModel> floors)
        {
            var exportProject = _excelProjectBuilder.Build(project, floors);

            if (exportProject == null || exportProject.Floors.Count == 0)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    Properties.Resources.Content_NoExportTargetSelected);
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.Title = FileConstants.ExcelSaveDialogTitle;
            dialog.Filter = FileConstants.ExcelFilter;
            dialog.InitialDirectory = PathHelper.GetDefaultProjectDirectory(FileConstants.ApplicationFolderName, FileConstants.ProjectFolderName);
            dialog.FileName = BuildExcelFileName(exportProject.ProjectName, exportProject.ScheduleTitle);
            dialog.DefaultExt = FileConstants.ExcelExtension;
            dialog.AddExtension = true;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            ExportExcelSheet(exportProject, dialog.FileName);
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

        private void ExportPages(string filePath, List<ScheduleExportPage> pages, DxfExportSetting setting, ScheduleProject project)
        {
            try
            {
                var settingName = project == null ? FileConstants.DxfSettingName : project.DxfSettingName;
                var dxfProfile = _dxfSettingService.LoadSetting(settingName);
                var templatePath = _dxfSettingService.GetTemplatePath(dxfProfile.TemplateName);

                if (!File.Exists(templatePath))
                {
                    DialogService.ShowNotice(
                        Properties.Resources.Title_ExportFailed,
                        Properties.Resources.Content_DxfTemplateFileNotFound);
                    return;
                }

                var options = DxfExportOptionFactory.Create(dxfProfile, templatePath, setting.FormColumnCount);
                var exporter = new DxfExporter();

                exporter.Export(pages, filePath, options);

                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportCompleted,
                    Properties.Resources.Content_ExportCompleted);
            }
            catch (UnauthorizedAccessException)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    string.Format(Properties.Resources.Content_FileSaveFailedNoPermission, filePath));
            }
            catch (IOException)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    string.Format(Properties.Resources.Content_FileSaveFailedFileOpened, filePath));
            }
            catch (Exception ex) when (ex.InnerException is IOException)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    string.Format(Properties.Resources.Content_FileSaveFailedFileOpened, filePath));
            }
            catch (Exception ex) when (ex.InnerException is UnauthorizedAccessException)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    string.Format(Properties.Resources.Content_FileSaveFailedNoPermission, filePath));
            }
            catch
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    Properties.Resources.Content_FileSaveFailedWithError);
            }
        }

        private void ExportExcelSheet(ScheduleProject exportProject, string filePath)
        {
            try
            {
                var exporter = new ExcelExporter();

                var options = new ExcelExportOptions();
                options.SheetName = FileConstants.ExcelSheetName;

                exporter.Export(exportProject, filePath, options);

                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportCompleted,
                    Properties.Resources.Content_ExportCompleted);
            }
            catch (IOException)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    string.Format(Properties.Resources.Content_FileSaveFailedFileOpened, filePath));
            }
            catch (UnauthorizedAccessException)
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    string.Format(Properties.Resources.Content_FileSaveFailedNoPermission, filePath));
            }
            catch
            {
                DialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    Properties.Resources.Content_FileSaveFailedWithError);
            }
        }

        private string BuildDxfFileName(string projectName, string scheduleTitle)
        {
            projectName = FileNameHelper.Sanitize(projectName);
            scheduleTitle = FileNameHelper.Sanitize(scheduleTitle);

            if (string.IsNullOrWhiteSpace(projectName))
            {
                projectName = FileConstants.DefaultProjectNameForExport;
            }

            if (string.IsNullOrWhiteSpace(scheduleTitle))
            {
                scheduleTitle = FileConstants.DefaultScheduleTitleForExport;
            }

            return projectName + "_" + scheduleTitle + FileConstants.DxfExtension;
        }

        private string BuildExcelFileName(string projectName, string scheduleTitle)
        {
            projectName = FileNameHelper.Sanitize(projectName);
            scheduleTitle = FileNameHelper.Sanitize(scheduleTitle);

            if (string.IsNullOrWhiteSpace(projectName))
            {
                projectName = FileConstants.DefaultScheduleTitleForExport;
            }

            if (string.IsNullOrWhiteSpace(scheduleTitle))
            {
                scheduleTitle = FileConstants.DefaultScheduleTitleForExport;
            }

            return projectName + "_" + scheduleTitle + FileConstants.ExcelExtension;
        }
    }
}