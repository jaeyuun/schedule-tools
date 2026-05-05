using GirderSchedule.Application;
using GirderSchedule.Application.Layout;
using GirderSchedule.Application.Services;
using GirderSchedule.Domain;
using GirderSchedule.Domain.Models;
using GirderSchedule.Renderer.Preview;
using System;
using System.Collections.Generic;

namespace GirderSchedule.Application.Preview
{
	public class BuildPreviewSceneService
	{
		public PreviewScene Build(GirderSetModel set, bool showForce)
		{
			var scene = new PreviewScene();
			scene.Width = GirderLayout.SetWidth;
			scene.Height = GirderLayout.SetHeight;

			DrawSetFrame(scene, 0.0, 0.0);
			DrawSetHeader(scene, set);
			DrawCell(scene, 0, set.Left, set.Width, set.Height, showForce);
			DrawCell(scene, 1, set.Center, set.Width, set.Height, showForce);
			DrawCell(scene, 2, set.Right, set.Width, set.Height, showForce);

			return scene;
		}

		private void DrawSetFrame(PreviewScene scene, double x, double y)
		{
			AddRect(scene, x, y, GirderLayout.SetWidth, GirderLayout.SetHeight, "#BFBFBF", 1.0, string.Empty);

			AddLine(scene, x + 1200, y, x + 1200, y + 5000, "#BFBFBF", 1.0);
			AddLine(scene, x, y + 600, x + 22710, y + 600, "#BFBFBF", 1.0);
			AddLine(scene, x, y + 4400, x + 22710, y + 4400, "#BFBFBF", 1.0);
			AddLine(scene, x, y + 4100, x + 22710, y + 4100, "#D8C8FF", 1.0);
			AddLine(scene, x, y + 3800, x + 22710, y + 3800, "#D8C8FF", 1.0);
			AddLine(scene, x, y + 3500, x + 22710, y + 3500, "#BFBFBF", 1.0);
			AddLine(scene, x, y + 3200, x + 22710, y + 3200, "#BFBFBF", 1.0);

			AddLine(scene, x + 8370, y, x + 8370, y + 5000, "#BFBFBF", 1.0);
			AddLine(scene, x + 15540, y, x + 15540, y + 5000, "#BFBFBF", 1.0);

			AddText(scene, 600, 300, "부 재 명", 120, "#222222");
			AddText(scene, 600, 2600, "단    면", 90, "#222222");
			AddText(scene, 600, 4200, "상 부 근", 90, "#222222");
			AddText(scene, 600, 4500, "하 부 근", 90, "#222222");
			AddText(scene, 600, 4800, "스 터 럽", 90, "#222222");
			AddText(scene, 600, 5100, "표피철근(X)", 90, "#222222");
		}

		private void DrawSetHeader(PreviewScene scene, GirderSetModel set)
		{
			AddText(scene, 8300, 250, set.MemberName, 120, "#00AA00");
			AddText(scene, 8600, 480, string.Format("({0} × {1})", set.Width, set.Height), 120, "#00AA00");
		}

		private void DrawCell(PreviewScene scene, int cellIndex, GirderCellModel cell, double width, double height, bool showForce)
		{
			if (cell == null || !cell.IsVisible)
			{
				return;
			}

			var cellWidth = (GirderLayout.SetWidth - GirderLayout.LabelColumnWidth) / 3.0;
			var cellX = GirderLayout.LabelColumnWidth + cellWidth * cellIndex;
			var titleX = cellX + cellWidth / 2.0 - 300.0;

			AddText(scene, titleX, 820, cell.Title, 90, "#00AA00");

			if (showForce && cell.Force != null && cell.Force.Visible)
			{
				AddText(scene, cellX + 350, 1030, "M = " + cell.Force.M, 80, "#7A50C8");
				AddText(scene, cellX + 1350, 1030, "V = " + cell.Force.V, 80, "#7A50C8");
			}

			DrawSection(scene, cellX, cellWidth, width, height, cell);
			DrawBottomTexts(scene, cellX, cellWidth, cell);
		}

