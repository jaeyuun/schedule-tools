namespace GirderSchedule.Domain.Models
{
	public class RebarLayerModel
	{
		public int FirstCount { get; set; }
		public int SecondCount { get; set; }
		public int Diameter { get; set; }

		public int TotalCount
		{
			get { return FirstCount + SecondCount; }
		}
	}
}