using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GirderSchedule.Renderer.Preview;

namespace GirderSchedule.Infrastructure.Export
{
	internal static class PreviewSceneBitmapRenderer
	{
		public static BitmapSource RenderBitmap(PreviewScene scene, double zoom, Color background)
		{
			var width = (int)System.Math.Ceiling(scene.Width * zoom);
			var height = (int)System.Math.Ceiling(scene.Height * zoom);

			if (width <= 0)
			{
				width = 1;
			}

			if (height <= 0)
			{
				height = 1;
			}

			var visual = new DrawingVisual();
			using (var dc = visual.RenderOpen())
			{
				dc.DrawRectangle(new SolidColorBrush(background), null, new Rect(0, 0, width, height));

				for (var i = 0; i < scene.Items.Count; i++)
				{
					var item = scene.Items[i];

					if (item is PreviewLine)
					{
						var line = (PreviewLine)item;
						dc.DrawLine(
							CreatePen(line.Stroke, line.StrokeThickness * zoom),
							new Point(line.X1 * zoom, line.Y1 * zoom),
							new Point(line.X2 * zoom, line.Y2 * zoom));
					}
					else if (item is PreviewRect)
					{
						var rect = (PreviewRect)item;
						dc.DrawRectangle(
							CreateBrush(rect.Fill),
							CreatePen(rect.Stroke, rect.StrokeThickness * zoom),
							new Rect(rect.X * zoom, rect.Y * zoom, rect.Width * zoom, rect.Height * zoom));
					}
					else if (item is PreviewCircle)
					{
						var circle = (PreviewCircle)item;
						dc.DrawEllipse(
							CreateBrush(circle.Fill),
							CreatePen(circle.Stroke, circle.StrokeThickness * zoom),
							new Point(circle.CenterX * zoom, circle.CenterY * zoom),
							circle.Radius * zoom,
							circle.Radius * zoom);
					}
					else if (item is PreviewText)
					{
						var text = (PreviewText)item;
						var formatted = new FormattedText(
							text.Text ?? string.Empty,
							CultureInfo.CurrentCulture,
							FlowDirection.LeftToRight,
							new Typeface(new FontFamily("Malgun Gothic"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
							text.FontSize * zoom,
							CreateBrush(text.Stroke),
							1.0);

						dc.DrawText(formatted, new Point(text.X * zoom, text.Y * zoom));
					}
					else if (item is PreviewPolyline)
					{
						var polyline = (PreviewPolyline)item;
						if (polyline.Points.Count == 0)
						{
							continue;
						}

						var geometry = new StreamGeometry();
						using (var gc = geometry.Open())
						{
							gc.BeginFigure(
								new Point(polyline.Points[0].X * zoom, polyline.Points[0].Y * zoom),
								!string.IsNullOrWhiteSpace(polyline.Fill),
								polyline.IsClosed);

							for (var j = 1; j < polyline.Points.Count; j++)
							{
								gc.LineTo(
									new Point(polyline.Points[j].X * zoom, polyline.Points[j].Y * zoom),
									true,
									false);
							}
						}

						geometry.Freeze();

						dc.DrawGeometry(
							CreateBrush(polyline.Fill),
							CreatePen(polyline.Stroke, polyline.StrokeThickness * zoom),
							geometry);
					}
				}
			}

			var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
			bitmap.Render(visual);
			bitmap.Freeze();
			return bitmap;
		}

		public static byte[] RenderPngBytes(PreviewScene scene, double zoom, Color background)
		{
			var bitmap = RenderBitmap(scene, zoom, background);
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bitmap));

			using (var stream = new MemoryStream())
			{
				encoder.Save(stream);
				return stream.ToArray();
			}
		}

		public static void SaveJpeg(PreviewScene scene, string path, double zoom, Color background, int quality)
		{
			var bitmap = RenderBitmap(scene, zoom, background);
			var encoder = new JpegBitmapEncoder();
			encoder.QualityLevel = quality;
			encoder.Frames.Add(BitmapFrame.Create(bitmap));

			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				encoder.Save(stream);
			}
		}

		private static Pen CreatePen(string color, double thickness)
		{
			return new Pen(CreateBrush(color), thickness <= 0 ? 1.0 : thickness);
		}

		private static Brush CreateBrush(string color)
		{
			if (string.IsNullOrWhiteSpace(color))
			{
				return Brushes.Transparent;
			}

			return (Brush)new BrushConverter().ConvertFromString(color);
		}
	}
}