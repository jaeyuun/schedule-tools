using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Girder.Services;
using GirderSchedule.Dxf.Girder;
using Microsoft.Win32;

namespace GirderSchedule.App.ViewModels.Girder
{
    public sealed class ScheduleViewModel : ViewModelBase
    {
        private readonly ScheduleSheet _sheet;
        private readonly ScheduleSet _set;
        private bool _isWidthOver;
        private bool _isHeightOver;

        public SectionItemViewModel Left { get; private set; }
        public SectionItemViewModel Center { get; private set; }
        public SectionItemViewModel Right { get; private set; }

        public ICommand RedrawCommand { get; private set; }
        public ICommand ExportDxfCommand { get; private set; }

        public ScheduleSheet Sheet
        {
            get { return _sheet; }
        }

        public string MemberName
        {
            get { return _set.MemberName; }
            set
            {
                if (_set.MemberName == value)
                {
                    return;
                }

                _set.MemberName = value;

                Left.Name = value;
                Center.Name = value;
                Right.Name = value;

                OnPropertyChanged(nameof(MemberName));
                RefreshItems();
            }
        }

        public double WidthValue
        {
            get { return Left.WidthValue; }
            set
            {
                if (Left.WidthValue == value)
                {
                    return;
                }

                Left.WidthValue = value;
                Center.WidthValue = value;
                Right.WidthValue = value;

                OnPropertyChanged(nameof(WidthValue));
                RefreshItems();
            }
        }

        public double HeightValue
        {
            get { return Left.HeightValue; }
            set
            {
                if (Left.HeightValue == value)
                {
                    return;
                }

                Left.HeightValue = value;
                Center.HeightValue = value;
                Right.HeightValue = value;

                OnPropertyChanged(nameof(HeightValue));
                RefreshItems();
            }
        }

        public bool IsWidthOver
        {
            get { return _isWidthOver; }
            set
            {
                if (_isWidthOver == value)
                {
                    return;
                }

                _isWidthOver = value;
                OnPropertyChanged(nameof(IsWidthOver));
                RefreshItems();
            }
        }

        public bool IsHeightOver
        {
            get { return _isHeightOver; }
            set
            {
                if (_isHeightOver == value)
                {
                    return;
                }

                _isHeightOver = value;
                OnPropertyChanged(nameof(IsHeightOver));
                RefreshItems();
            }
        }

        public ScheduleViewModel()
        {
            var service = new ScheduleBuildService();
            _sheet = service.CreateSample();

            _set = _sheet.Rows[0].Sets[0];

            Left = new SectionItemViewModel(_set.Left);
            Center = new SectionItemViewModel(_set.Center);
            Right = new SectionItemViewModel(_set.Right);

            Left.IsSectionEnabled = true;
            Center.IsSectionEnabled = true;
            Right.IsSectionEnabled = true;

            RedrawCommand = new RelayCommand(p => RefreshItems());
            ExportDxfCommand = new RelayCommand(p => ExportDxf());
        }

        public void RefreshItems()
        {
            Left.RefreshAll();
            Center.RefreshAll();
            Right.RefreshAll();

            OnPropertyChanged(nameof(Left));
            OnPropertyChanged(nameof(Center));
            OnPropertyChanged(nameof(Right));
        }

        private void ExportDxf()
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "DXF 저장";
            dialog.Filter = "DXF 파일 (*.dxf)|*.dxf";
            dialog.FileName = string.IsNullOrWhiteSpace(MemberName) ? "BeamSchedule.dxf" : MemberName + ".dxf";
            dialog.DefaultExt = ".dxf";
            dialog.AddExtension = true;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            ExportDxf(dialog.FileName);
        }

        private void ExportDxf(string filePath)
        {
            try
            {
                var exporter = new ScheduleDxfExporter();

                var options = new ScheduleDxfExportOptions();
                options.IncludeLeft = Left.IsSectionEnabled;
                options.IncludeCenter = true;
                options.IncludeRight = Right.IsSectionEnabled;
                options.IsWidthOver = IsWidthOver;
                options.IsHeightOver = IsHeightOver;
                options.TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "GirderTemplate.dxf");

                exporter.Export(Sheet, filePath, options);

                MessageBox.Show(
                    "DXF 파일을 저장했습니다.",
                    "DXF 저장 완료",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "DXF 저장 실패",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "DXF 파일을 저장할 수 없습니다.\r\n\r\n" +
                    "저장하려는 파일이 AutoCAD, GstarCAD, 뷰어 또는 다른 프로그램에서 열려 있을 수 있습니다.\r\n" +
                    "파일을 닫은 뒤 다시 저장해 주세요.\r\n\r\n" +
                    "파일 경로: " + filePath,
                    "DXF 저장 실패",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "DXF 파일을 저장할 권한이 없습니다.\r\n\r\n" +
                    "쓰기 권한이 있는 폴더인지 확인하거나 다른 위치에 저장해 주세요.\r\n\r\n" +
                    "파일 경로: " + filePath,
                    "DXF 저장 실패",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "DXF 저장 중 오류가 발생했습니다.\r\n\r\n" + ex.Message,
                    "DXF 저장 실패",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}