		private void DrawSection(PreviewScene scene, double cellX, double cellWidth, double width, double height, GirderCellModel cell)
		{
			var scale = GetSectionScale(width, height);
			var drawWidth = width * scale;
			var drawHeight = height * scale;

			var topY = 1700.0;
			var leftX = cellX + (cellWidth - drawWidth) / 2.0;
			var rightX = leftX + drawWidth;
			var bottomY = topY + drawHeight;

			AddLine(scene, leftX - 80, topY, rightX + 80, topY, "#FF00FF", 2.0);

			AddPolyline(scene, new[]
			{
				Tuple.Create(leftX, topY + 40.0),
				Tuple.Create(leftX, bottomY),
				Tuple.Create(rightX, bottomY),
				Tuple.Create(rightX, topY + 40.0)
			}, false, "#00FF00", 2.0, string.Empty);

			DrawDimension(scene, leftX, topY - 120, rightX, topY - 120, width.ToString(), true);
			DrawDimension(scene, leftX - 140, topY, leftX - 140, bottomY, height.ToString(), false);

			DrawRebars(scene, leftX, rightX, topY, bottomY, cell);
			DrawStirrups(scene, leftX, rightX, topY, bottomY, cell);

			if (height > 900)
			{
				DrawX(scene, (leftX + rightX) / 2.0, (topY + bottomY) / 2.0);
			}
		}

		private void DrawRebars(PreviewScene scene, double leftX, double rightX, double topY, double bottomY, GirderCellModel cell)
		{
			DrawRebarLayer(scene, leftX, rightX, topY + 20.0, cell.Top1, true, 0.0);
			DrawRebarLayer(scene, leftX, rightX, topY + 55.0, cell.Top2, true, 0.0);
			DrawRebarLayer(scene, leftX, rightX, bottomY - 20.0, cell.Bottom1, false, 0.0);
			DrawRebarLayer(scene, leftX, rightX, bottomY - 55.0, cell.Bottom2, false, 0.0);
		}

		private void DrawRebarLayer(PreviewScene scene, double leftX, double rightX, double y, RebarLayerModel layer, bool isTop, double offset)
		{
			if (layer == null || layer.FirstCount <= 0 || layer.Diameter <= 0)
			{
				return;
			}

			var radius = Math.Max(3.0, layer.Diameter * 0.35);
			var minX = leftX + radius + 10.0;
			var maxX = rightX - radius - 10.0;
			var firstXs = RebarPlacementService.GetFirstLayerPositions(minX, maxX, layer.FirstCount);
			var secondXs = RebarPlacementService.GetSecondLayerPositions(minX, maxX, layer.FirstCount, layer.SecondCount);

			for (var i = 0; i < firstXs.Count; i++)
			{
				AddCircle(scene, firstXs[i], y, radius, "#00FF00", 1.5, "#00FF00");
			}

			for (var i = 0; i < secondXs.Count; i++)
			{
				AddCircle(scene, secondXs[i], y, radius, "#00FF00", 1.5, "#00FF00");
			}
		}

		private void DrawStirrups(PreviewScene scene, double leftX, double rightX, double topY, double bottomY, GirderCellModel cell)
		{
			if (cell == null || cell.Stirrup == null || cell.Stirrup.Legs <= 0)
			{
				return;
			}

			var x1 = leftX + 8.0;
			var x2 = rightX - 8.0;
			var y1 = topY + 10.0;
			var y2 = bottomY - 10.0;

			AddRect(scene, x1, y1, x2 - x1, y2 - y1, "#00FF00", 1.5, string.Empty);

			var xs = RebarPlacementService.GetStirrupLegPositions(x1 + 10.0, x2 - 10.0, cell.Stirrup.Legs);
			for (var i = 0; i < xs.Count; i++)
			{
				AddLine(scene, xs[i], y1, xs[i], y2, "#00FF00", 1.2);
			}
		}

		private void DrawBottomTexts(PreviewScene scene, double cellX, double cellWidth, GirderCellModel cell)
		{
			var textX = cellX + 500.0;

			AddText(scene, textX, 4200, BuildRebarText(cell.Top1, cell.Top2), 90, "#00AA00");
			AddText(scene, textX, 4500, BuildRebarText(cell.Bottom1, cell.Bottom2), 90, "#00AA00");
			AddText(scene, textX, 4800, BuildStirrupText(cell.Stirrup), 90, "#00AA00");
			AddText(scene, textX, 5100, string.IsNullOrWhiteSpace(cell.SkinRebarText) ? "-" : cell.SkinRebarText, 90, "#00AA00");
		}

