namespace GirderSchedule.Domain.Models
{
	public class GirderCellModel
	{
		public bool IsVisible { get; set; }
		public string Title { get; set; } = string.Empty;
		public SectionCellType Type { get; set; }

		public RebarLayerModel Top1 { get; set; } = new RebarLayerModel();
		public RebarLayerModel Top2 { get; set; } = new RebarLayerModel();
		public RebarLayerModel Bottom1 { get; set; } = new RebarLayerModel();
		public RebarLayerModel Bottom2 { get; set; } = new RebarLayerModel();
		public StirrupModel Stirrup { get; set; } = new StirrupModel();
		public string SkinRebarText { get; set; } = "-";
		public ForceDisplayModel Force { get; set; } = new ForceDisplayModel();
	}
}