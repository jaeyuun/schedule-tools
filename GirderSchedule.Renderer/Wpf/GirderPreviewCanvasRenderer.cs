using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GirderSchedule.Renderer.Preview;

namespace GirderSchedule.Renderer.Wpf
{
	public static class GirderPreviewCanvasRenderer
	{
		public static void Render(Canvas canvas, PreviewScene scene, double zoom)
		{
			canvas.Children.Clear();

			if (scene == null)
			{
				canvas.Width = 0;
				canvas.Height = 0;
				return;
			}

			canvas.Width = scene.Width * zoom + 40.0;
			canvas.Height = scene.Height * zoom + 40.0;

			for (var i = 0; i < scene.Items.Count; i++)
			{
				var item = scene.Items[i];

				if (item is PreviewLine)
				{
					DrawLine(canvas, (PreviewLine)item, zoom);
				}
				else if (item is PreviewRect)
				{
					DrawRect(canvas, (PreviewRect)item, zoom);
				}
				else if (item is PreviewCircle)
				{
					DrawCircle(canvas, (PreviewCircle)item, zoom);
				}
				else if (item is PreviewText)
				{
					DrawText(canvas, (PreviewText)item, zoom);
				}
				else if (item is PreviewPolyline)
				{
					DrawPolyline(canvas, (PreviewPolyline)item, zoom);
				}
			}
		}

		private static void DrawLine(Canvas canvas, PreviewLine item, double zoom)
		{
			var line = new Line();
			line.X1 = item.X1 * zoom;
			line.Y1 = item.Y1 * zoom;
			line.X2 = item.X2 * zoom;
			line.Y2 = item.Y2 * zoom;
			line.Stroke = BrushFrom(item.Stroke);
			line.StrokeThickness = item.StrokeThickness * zoom;
			canvas.Children.Add(line);
		}

		private static void DrawRect(Canvas canvas, PreviewRect item, double zoom)
		{
			var rect = new Rectangle();
			rect.Width = item.Width * zoom;
			rect.Height = item.Height * zoom;
			rect.Stroke = BrushFrom(item.Stroke);
			rect.StrokeThickness = item.StrokeThickness * zoom;
			rect.Fill = string.IsNullOrWhiteSpace(item.Fill) ? Brushes.Transparent : BrushFrom(item.Fill);

			Canvas.SetLeft(rect, item.X * zoom);
			Canvas.SetTop(rect, item.Y * zoom);
			canvas.Children.Add(rect);
		}

		private static void DrawCircle(Canvas canvas, PreviewCircle item, double zoom)
		{
			var ellipse = new Ellipse();
			ellipse.Width = item.Radius * 2.0 * zoom;
			ellipse.Height = item.Radius * 2.0 * zoom;
			ellipse.Stroke = BrushFrom(item.Stroke);
			ellipse.StrokeThickness = item.StrokeThickness * zoom;
			ellipse.Fill = string.IsNullOrWhiteSpace(item.Fill) ? Brushes.Transparent : BrushFrom(item.Fill);

			Canvas.SetLeft(ellipse, (item.CenterX - item.Radius) * zoom);
			Canvas.SetTop(ellipse, (item.CenterY - item.Radius) * zoom);
			canvas.Children.Add(ellipse);
		}

		private static void DrawText(Canvas canvas, PreviewText item, double zoom)
		{
			var text = new TextBlock();
			text.Text = item.Text;
			text.Foreground = BrushFrom(item.Stroke);
			text.FontSize = item.FontSize * zoom;
			text.FontFamily = new FontFamily("Malgun Gothic");

			Canvas.SetLeft(text, item.X * zoom);
			Canvas.SetTop(text, item.Y * zoom);
			canvas.Children.Add(text);
		}

		private static void DrawPolyline(Canvas canvas, PreviewPolyline item, double zoom)
		{
			Shape shape;

			if (item.IsClosed)
			{
				var polygon = new Polygon();
				for (var i = 0; i < item.Points.Count; i++)
				{
					polygon.Points.Add(new System.Windows.Point(item.Points[i].X * zoom, item.Points[i].Y * zoom));
				}

				polygon.Fill = string.IsNullOrWhiteSpace(item.Fill) ? Brushes.Transparent : BrushFrom(item.Fill);
				shape = polygon;
			}
			else
			{
				var polyline = new Polyline();
				for (var i = 0; i < item.Points.Count; i++)
				{
					polyline.Points.Add(new System.Windows.Point(item.Points[i].X * zoom, item.Points[i].Y * zoom));
				}

				shape = polyline;
			}

			shape.Stroke = BrushFrom(item.Stroke);
			shape.StrokeThickness = item.StrokeThickness * zoom;
			canvas.Children.Add(shape);
		}

		private static Brush BrushFrom(string color)
		{
			if (string.IsNullOrWhiteSpace(color))
			{
				return Brushes.White;
			}

			return (Brush)new BrushConverter().ConvertFromString(color);
		}
	}
}