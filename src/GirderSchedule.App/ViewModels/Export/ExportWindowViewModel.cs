using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using GirderSchedule.Domain.Models;
using GirderSchedule.Infrastructure.Export;
using Forms = System.Windows.Forms;

namespace GirderSchedule.App.ViewModels.Export
{
	public class ExportWindowViewModel : ViewModelBase
	{
		private readonly IDrawingExportService _dxfExportService;
		private readonly IDrawingExportService _pdfExportService;
		private readonly IDrawingExportService _jpgExportService;

		private GirderScheduleDocumentModel _document;
		private string _fileName = "보일람표";
		private string _selectedFormat = "DXF";
		private string _saveFolder = string.Empty;
		private int _selectedPageIndex;
		private ExportPreviewViewModel _preview = new ExportPreviewViewModel();

		public event EventHandler RequestClose;

		public ObservableCollection<string> Formats { get; } = new ObservableCollection<string>
		{
			"DXF",
			"PDF",
			"JPG"
		};

		public ObservableCollection<string> PageItems { get; } = new ObservableCollection<string>();

		public ICommand BrowseFolderCommand { get; }
		public ICommand ExportCommand { get; }
		public ICommand CloseCommand { get; }

		public ExportWindowViewModel(
			IDrawingExportService dxfExportService,
			IDrawingExportService pdfExportService,
			IDrawingExportService jpgExportService)
		{
			_dxfExportService = dxfExportService;
			_pdfExportService = pdfExportService;
			_jpgExportService = jpgExportService;

			BrowseFolderCommand = new RelayCommand(BrowseFolder);
			ExportCommand = new RelayCommand(Export);
			CloseCommand = new RelayCommand(Close);
		}

		public ExportPreviewViewModel Preview
		{
			get { return _preview; }
			set { SetProperty(ref _preview, value); }
		}

		public string FileName
		{
			get { return _fileName; }
			set { SetProperty(ref _fileName, value); }
		}

		public string SelectedFormat
		{
			get { return _selectedFormat; }
			set { SetProperty(ref _selectedFormat, value); }
		}

		public string SaveFolder
		{
			get { return _saveFolder; }
			set { SetProperty(ref _saveFolder, value); }
		}

		public int SelectedPageIndex
		{
			get { return _selectedPageIndex; }
			set
			{
				SetProperty(ref _selectedPageIndex, value);
				Preview.SelectedPageIndex = value;
			}
		}

		public void Load(GirderScheduleDocumentModel document)
		{
			_document = document;
			PageItems.Clear();

			if (document == null || document.Pages == null)
			{
				Preview.Load(null);
				return;
			}

			for (var i = 0; i < document.Pages.Count; i++)
			{
				PageItems.Add("보 일람표-" + (i + 1));
			}

			Preview.Load(document);
			SelectedPageIndex = 0;
		}

		private void BrowseFolder()
		{
			using (var dialog = new Forms.FolderBrowserDialog())
			{
				dialog.Description = "저장 위치를 선택하세요.";
				dialog.UseDescriptionForTitle = true;

				if (!string.IsNullOrWhiteSpace(SaveFolder) && Directory.Exists(SaveFolder))
				{
					dialog.InitialDirectory = SaveFolder;
				}

				if (dialog.ShowDialog() == Forms.DialogResult.OK)
				{
					SaveFolder = dialog.SelectedPath;
				}
			}
		}

		private void Export()
		{
			if (_document == null)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(SaveFolder) || !Directory.Exists(SaveFolder))
			{
				BrowseFolder();
				if (string.IsNullOrWhiteSpace(SaveFolder) || !Directory.Exists(SaveFolder))
				{
					return;
				}
			}

			var extension = GetExtension(SelectedFormat);
			var path = Path.Combine(SaveFolder, FileName + extension);

			if (SelectedFormat == "DXF")
			{
				_dxfExportService.Export(path, _document);
			}
			else if (SelectedFormat == "PDF")
			{
				_pdfExportService.Export(path, _document);
			}
			else if (SelectedFormat == "JPG")
			{
				_jpgExportService.Export(path, _document);
			}
		}

		private string GetExtension(string format)
		{
			if (format == "PDF")
			{
				return ".pdf";
			}

			if (format == "JPG")
			{
				return ".jpg";
			}

			return ".dxf";
		}

		private void Close()
		{
			var handler = RequestClose;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}
	}
}