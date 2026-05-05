namespace GirderSchedule.Domain.Models
{
	public class GirderSetModel
	{
		public string MemberName { get; set; } = string.Empty;
		public double Width { get; set; }
		public bool IsWidthOver { get; set; }
		public double Height { get; set; }
		public bool IsHeightOver { get; set; }

		public GirderCellModel Left { get; set; } = new GirderCellModel();
		public GirderCellModel Center { get; set; } = new GirderCellModel();
		public GirderCellModel Right { get; set; } = new GirderCellModel();
	}
}