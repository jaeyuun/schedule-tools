using GirderSchedule.Domain.Models;
using GirderSchedule.Infrastructure.Dxf;

namespace GirderSchedule.Infrastructure.Export
{
	public class DxfExportService : IDrawingExportService
	{
		private readonly GirderDxfExporter _exporter;

		public DxfExportService(GirderDxfExporter exporter)
		{
			_exporter = exporter;
		}

		public string Format
		{
			get { return "DXF"; }
		}

		public void Export(string path, GirderScheduleDocumentModel document)
		{
			if (document == null || document.Pages == null || document.Pages.Count == 0)
			{
				return;
			}

			_exporter.Save(path, document.Pages);
		}
	}
}