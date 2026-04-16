using GirderSchedule.Domain.Models;
using netDxf;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;
using System.Windows.Media;

namespace GirderSchedule.Infrastructure.Export
{
	public class PdfExportService : IDrawingExportService
	{
		public string Format
		{
			get { return "PDF"; }
		}

		public void Export(string path, GirderScheduleDocumentModel document)
		{
			if (document == null || document.PageScenes == null || document.PageScenes.Count == 0)
			{
				return;
			}

			var pdf = new PdfDocument();

			for (var i = 0; i < document.PageScenes.Count; i++)
			{
				var scene = document.PageScenes[i];
				var pngBytes = PreviewSceneBitmapRenderer.RenderPngBytes(scene, 1.0, Colors.White);

				var tempPath = Path.GetTempFileName() + ".png";
				File.WriteAllBytes(tempPath, pngBytes);

				try
				{
					var page = pdf.AddPage();

					using (var image = XImage.FromFile(tempPath))
					{
						page.Width = image.PointWidth;
						page.Height = image.PointHeight;

						using (var gfx = XGraphics.FromPdfPage(page))
						{
							gfx.DrawImage(image, 0, 0, page.Width, page.Height);
						}
					}
				}
				finally
				{
					if (File.Exists(tempPath))
					{
						File.Delete(tempPath);
					}
				}
			}

			pdf.Save(path);
		}
	}
}