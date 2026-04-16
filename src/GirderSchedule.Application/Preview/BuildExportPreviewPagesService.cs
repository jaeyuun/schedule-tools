using System;
using System.Collections.Generic;
using System.Windows;
using GirderSchedule.Application.Layout;
using GirderSchedule.Domain.Models;
using GirderSchedule.Renderer.Preview;

namespace GirderSchedule.Application.Preview
{
	public class BuildExportPreviewPagesService
	{
		private const double PreviewPageWidth = 1400.0;
		private const double PreviewPageHeight = 1000.0;
		private const double PageMargin = 40.0;
		private const double TitleBlockWidth = 250.0;
		private const double HeaderHeight = 70.0;
		private const double GridGap = 18.0;
		private const double TitleGap = 24.0;

		public GirderScheduleDocumentModel Build(List<GirderSetModel> sets)
		{
			var document = new GirderScheduleDocumentModel();
			var pages = GirderPaginator.Paginate(sets);

			document.Pages = pages;

			for (var i = 0; i < pages.Count; i++)
			{
				var pageScene = BuildPageScene(pages[i], i + 1);
				document.PageScenes.Add(pageScene);
			}

			if (document.PageScenes.Count > 0)
			{
				document.ExportScene = document.PageScenes[0];
			}

			return document;
		}

		private PreviewScene BuildPageScene(GirderPageModel page, int pageNumber)
		{
			var scene = new PreviewScene();
			scene.Width = PreviewPageWidth;
			scene.Height = PreviewPageHeight;

			AddPageFrame(scene);
			AddPageTitle(scene, pageNumber);
			AddTitleBlock(scene);

			var contentX = PageMargin;
			var contentY = PageMargin + HeaderHeight + TitleGap;
			var contentWidth = PreviewPageWidth - PageMargin * 2.0 - TitleBlockWidth - TitleGap;
			var contentHeight = PreviewPageHeight - PageMargin * 2.0 - HeaderHeight - TitleGap;

			var cellWidth = (contentWidth - GridGap * 2.0) / 3.0;
			var cellHeight = (contentHeight - GridGap * 2.0) / 3.0;

			var previewBuilder = new BuildPreviewSceneService();

			for (var i = 0; i < GirderLayout.MaxSetPerPage; i++)
			{
				var row = i / 3;
				var col = i % 3;

				var x = contentX + col * (cellWidth + GridGap);
				var y = contentY + row * (cellHeight + GridGap);

				AddCellFrame(scene, x, y, cellWidth, cellHeight);

				GirderSetModel set = null;
				if (i < page.Sets.Count)
				{
					set = page.Sets[i];
				}

				if (set == null)
				{
					continue;
				}

				var setScene = previewBuilder.Build(set, true);
				var scaleX = (cellWidth - 16.0) / setScene.Width;
				var scaleY = (cellHeight - 16.0) / setScene.Height;
				var scale = Math.Min(scaleX, scaleY);

				var offsetX = x + (cellWidth - setScene.Width * scale) / 2.0;
				var offsetY = y + (cellHeight - setScene.Height * scale) / 2.0;

				AppendScene(scene, setScene, offsetX, offsetY, scale);
			}

			return scene;
		}

		private void AddPageFrame(PreviewScene scene)
		{
			scene.Items.Add(new PreviewRect
			{
				X = 0.0,
				Y = 0.0,
				Width = scene.Width,
				Height = scene.Height,
				Stroke = "#909090",
				StrokeThickness = 1.0,
				Fill = "#ECECEC"
			});

			scene.Items.Add(new PreviewRect
			{
				X = PageMargin,
				Y = PageMargin,
				Width = scene.Width - PageMargin * 2.0,
				Height = scene.Height - PageMargin * 2.0,
				Stroke = "#606060",
				StrokeThickness = 1.2,
				Fill = "#F8F8F8"
			});
		}

		private void AddPageTitle(PreviewScene scene, int pageNumber)
		{
			scene.Items.Add(new PreviewText
			{
				X = PageMargin + 24.0,
				Y = PageMargin + 14.0,
				Text = "보 일람표",
				FontSize = 24.0,
				Stroke = "#202020"
			});

			scene.Items.Add(new PreviewText
			{
				X = PageMargin + 150.0,
				Y = PageMargin + 17.0,
				Text = pageNumber.ToString(),
				FontSize = 18.0,
				Stroke = "#202020"
			});
		}

