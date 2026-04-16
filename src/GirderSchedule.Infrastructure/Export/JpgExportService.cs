using System.IO;
using System.Windows.Media;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.Infrastructure.Export
{
	public class JpgExportService : IDrawingExportService
	{
		public string Format
		{
			get { return "JPG"; }
		}

		public void Export(string path, GirderScheduleDocumentModel document)
		{
			if (document == null || document.PageScenes == null || document.PageScenes.Count == 0)
			{
				return;
			}

			var directory = Path.GetDirectoryName(path);
			var fileName = Path.GetFileNameWithoutExtension(path);
			var extension = Path.GetExtension(path);

			if (string.IsNullOrWhiteSpace(extension))
			{
				extension = ".jpg";
			}

			if (document.PageScenes.Count == 1)
			{
				PreviewSceneBitmapRenderer.SaveJpeg(document.PageScenes[0], path, 1.0, Colors.White, 95);
				return;
			}

			for (var i = 0; i < document.PageScenes.Count; i++)
			{
				var pagePath = Path.Combine(directory, fileName + "_" + (i + 1) + extension);
				PreviewSceneBitmapRenderer.SaveJpeg(document.PageScenes[i], pagePath, 1.0, Colors.White, 95);
			}
		}
	}
}