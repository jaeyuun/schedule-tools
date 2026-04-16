namespace GirderSchedule.Domain.Models
{
	public class GirderSetModel
	{
		public string MemberName { get; set; } = string.Empty;
		public int Width { get; set; }
		public int Height { get; set; }

		public GirderCellModel Left { get; set; } = new GirderCellModel();
		public GirderCellModel Center { get; set; } = new GirderCellModel();
		public GirderCellModel Right { get; set; } = new GirderCellModel();
	}
}