		private void AddTitleBlock(PreviewScene scene)
		{
			var x = scene.Width - PageMargin - TitleBlockWidth;
			var y = scene.Height - PageMargin - 190.0;
			var w = TitleBlockWidth;
			var h = 190.0;

			scene.Items.Add(new PreviewRect
			{
				X = x,
				Y = y,
				Width = w,
				Height = h,
				Stroke = "#707070",
				StrokeThickness = 1.0,
				Fill = "#FFFFFF"
			});

			scene.Items.Add(new PreviewLine
			{
				X1 = x,
				Y1 = y + 35.0,
				X2 = x + w,
				Y2 = y + 35.0,
				Stroke = "#B0B0B0",
				StrokeThickness = 1.0
			});

			scene.Items.Add(new PreviewLine
			{
				X1 = x,
				Y1 = y + 80.0,
				X2 = x + w,
				Y2 = y + 80.0,
				Stroke = "#B0B0B0",
				StrokeThickness = 1.0
			});

			scene.Items.Add(new PreviewLine
			{
				X1 = x,
				Y1 = y + 125.0,
				X2 = x + w,
				Y2 = y + 125.0,
				Stroke = "#B0B0B0",
				StrokeThickness = 1.0
			});

			scene.Items.Add(new PreviewText
			{
				X = x + 12.0,
				Y = y + 10.0,
				Text = "PROJECT TITLE",
				FontSize = 11.0,
				Stroke = "#404040"
			});

			scene.Items.Add(new PreviewText
			{
				X = x + 12.0,
				Y = y + 47.0,
				Text = "DRAWING TITLE",
				FontSize = 11.0,
				Stroke = "#404040"
			});

			scene.Items.Add(new PreviewText
			{
				X = x + 12.0,
				Y = y + 92.0,
				Text = "DRAWING NO.",
				FontSize = 11.0,
				Stroke = "#404040"
			});

			scene.Items.Add(new PreviewText
			{
				X = x + 12.0,
				Y = y + 137.0,
				Text = "SCALE",
				FontSize = 11.0,
				Stroke = "#404040"
			});

			scene.Items.Add(new PreviewText
			{
				X = x + 150.0,
				Y = y + 137.0,
				Text = "1 / 60",
				FontSize = 12.0,
				Stroke = "#202020"
			});
		}

		private void AddCellFrame(PreviewScene scene, double x, double y, double width, double height)
		{
			scene.Items.Add(new PreviewRect
			{
				X = x,
				Y = y,
				Width = width,
				Height = height,
				Stroke = "#C0C0C0",
				StrokeThickness = 0.8,
				Fill = "#FFFFFF"
			});
		}

		private void AppendScene(PreviewScene target, PreviewScene source, double offsetX, double offsetY, double scale)
		{
			for (var i = 0; i < source.Items.Count; i++)
			{
				var item = source.Items[i];

				if (item is PreviewLine)
				{
					var line = (PreviewLine)item;
					target.Items.Add(new PreviewLine
					{
						X1 = offsetX + line.X1 * scale,
						Y1 = offsetY + line.Y1 * scale,
						X2 = offsetX + line.X2 * scale,
						Y2 = offsetY + line.Y2 * scale,
						Stroke = line.Stroke,
						StrokeThickness = Math.Max(0.5, line.StrokeThickness * scale)
					});
				}
				else if (item is PreviewRect)
				{
					var rect = (PreviewRect)item;
					target.Items.Add(new PreviewRect
					{
						X = offsetX + rect.X * scale,
						Y = offsetY + rect.Y * scale,
						Width = rect.Width * scale,
						Height = rect.Height * scale,
						Stroke = rect.Stroke,
						StrokeThickness = Math.Max(0.5, rect.StrokeThickness * scale),
						Fill = rect.Fill
					});
				}
				else if (item is PreviewCircle)
				{
					var circle = (PreviewCircle)item;
					target.Items.Add(new PreviewCircle
					{
						CenterX = offsetX + circle.CenterX * scale,
						CenterY = offsetY + circle.CenterY * scale,
						Radius = circle.Radius * scale,
						Stroke = circle.Stroke,
						StrokeThickness = Math.Max(0.5, circle.StrokeThickness * scale),
						Fill = circle.Fill
					});
				}
				else if (item is PreviewText)
				{
					var text = (PreviewText)item;
					target.Items.Add(new PreviewText
					{
						X = offsetX + text.X * scale,
						Y = offsetY + text.Y * scale,
						Text = text.Text,
						FontSize = Math.Max(8.0, text.FontSize * scale),
						Stroke = text.Stroke
					});
				}
				else if (item is PreviewPolyline)
				{
					var polyline = (PreviewPolyline)item;
					var copied = new PreviewPolyline
					{
						Stroke = polyline.Stroke,
						StrokeThickness = Math.Max(0.5, polyline.StrokeThickness * scale),
						Fill = polyline.Fill,
						IsClosed = polyline.IsClosed
					};

					for (var j = 0; j < polyline.Points.Count; j++)
					{
						copied.Points.Add(new Point(
							offsetX + polyline.Points[j].X * scale,
							offsetY + polyline.Points[j].Y * scale));
					}

					target.Items.Add(copied);
				}
			}
		}
	}
}