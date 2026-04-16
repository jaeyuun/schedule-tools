using GirderSchedule.Application.Layout;
using GirderSchedule.Application.Services;
using GirderSchedule.Domain.Models;
using netDxf;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Tables;
using System;
using System.Collections.Generic;
using TextEntity = netDxf.Entities.Text;

namespace GirderSchedule.Infrastructure.Dxf
{
	public class GirderDxfExporter
	{
		public DxfDocument BuildDocument(List<GirderPageModel> pages)
		{
			var document = new DxfDocument(DxfVersion.AutoCad2010);

			EnsureLayers(document);
			EnsureTextStyles(document);

			for (var i = 0; i < pages.Count; i++)
			{
				DrawPage(document, pages[i], i);
			}

			return document;
		}

		public void Save(string path, List<GirderPageModel> pages)
		{
			var document = BuildDocument(pages);
			document.Save(path);
		}

		private void EnsureLayers(DxfDocument document)
		{
			AddLayer(document, DxfLayerNames.FormLine, new AciColor(8));
			AddLayer(document, DxfLayerNames.FormText, new AciColor(7));
			AddLayer(document, DxfLayerNames.Text, new AciColor(3));
			AddLayer(document, DxfLayerNames.Rebar, new AciColor(3));
			AddLayer(document, DxfLayerNames.RcGir, new AciColor(6));
			AddLayer(document, DxfLayerNames.Dim, new AciColor(7));
		}

		private void AddLayer(DxfDocument document, string name, AciColor color)
		{
			if (document.Layers.Contains(name))
			{
				return;
			}

			var layer = new Layer(name);
			layer.Color = color;
			document.Layers.Add(layer);
		}

		private void EnsureTextStyles(DxfDocument document)
		{
			if (document.TextStyles.Contains("맑은고딕"))
			{
				return;
			}

			var style = new TextStyle("맑은고딕", "Malgun Gothic", FontStyle.Regular);
			document.TextStyles.Add(style);
		}

		private void DrawPage(DxfDocument document, GirderPageModel page, int pageIndex)
		{
			var pageOriginX = pageIndex * (GirderLayout.PageWidth + 1000.0);
			var pageOriginY = 0.0;

			DrawPageFrame(document, pageOriginX, pageOriginY, page.PageNumber);

			for (var i = 0; i < GirderLayout.MaxSetPerPage; i++)
			{
				var row = i / 3;
				var col = i % 3;

				var x = pageOriginX + GirderLayout.ContentLeft + col * GirderLayout.SetWidth;
				var y = pageOriginY + GirderLayout.ContentTop - row * GirderLayout.SetHeight;

				GirderSetModel set = null;
				if (i < page.Sets.Count)
				{
					set = page.Sets[i];
				}

				DrawSet(document, x, y, set);
			}
		}

		private void DrawPageFrame(DxfDocument document, double x, double y, int pageNumber)
		{
			AddRect(document, x, y, GirderLayout.PageWidth, GirderLayout.PageHeight, DxfLayerNames.FormLine);
			AddText(document, "보 일람표", x + 18000.0, y + 28500.0, 230.0, DxfLayerNames.FormText);
			AddText(document, pageNumber.ToString(), x + 17000.0, y + 28500.0, 180.0, DxfLayerNames.FormText);
		}

		private void DrawSet(DxfDocument document, double originX, double originY, GirderSetModel set)
		{
			DrawSetFrame(document, originX, originY);

			if (set == null)
			{
				return;
			}

			DrawSetHeader(document, originX, originY, set);
			DrawCell(document, originX, originY, 0, set.Left, set.Width, set.Height);
			DrawCell(document, originX, originY, 1, set.Center, set.Width, set.Height);
			DrawCell(document, originX, originY, 2, set.Right, set.Width, set.Height);

			DrawRowTexts(document, originX, originY, 0, set.Left);
			DrawRowTexts(document, originX, originY, 1, set.Center);
			DrawRowTexts(document, originX, originY, 2, set.Right);
		}

