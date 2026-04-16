using System.Collections.Generic;

namespace GirderSchedule.Domain.Models
{
	public class GirderPageModel
	{
		public int PageNumber { get; set; }
		public List<GirderSetModel> Sets { get; set; } = new List<GirderSetModel>();
	}
}