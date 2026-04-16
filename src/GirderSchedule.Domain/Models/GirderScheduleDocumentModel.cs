using GirderSchedule.Renderer.Preview;
using System.Collections.Generic;

namespace GirderSchedule.Domain.Models
{
	public class GirderScheduleDocumentModel
	{
		public List<GirderPageModel> Pages { get; set; } = new List<GirderPageModel>();
		public List<PreviewScene> PageScenes { get; set; } = new List<PreviewScene>();
		public PreviewScene ExportScene { get; set; } = new PreviewScene();
	}
}