		private void DrawSetFrame(DxfDocument document, double x, double y)
		{
			AddRect(document, x, y - GirderLayout.SetHeight, GirderLayout.SetWidth, GirderLayout.SetHeight, DxfLayerNames.FormLine);

			AddLine(document, x + GirderLayout.LabelColumnWidth, y, x + GirderLayout.LabelColumnWidth, y - GirderLayout.SetHeight, DxfLayerNames.FormLine);
			AddLine(document, x, y - 600.0, x + GirderLayout.SetWidth, y - 600.0, DxfLayerNames.FormLine);
			AddLine(document, x, y - 4400.0, x + GirderLayout.SetWidth, y - 4400.0, DxfLayerNames.FormLine);
			AddLine(document, x, y - 4100.0, x + GirderLayout.SetWidth, y - 4100.0, DxfLayerNames.FormLine);
			AddLine(document, x, y - 3800.0, x + GirderLayout.SetWidth, y - 3800.0, DxfLayerNames.FormLine);
			AddLine(document, x, y - 3500.0, x + GirderLayout.SetWidth, y - 3500.0, DxfLayerNames.FormLine);
			AddLine(document, x, y - 3200.0, x + GirderLayout.SetWidth, y - 3200.0, DxfLayerNames.FormLine);

			AddLine(document, x + 8370.0, y, x + 8370.0, y - GirderLayout.SetHeight, DxfLayerNames.FormLine);
			AddLine(document, x + 15540.0, y, x + 15540.0, y - GirderLayout.SetHeight, DxfLayerNames.FormLine);

			AddText(document, "부 재 명", x + 600.0, y - 250.0, 120.0, DxfLayerNames.FormText);
			AddText(document, "( B × H )", x + 6500.0, y - 250.0, 120.0, DxfLayerNames.FormText);
			AddText(document, "단    면", x + 600.0, y - 2500.0, 90.0, DxfLayerNames.FormText);
			AddText(document, "상 부 근", x + 600.0, y - 4200.0, 90.0, DxfLayerNames.FormText);
			AddText(document, "하 부 근", x + 600.0, y - 4500.0, 90.0, DxfLayerNames.FormText);
			AddText(document, "스 터 럽", x + 600.0, y - 4800.0, 90.0, DxfLayerNames.FormText);
			AddText(document, "표피철근(X)", x + 600.0, y - 5100.0, 90.0, DxfLayerNames.FormText);
		}

		private void DrawSetHeader(DxfDocument document, double x, double y, GirderSetModel set)
		{
			AddText(document, set.MemberName, x + 8200.0, y - 250.0, 120.0, DxfLayerNames.Text);
			AddText(document, string.Format("{0}×{1}", set.Width, set.Height), x + 8600.0, y - 450.0, 120.0, DxfLayerNames.Text);
		}

		private void DrawCell(DxfDocument document, double setX, double setY, int index, GirderCellModel cell, int width, int height)
		{
			var cellX = setX + GirderLayout.LabelColumnWidth + GirderLayout.CellWidth * index;
			var cellCenterX = cellX + GirderLayout.CellWidth / 2.0;

			if (cell == null || !cell.IsVisible)
			{
				return;
			}

			AddText(document, cell.Title, cellCenterX - 300.0, setY - 780.0, 90.0, DxfLayerNames.Text);

			if (cell.Force != null && cell.Force.Visible)
			{
				AddText(document, "M = " + cell.Force.M, cellX + 350.0, setY - 980.0, 80.0, DxfLayerNames.Text);
				AddText(document, "V = " + cell.Force.V, cellX + 1250.0, setY - 980.0, 80.0, DxfLayerNames.Text);
			}

			DrawSection(document, cellX, setY, GirderLayout.CellWidth, width, height, cell);
		}

		private void DrawSection(DxfDocument document, double cellX, double setY, double cellWidth, int width, int height, GirderCellModel cell)
		{
			var scale = GetSectionScale(width, height);
			var sectionWidth = width * scale;
			var sectionHeight = height * scale;

			var topY = setY - 1500.0;
			var leftX = cellX + (cellWidth - sectionWidth) / 2.0;
			var rightX = leftX + sectionWidth;
			var bottomY = topY - sectionHeight;

			AddLine(document, leftX - 100.0, topY, rightX + 100.0, topY, DxfLayerNames.RcGir);

			AddPolyline(document, new[]
			{
				new Vector2(leftX, topY - 50.0),
				new Vector2(leftX, bottomY),
				new Vector2(rightX, bottomY),
				new Vector2(rightX, topY - 50.0)
			}, false, DxfLayerNames.RcGir);

			DrawRebars(document, leftX, rightX, topY, bottomY, cell);
			DrawStirrups(document, leftX, rightX, topY, bottomY, cell);

			if (RebarPlacementService.NeedSkinMarkX(height))
			{
				DrawSkinMarkX(document, leftX, rightX, topY, bottomY);
			}
		}

		private void DrawRebars(DxfDocument document, double leftX, double rightX, double topY, double bottomY, GirderCellModel cell)
		{
			DrawRebarLayer(document, leftX, rightX, topY - 25.0, cell.Top1);
			DrawRebarLayer(document, leftX, rightX, topY - 60.0, cell.Top2);
			DrawRebarLayer(document, leftX, rightX, bottomY + 25.0, cell.Bottom1);
			DrawRebarLayer(document, leftX, rightX, bottomY + 60.0, cell.Bottom2);
		}

