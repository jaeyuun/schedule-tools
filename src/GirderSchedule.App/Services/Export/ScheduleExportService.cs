using GirderSchedule.App.Constants;
using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.Utils;
using GirderSchedule.App.ViewModels.Settings;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.App.Views.Settings;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Export;
using GirderSchedule.Domain.Models.Settings;
using GirderSchedule.Dxf.Export;
using GirderSchedule.Dxf.Styles;
using GirderSchedule.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
                AppDialogService.ShowNotice(
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
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    Properties.Resources.Content_NoExportTargetSelected);
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.Title = FileConstants.ExcelSaveDialogTitle;
            dialog.Filter = FileConstants.ExcelFilter;
            dialog.InitialDirectory = PathUtil.GetDownloadsDirectory();
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
                var settingName = project == null ? _dxfSettingService.DefaultSetting : project.DxfSettingName;
                var dxfProfile = _dxfSettingService.LoadSetting(settingName);
                var templatePath = _dxfSettingService.GetTemplatePath(dxfProfile.TemplateName);

                if (!File.Exists(templatePath))
                {
                    AppDialogService.ShowNotice(
                        Properties.Resources.Title_ExportFailed,
                        Properties.Resources.Content_DxfTemplateFileNotFound);
                    return;
                }

                var options = CreateDxfExportOptions(setting, dxfProfile, templatePath);
                var exporter = new DxfExporter();

                exporter.Export(pages, filePath, options);

                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ExportCompleted,
                    Properties.Resources.Content_ExportCompleted);
            }
            catch (IOException)
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    string.Format(Properties.Resources.Content_FileSaveFailedFileOpened, filePath));
            }
            catch (UnauthorizedAccessException)
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    string.Format(Properties.Resources.Content_FileSaveFailedNoPermission, filePath));
            }
            catch
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    Properties.Resources.Content_FileSaveFailedWithError);
            }
        }

        private DxfExportOptions CreateDxfExportOptions(DxfExportSetting setting, DxfSettingProfile dxfProfile, string templatePath)
        {
            var options = new DxfExportOptions();
            options.IncludeLeft = true;
            options.IncludeCenter = true;
            options.IncludeRight = true;
            options.FormColumnCount = setting == null ? 3 : setting.FormColumnCount;
            options.TemplatePath = templatePath;
            options.LayerNames = CreateLayerNameSet(dxfProfile == null ? null : dxfProfile.LayerSettings);
            options.StyleNames = CreateStyleNameSet(dxfProfile == null ? null : dxfProfile.StyleSetting);

            return options;
        }

        private DxfLayerNameSet CreateLayerNameSet(List<DxfLayerSetting> layerSettings)
        {
            var layerNames = new DxfLayerNameSet();

            if (layerSettings == null)
            {
                return layerNames;
            }

            layerNames.FormLine = GetLayerName(layerSettings, DxfLayerRole.FormLine, layerNames.FormLine);
            layerNames.FormText = GetLayerName(layerSettings, DxfLayerRole.FormText, layerNames.FormText);
            layerNames.MemberForce = GetLayerName(layerSettings, DxfLayerRole.MemberForce, layerNames.MemberForce);
            layerNames.Girder = GetLayerName(layerSettings, DxfLayerRole.Girder, layerNames.Girder);
            layerNames.Rebar = GetLayerName(layerSettings, DxfLayerRole.Rebar, layerNames.Rebar);
            layerNames.Stirrup = GetLayerName(layerSettings, DxfLayerRole.Stirrup, layerNames.Stirrup);
            layerNames.Text = GetLayerName(layerSettings, DxfLayerRole.Text, layerNames.Text);

            return layerNames;
        }

        private DxfStyleNameSet CreateStyleNameSet(DxfStyleSetting styleSetting)
        {
            var styleNames = new DxfStyleNameSet();

            if (styleSetting == null)
            {
                return styleNames;
            }

            if (!string.IsNullOrWhiteSpace(styleSetting.TextStyleName))
            {
                styleNames.TextStyleName = styleSetting.TextStyleName;
            }

            if (!string.IsNullOrWhiteSpace(styleSetting.DrawingBlockName))
            {
                styleNames.DrawingBlockName = styleSetting.DrawingBlockName;
            }

            if (!string.IsNullOrWhiteSpace(styleSetting.DimensionStyleName))
            {
                styleNames.DimensionStyleName = styleSetting.DimensionStyleName;
            }

            return styleNames;
        }

        private string GetLayerName(List<DxfLayerSetting> layerSettings, DxfLayerRole role, string fallback)
        {
            var layer = layerSettings.FirstOrDefault(x => x.Role == role);

            if (layer == null || string.IsNullOrWhiteSpace(layer.LayerName))
            {
                return fallback;
            }

            return layer.LayerName;
        }

        private void ExportExcelSheet(ScheduleProject exportProject, string filePath)
        {
            try
            {
                var exporter = new ExcelExporter();

                var options = new ExcelExportOptions();
                options.SheetName = FileConstants.ExcelSheetName;

                exporter.Export(exportProject, filePath, options);

                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ExportCompleted,
                    Properties.Resources.Content_ExportCompleted);
            }
            catch (IOException)
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    string.Format(Properties.Resources.Content_FileSaveFailedFileOpened, filePath));
            }
            catch (UnauthorizedAccessException)
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    string.Format(Properties.Resources.Content_FileSaveFailedNoPermission, filePath));
            }
            catch
            {
                AppDialogService.ShowNotice(
                    Properties.Resources.Title_ExportFailed,
                    Properties.Resources.Content_FileSaveFailedWithError);
            }
        }

        private string BuildDxfFileName(string projectName, string scheduleTitle)
        {
            projectName = FileNameUtil.Sanitize(projectName);
            scheduleTitle = FileNameUtil.Sanitize(scheduleTitle);

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
            projectName = FileNameUtil.Sanitize(projectName);
            scheduleTitle = FileNameUtil.Sanitize(scheduleTitle);

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