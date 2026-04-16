namespace GirderSchedule.Renderer.Preview
{
	public class PreviewCircle : PreviewDrawable
	{
		public double CenterX { get; set; }
		public double CenterY { get; set; }
		public double Radius { get; set; }
		public string Fill { get; set; } = string.Empty;
	}
}