		private void DrawDimension(PreviewScene scene, double x1, double y1, double x2, double y2, string text, bool horizontal)
		{
			AddLine(scene, x1, y1, x2, y2, "#FFFFFF", 1.0);

			if (horizontal)
			{
				AddLine(scene, x1, y1 - 20, x1, y1 + 20, "#FFFFFF", 1.0);
				AddLine(scene, x2, y2 - 20, x2, y2 + 20, "#FFFFFF", 1.0);
				AddText(scene, (x1 + x2) / 2.0 - 25.0, y1 - 10.0, text, 75, "#00FF00");
			}
			else
			{
				AddLine(scene, x1 - 20, y1, x1 + 20, y1, "#FFFFFF", 1.0);
				AddLine(scene, x2 - 20, y2, x2 + 20, y2, "#FFFFFF", 1.0);
				AddText(scene, x1 - 40.0, (y1 + y2) / 2.0, text, 75, "#00FF00");
			}
		}

		private void DrawX(PreviewScene scene, double cx, double cy)
		{
			AddLine(scene, cx - 20, cy - 20, cx + 20, cy + 20, "#00FF00", 2.0);
			AddLine(scene, cx - 20, cy + 20, cx + 20, cy - 20, "#00FF00", 2.0);
		}

		private double GetSectionScale(double width, double height)
		{
			var sx = 600.0 / width;
			var sy = 950.0 / height;
			return Math.Min(sx, sy);
		}

		private string BuildRebarText(RebarLayerModel layer1, RebarLayerModel layer2)
		{
			var parts = new List<string>();

			if (layer1 != null && layer1.TotalCount > 0 && layer1.Diameter > 0)
			{
				parts.Add(string.Format("{0}-HD{1}", layer1.TotalCount, layer1.Diameter));
			}

			if (layer2 != null && layer2.TotalCount > 0 && layer2.Diameter > 0)
			{
				parts.Add(string.Format("{0}-HD{1}", layer2.TotalCount, layer2.Diameter));
			}

			if (parts.Count == 0)
			{
				return "- HD";
			}

			return string.Join(" + ", parts.ToArray());
		}

		private string BuildStirrupText(StirrupModel stirrup)
		{
			if (stirrup == null || stirrup.Legs <= 0 || stirrup.Diameter <= 0 || stirrup.Spacing <= 0)
			{
				return "- HD @";
			}

			return string.Format("{0}-HD{1} @{2}", stirrup.Legs, stirrup.Diameter, stirrup.Spacing);
		}

		private void AddLine(PreviewScene scene, double x1, double y1, double x2, double y2, string color, double thickness)
		{
			scene.Items.Add(new PreviewLine
			{
				X1 = x1,
				Y1 = y1,
				X2 = x2,
				Y2 = y2,
				Stroke = color,
				StrokeThickness = thickness
			});
		}

		private void AddRect(PreviewScene scene, double x, double y, double width, double height, string color, double thickness, string fill)
		{
			scene.Items.Add(new PreviewRect
			{
				X = x,
				Y = y,
				Width = width,
				Height = height,
				Stroke = color,
				StrokeThickness = thickness,
				Fill = fill
			});
		}

		private void AddCircle(PreviewScene scene, double cx, double cy, double radius, string color, double thickness, string fill)
		{
			scene.Items.Add(new PreviewCircle
			{
				CenterX = cx,
				CenterY = cy,
				Radius = radius,
				Stroke = color,
				StrokeThickness = thickness,
				Fill = fill
			});
		}

		private void AddText(PreviewScene scene, double x, double y, string text, double fontSize, string color)
		{
			scene.Items.Add(new PreviewText
			{
				X = x,
				Y = y,
				Text = text,
				FontSize = fontSize,
				Stroke = color
			});
		}

		private void AddPolyline(PreviewScene scene, Tuple<double, double>[] points, bool isClosed, string color, double thickness, string fill)
		{
			var polyline = new PreviewPolyline();
			polyline.Stroke = color;
			polyline.StrokeThickness = thickness;
			polyline.IsClosed = isClosed;
			polyline.Fill = fill;

			for (var i = 0; i < points.Length; i++)
			{
				polyline.Points.Add(new System.Windows.Point(points[i].Item1, points[i].Item2));
			}

			scene.Items.Add(polyline);
		}
	}
}