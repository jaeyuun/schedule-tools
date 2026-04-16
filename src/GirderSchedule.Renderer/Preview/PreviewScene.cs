using System.Collections.Generic;

namespace GirderSchedule.Renderer.Preview
{
	public class PreviewScene
	{
		public double Width { get; set; }
		public double Height { get; set; }
		public List<PreviewDrawable> Items { get; set; } = new List<PreviewDrawable>();
	}
}