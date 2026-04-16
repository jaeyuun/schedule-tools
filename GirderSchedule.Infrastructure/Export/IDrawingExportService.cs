using GirderSchedule.Domain.Models;

namespace GirderSchedule.Infrastructure.Export
{
	public interface IDrawingExportService
	{
		string Format { get; }
		void Export(string path, GirderScheduleDocumentModel document);
	}
}