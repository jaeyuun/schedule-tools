using System.Windows.Controls;
using GirderSchedule.Renderer.Preview;

namespace GirderSchedule.Renderer.Wpf
{
	public static class ExportPreviewCanvasRenderer
	{
		public static void Render(Canvas canvas, PreviewScene scene, double zoom)
		{
			GirderPreviewCanvasRenderer.Render(canvas, scene, zoom);
		}
	}
}