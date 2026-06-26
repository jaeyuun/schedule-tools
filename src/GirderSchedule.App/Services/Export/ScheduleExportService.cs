using GirderSchedule.App.Services.Schedule;
using GirderSchedule.App.Utils;
using GirderSchedule.App.ViewModels.Export;
using GirderSchedule.App.ViewModels.Schedule;
using GirderSchedule.App.Views.Export;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Export;
using GirderSchedule.Dxf.Export;
using GirderSchedule.Excel;
using Microsoft.Win32;
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

        public List<ScheduleExportPage> BuildExportPages(string scheduleTitle, ObservableCollection<FloorNodeViewModel> floors, DxfExportSetting setting)
        {
            if (setting == null)
            {
                setting = new DxfExportSetting();
            }

            return _exportPageBuilder.Build(scheduleTitle, floors, setting);
        }

        public void ExportDxf(string projectName, string scheduleTitle, ObservableCollection<FloorNodeViewModel> floors)
        {
            var setting = ShowDxfExportSettingWindow();

            if (setting == null)
            {
                return;
            }

            var pages = BuildExportPages(scheduleTitle, floors, setting);

            if (pages.Count == 0)
            {
                MessageBox.Show("내보낼 층 또는 부재가 선택되지 않았습니다.", "DXF 내보내기", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.Title = "DXF 저장";
            dialog.Filter = "DXF 파일 (*.dxf)|*.dxf";
            dialog.FileName = BuildDxfFileName(projectName, scheduleTitle);
            dialog.DefaultExt = ".dxf";
            dialog.AddExtension = true;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            ExportPages(dialog.FileName, pages, setting);
        }

        public void ExportExcel(ScheduleProject project, ObservableCollection<FloorNodeViewModel> floors)
        {
            var exportProject = _excelProjectBuilder.Build(project, floors);

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

            ExportExcelFile(exportProject, dialog.FileName);
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

        private void ExportExcelFile(ScheduleProject exportProject, string filePath)
        {
            try
            {
                var exporter = new ExcelExporter();

                var options = new ExcelExportOptions();
                options.SheetName = "보 일람표";

                exporter.Export(exportProject, filePath, options);
                MessageBox.Show("Excel 파일을 저장했습니다.", "Excel 저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException)
            {
                MessageBox.Show("Excel 파일을 저장할 수 없습니다.\r\n\r\n저장하려는 파일이 Excel 또는 다른 프로그램에서 열려 있을 수 있습니다.\r\n파일을 닫은 뒤 다시 저장해 주세요.\r\n\r\n파일 경로: " + filePath, "Excel 저장 실패", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Excel 파일을 저장할 권한이 없습니다.\r\n\r\n쓰기 권한이 있는 폴더인지 확인하거나 다른 위치에 저장해 주세요.\r\n\r\n파일 경로: " + filePath, "Excel 저장 실패", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel 저장 중 오류가 발생했습니다.\r\n\r\n" + ex.Message, "Excel 저장 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string BuildDxfFileName(string projectName, string scheduleTitle)
        {
            projectName = FileNameUtil.Sanitize(projectName);
            scheduleTitle = FileNameUtil.Sanitize(scheduleTitle);

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
    }
}