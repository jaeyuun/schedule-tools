using GirderSchedule.App.Commands;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Services;
using GirderSchedule.Domain.Models;
using GirderSchedule.Dxf.Export;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace GirderSchedule.App.ViewModels
{
    public sealed class ScheduleViewModel : ViewModelBase
    {
        private ScheduleSheet _sheet;
        private ScheduleSet _set;
        private bool _isWidthOver;
        private bool _isHeightOver;

        public SectionItemViewModel Left { get; private set; }
        public SectionItemViewModel Center { get; private set; }
        public SectionItemViewModel Right { get; private set; }

        public ICommand RedrawCommand { get; private set; }
        public ICommand ExportDxfCommand { get; private set; }

        public event EventHandler CurrentSetChanged;

        public ScheduleSheet Sheet
        {
            get { return _sheet; }
        }

        public ScheduleSet CurrentSet
        {
            get { return _set; }
        }

        public string MemberName
        {
            get
            {
                if (_set == null)
                {
                    return string.Empty;
                }

                return _set.MemberName;
            }
            set
            {
                if (_set == null || _set.MemberName == value)
                {
                    return;
                }

                _set.MemberName = value;

                if (Left != null)
                {
                    Left.Name = value;
                }

                if (Center != null)
                {
                    Center.Name = value;
                }

                if (Right != null)
                {
                    Right.Name = value;
                }

                OnPropertyChanged(nameof(MemberName));
                RefreshItems();
                RaiseCurrentSetChanged();
            }
        }

        public double WidthValue
        {
            get { return Left == null ? 0.0 : Left.WidthValue; }
            set
            {
                if (Left == null || Left.WidthValue == value)
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
            get { return Left == null ? 0.0 : Left.HeightValue; }
            set
            {
                if (Left == null || Left.HeightValue == value)
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

            RedrawCommand = new RelayCommand(p => RefreshItems());
            ExportDxfCommand = new RelayCommand(p => ExportDxf());

            LoadSet(_sheet.Rows[0].Sets[0]);
        }

        public void LoadSet(ScheduleSet set)
        {
            if (set == null)
            {
                return;
            }

            _set = set;

            Left = new SectionItemViewModel(_set.Left);
            Center = new SectionItemViewModel(_set.Center);
            Right = new SectionItemViewModel(_set.Right);

            Left.IsSectionEnabled = true;
            Center.IsSectionEnabled = true;
            Right.IsSectionEnabled = true;

            EnsureSheetForCurrentSet();

            OnPropertyChanged(nameof(CurrentSet));
            OnPropertyChanged(nameof(Sheet));
            OnPropertyChanged(nameof(MemberName));
            OnPropertyChanged(nameof(WidthValue));
            OnPropertyChanged(nameof(HeightValue));
            OnPropertyChanged(nameof(Left));
            OnPropertyChanged(nameof(Center));
            OnPropertyChanged(nameof(Right));

            RefreshItems();
        }

        public void ApplyFloorSetting(FloorSetting setting)
        {
            if (setting == null || Left == null || Center == null || Right == null)
            {
                return;
            }

            ApplyFloorSettingToItem(Left, setting);
            ApplyFloorSettingToItem(Center, setting);
            ApplyFloorSettingToItem(Right, setting);

            RefreshItems();
        }

        public void RefreshItems()
        {
            if (Left == null || Center == null || Right == null)
            {
                return;
            }

            Left.RefreshAll();
            Center.RefreshAll();
            Right.RefreshAll();

            OnPropertyChanged(nameof(Left));
            OnPropertyChanged(nameof(Center));
            OnPropertyChanged(nameof(Right));
        }

        private void RaiseCurrentSetChanged()
        {
            var handler = CurrentSetChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void ApplyFloorSettingToItem(SectionItemViewModel item, FloorSetting setting)
        {
            if (setting.MainRebarDiameter > 0)
            {
                item.TopDiameter = setting.MainRebarDiameter;
                item.BottomDiameter = setting.MainRebarDiameter;
            }

            if (setting.StirrupDiameter > 0)
            {
                item.StirrupDiameter = setting.StirrupDiameter;
            }

            if (setting.SkinRebarDiameter > 0)
            {
                item.SkinRebarDiameter = setting.SkinRebarDiameter;
            }
        }

        private void EnsureSheetForCurrentSet()
        {
            var service = new ScheduleBuildService();
            _sheet = service.CreateSample();

            if (_sheet.Rows.Count == 0 || _sheet.Rows[0].Sets.Count == 0)
            {
                return;
            }

            _sheet.Rows[0].Sets[0] = _set;
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
                EnsureSheetForCurrentSet();

                var exporter = new ScheduleDxfExporter();

                var options = new ScheduleDxfExportOptions();
                options.IncludeLeft = Left.IsSectionEnabled;
                options.IncludeCenter = true;
                options.IncludeRight = Right.IsSectionEnabled;
                options.IsWidthOver = IsWidthOver;
                options.IsHeightOver = IsHeightOver;
                options.TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "GirderTemplate.dxf");

                exporter.Export(Sheet, filePath, options);

                MessageBox.Show("DXF 파일을 저장했습니다.", "DXF 저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "DXF 저장 실패", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("DXF 저장 중 오류가 발생했습니다.\r\n\r\n" + ex.Message, "DXF 저장 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}