		private void DrawRebarLayer(DxfDocument document, double leftX, double rightX, double y, RebarLayerModel layer)
		{
			if (layer == null || layer.Count <= 0 || layer.Diameter <= 0)
			{
				return;
			}

			var radius = RebarPlacementService.GetRebarRadius(layer.Diameter);
			var minX = leftX + 25.0 + radius;
			var maxX = rightX - 25.0 - radius;
			var xs = RebarPlacementService.GetSymmetricPositions(minX, maxX, layer.Count);

			for (var i = 0; i < xs.Count; i++)
			{
				AddCircle(document, xs[i], y, radius, DxfLayerNames.Rebar);
			}
		}

		private void DrawStirrups(DxfDocument document, double leftX, double rightX, double topY, double bottomY, GirderCellModel cell)
		{
			if (cell == null || cell.Stirrup == null || cell.Stirrup.Legs <= 0)
			{
				return;
			}

			var x1 = leftX + 20.0;
			var x2 = rightX - 20.0;
			var y1 = topY - 40.0;
			var y2 = bottomY + 40.0;

			AddRect(document, x1, y1, x2 - x1, y2 - y1, DxfLayerNames.Rebar);

			var xs = RebarPlacementService.GetStirrupLegPositions(x1 + 25.0, x2 - 25.0, cell.Stirrup.Legs);

			for (var i = 0; i < xs.Count; i++)
			{
				AddLine(document, xs[i], y1, xs[i], y2, DxfLayerNames.Rebar);
			}
		}

		private void DrawSkinMarkX(DxfDocument document, double leftX, double rightX, double topY, double bottomY)
		{
			var cx = (leftX + rightX) / 2.0;
			var cy = (topY + bottomY) / 2.0;
			var s = 25.0;

			AddLine(document, cx - s, cy - s, cx + s, cy + s, DxfLayerNames.Rebar);
			AddLine(document, cx - s, cy + s, cx + s, cy - s, DxfLayerNames.Rebar);
		}

		private void DrawRowTexts(DxfDocument document, double setX, double setY, int index, GirderCellModel cell)
		{
			if (cell == null || !cell.IsVisible)
			{
				return;
			}

			var cellX = setX + GirderLayout.LabelColumnWidth + GirderLayout.CellWidth * index;
			var textX = cellX + 550.0;

			AddText(document, BuildRebarText(cell.Top1, cell.Top2), textX, setY - 4200.0, 90.0, DxfLayerNames.Text);
			AddText(document, BuildRebarText(cell.Bottom1, cell.Bottom2), textX, setY - 4500.0, 90.0, DxfLayerNames.Text);
			AddText(document, BuildStirrupText(cell.Stirrup), textX, setY - 4800.0, 90.0, DxfLayerNames.Text);
			AddText(document, cell.SkinRebarText, textX, setY - 5100.0, 90.0, DxfLayerNames.Text);
		}

		private string BuildRebarText(RebarLayerModel layer1, RebarLayerModel layer2)
		{
			var parts = new List<string>();

			if (layer1 != null && layer1.Count > 0 && layer1.Diameter > 0)
			{
				parts.Add(string.Format("{0}-HD{1}", layer1.Count, layer1.Diameter));
			}

			if (layer2 != null && layer2.Count > 0 && layer2.Diameter > 0)
			{
				parts.Add(string.Format("{0}-HD{1}", layer2.Count, layer2.Diameter));
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

		private double GetSectionScale(int width, int height)
		{
			var maxDrawWidth = 600.0;
			var maxDrawHeight = 900.0;

			var sx = maxDrawWidth / width;
			var sy = maxDrawHeight / height;

			return Math.Min(sx, sy);
		}

		private void AddRect(DxfDocument document, double x, double y, double width, double height, string layerName)
		{
			var polyline = new Polyline2D(new[]
			{
				new Vector2(x, y),
				new Vector2(x + width, y),
				new Vector2(x + width, y + height),
				new Vector2(x, y + height)
			}, true);

			polyline.Layer = document.Layers[layerName];
			document.Entities.Add(polyline);
		}

		private void AddPolyline(DxfDocument document, Vector2[] points, bool isClosed, string layerName)
		{
			var polyline = new Polyline2D(points, isClosed);
			polyline.Layer = document.Layers[layerName];
			document.Entities.Add(polyline);
		}

		private void AddLine(DxfDocument document, double x1, double y1, double x2, double y2, string layerName)
		{
			var line = new Line(new Vector3(x1, y1, 0.0), new Vector3(x2, y2, 0.0));
			line.Layer = document.Layers[layerName];
			document.Entities.Add(line);
		}

		private void AddText(DxfDocument document, string value, double x, double y, double height, string layerName)
		{
			var text = new TextEntity(value, new Vector3(x, y, 0.0), height);
			text.Layer = document.Layers[layerName];
			text.Style = document.TextStyles["맑은고딕"];
			document.Entities.Add(text);
		}

		private void AddCircle(DxfDocument document, double x, double y, double radius, string layerName)
		{
			var circle = new Circle(new Vector3(x, y, 0.0), radius);
			circle.Layer = document.Layers[layerName];
			document.Entities.Add(circle);
		}
	}
}