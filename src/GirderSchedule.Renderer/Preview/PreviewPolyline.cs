using System.Collections.Generic;
using System.Windows;

namespace GirderSchedule.Renderer.Preview
{
	public class PreviewPolyline : PreviewDrawable
	{
		public List<Point> Points { get; set; } = new List<Point>();
		public bool IsClosed { get; set; }
		public string Fill { get; set; } = string.Empty;
	}
}