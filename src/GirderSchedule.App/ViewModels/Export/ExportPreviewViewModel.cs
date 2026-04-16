using GirderSchedule.Domain.Models;
using GirderSchedule.Renderer.Preview;

namespace GirderSchedule.App.ViewModels.Export
{
	public class ExportPreviewViewModel : ViewModelBase
	{
		private GirderScheduleDocumentModel _document;
		private PreviewScene _scene = new PreviewScene();
		private double _zoom = 1.0;
		private int _selectedPageIndex;

		public PreviewScene Scene
		{
			get { return _scene; }
			set { SetProperty(ref _scene, value); }
		}

		public double Zoom
		{
			get { return _zoom; }
			set
			{
				SetProperty(ref _zoom, value);
				Refresh();
			}
		}

		public int SelectedPageIndex
		{
			get { return _selectedPageIndex; }
			set
			{
				SetProperty(ref _selectedPageIndex, value);
				Refresh();
			}
		}

		public void Load(GirderScheduleDocumentModel document)
		{
			_document = document;
			_selectedPageIndex = 0;
			Refresh();
		}

		public void Refresh()
		{
			if (_document == null || _document.PageScenes == null || _document.PageScenes.Count == 0)
			{
				Scene = new PreviewScene();
				return;
			}

			var index = SelectedPageIndex;
			if (index < 0)
			{
				index = 0;
			}

			if (index >= _document.PageScenes.Count)
			{
				index = _document.PageScenes.Count - 1;
			}

			Scene = _document.PageScenes[index];
		}
	}
}