using System;
using System.IO;
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
            get { return Left.Name; }
            set
            {
                if (Left.Name == value)
                {
                    return;
                }

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

            Left = new SectionItemViewModel(_sheet.Rows[0].Items[0]);
            Center = new SectionItemViewModel(_sheet.Rows[0].Items[1]);
            Right = new SectionItemViewModel(_sheet.Rows[0].Items[2]);

            Center.IsSectionEnabled = true;

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
            var exporter = new ScheduleDxfExporter();

            var options = new ScheduleDxfExportOptions();
            options.IncludeLeft = Left.IsSectionEnabled;
            options.IncludeCenter = true;
            options.IncludeRight = Right.IsSectionEnabled;
            options.IsWidthOver = IsWidthOver;
            options.IsHeightOver = IsHeightOver;
            options.TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "GirderTemplate.dxf");

            exporter.Export(Sheet, filePath, options);
        }
    }
}