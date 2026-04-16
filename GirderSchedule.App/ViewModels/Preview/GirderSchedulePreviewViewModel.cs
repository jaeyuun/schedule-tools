using GirderSchedule.Application.Preview;
using GirderSchedule.Domain;
using GirderSchedule.Domain.Models;
using GirderSchedule.Renderer.Preview;

namespace GirderSchedule.App.ViewModels.Preview
{
	public class GirderSchedulePreviewViewModel : ViewModelBase
	{
		private PreviewScene _scene = new PreviewScene();
		private GirderSetModel _currentSet = new GirderSetModel();
		private bool _showForce = true;
		private double _zoom = 1.0;

		public PreviewScene Scene
		{
			get { return _scene; }
			set { SetProperty(ref _scene, value); }
		}

		public GirderSetModel CurrentSet
		{
			get { return _currentSet; }
			set
			{
				SetProperty(ref _currentSet, value);
				Refresh();
			}
		}

		public bool ShowForce
		{
			get { return _showForce; }
			set
			{
				SetProperty(ref _showForce, value);
				Refresh();
			}
		}

		public double Zoom
		{
			get { return _zoom; }
			set { SetProperty(ref _zoom, value); }
		}

		public void Refresh()
		{
			var service = new BuildPreviewSceneService();
			Scene = service.Build(CurrentSet, ShowForce);
		}
	}
}