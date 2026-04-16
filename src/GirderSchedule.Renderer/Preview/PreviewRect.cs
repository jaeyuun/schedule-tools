namespace GirderSchedule.Renderer.Preview
{
	public class PreviewRect : PreviewDrawable
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public string Fill { get; set; } = string.Empty;
	}
}