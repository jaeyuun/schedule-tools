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
			EnsureDimensionStyles(document);

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

		private void EnsureDimensionStyles(DxfDocument document)
		{
			if (document.DimensionStyles.Contains("Sejin-dim3"))
			{
				return;
			}

			var style = new DimensionStyle("Sejin-dim3");
			style.TextHeight = 90.0;
			style.ArrowSize = 45.0;

			document.DimensionStyles.Add(style);
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

		private void DrawCell(DxfDocument document, double setX, double setY, int index, GirderCellModel cell, double width, double height)
		{
			var cellX = setX + GirderLayout.LabelColumnWidth + GirderLayout.CellWidth * index;
			var cellCenterX = cellX + GirderLayout.CellWidth / 2.0;

			if (cell == null)
			{
				return;
			}

			if (!cell.IsVisible)
			{
				DrawNoneCell(document, cellX, setY);
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

		private void DrawNoneCell(DxfDocument document, double cellX, double setY)
		{
			var x1 = cellX + 150.0;
			var x2 = cellX + GirderLayout.CellWidth - 150.0;
			var y1 = setY - 650.0;
			var y2 = setY - 3150.0;

			AddLine(document, x1, y1, x2, y2, DxfLayerNames.FormLine);
		}

		private void DrawSection(DxfDocument document, double cellX, double setY, double cellWidth, double width, double height, GirderCellModel cell)
		{
			var layout = CreateSectionDxfLayout(cellX, setY, cellWidth, width, height);

			DrawGirderPreviewShape(document, layout);
			DrawRebars(document, layout, cell);
			DrawStirrups(document, layout, cell);
			DrawSectionDimensions(document, layout);

			if (RebarPlacementService.NeedSkinMarkX(height))
			{
				DrawSkinMarkX(document, layout.SectionLeft, layout.SectionRight, layout.SectionTop, layout.SectionBottom);
			}
		}

		private SectionDxfLayout CreateSectionDxfLayout(double cellX, double setY, double cellWidth, double width, double height)
		{
			var scale = GetSectionScale(width, height);

			var sectionWidth = width * scale;
			var sectionHeight = height * scale;

			const double topGap = 30.0;
			const double sideDrop = 150.0;
			const double sideWing = 175.0;
			const double sideOffset = 25.0;
			const double bottomOffset = 30.0;
			const double rebarCover = 25.0;
			const double secondLayerGap = 40.0;

			var outerWidth = sectionWidth + sideOffset * 2.0;
			var outerHeight = topGap + sectionHeight + bottomOffset;
			var totalShapeWidth = outerWidth + sideWing * 2.0;

			var shapeLeft = cellX + (cellWidth - totalShapeWidth) / 2.0;
			var outerLeft = shapeLeft + sideWing;
			var outerRight = outerLeft + outerWidth;

			var outerTop = setY - 1450.0;
			var outerShelfY = outerTop - sideDrop;
			var outerBottom = outerTop - outerHeight;

			var sectionLeft = outerLeft + sideOffset;
			var sectionRight = sectionLeft + sectionWidth;
			var sectionTop = outerTop - topGap;
			var sectionBottom = sectionTop - sectionHeight;

			var barStartX = sectionLeft + rebarCover;
			var barEndX = sectionRight - rebarCover;

			var topBarY1 = sectionTop - rebarCover;
			var topBarY2 = topBarY1 - secondLayerGap;

			var bottomBarY1 = sectionBottom + rebarCover;
			var bottomBarY2 = bottomBarY1 + secondLayerGap;

			return new SectionDxfLayout
			{
				OuterLeft = outerLeft,
				OuterRight = outerRight,
				OuterTop = outerTop,
				OuterShelfY = outerShelfY,
				OuterBottom = outerBottom,
				SectionLeft = sectionLeft,
				SectionRight = sectionRight,
				SectionTop = sectionTop,
				SectionBottom = sectionBottom,
				BarStartX = barStartX,
				BarEndX = barEndX,
				TopBarY1 = topBarY1,
				TopBarY2 = topBarY2,
				BottomBarY1 = bottomBarY1,
				BottomBarY2 = bottomBarY2,
				SideWing = sideWing,
				RebarCover = rebarCover
			};
		}

		private void DrawGirderPreviewShape(DxfDocument document, SectionDxfLayout layout)
		{
			AddLine(
				document,
				layout.OuterLeft - layout.SideWing,
				layout.OuterTop,
				layout.OuterRight + layout.SideWing,
				layout.OuterTop,
				DxfLayerNames.RcGir);

			AddPolyline(document, new[]
			{
				new Vector2(layout.OuterLeft - layout.SideWing, layout.OuterShelfY),
				new Vector2(layout.OuterLeft, layout.OuterShelfY),
				new Vector2(layout.OuterLeft, layout.OuterBottom),
				new Vector2(layout.OuterRight, layout.OuterBottom),
				new Vector2(layout.OuterRight, layout.OuterShelfY),
				new Vector2(layout.OuterRight + layout.SideWing, layout.OuterShelfY)
			}, false, DxfLayerNames.RcGir);

			AddRect(
				document,
				layout.SectionLeft,
				layout.SectionBottom,
				layout.SectionRight - layout.SectionLeft,
				layout.SectionTop - layout.SectionBottom,
				DxfLayerNames.Rebar);
		}

		private void DrawRebars(DxfDocument document, SectionDxfLayout layout, GirderCellModel cell)
		{
			DrawFirstLayerRebars(document, layout.BarStartX, layout.BarEndX, layout.TopBarY1, cell.Top1);
			DrawSecondLayerRebars(document, layout.BarStartX, layout.BarEndX, layout.TopBarY2, cell.Top1);

			DrawFirstLayerRebars(document, layout.BarStartX, layout.BarEndX, layout.BottomBarY1, cell.Bottom1);
			DrawSecondLayerRebars(document, layout.BarStartX, layout.BarEndX, layout.BottomBarY2, cell.Bottom1);
		}

		private void DrawFirstLayerRebars(DxfDocument document, double minX, double maxX, double y, RebarLayerModel layer)
		{
			if (layer == null || layer.FirstCount <= 0 || layer.Diameter <= 0)
			{
				return;
			}

			var radius = RebarPlacementService.GetRebarRadius(layer.Diameter);
			var xs = RebarPlacementService.GetFirstLayerPositions(minX, maxX, layer.FirstCount);

			for (var i = 0; i < xs.Count; i++)
			{
				AddCircle(document, xs[i], y, radius, DxfLayerNames.Rebar);
			}
		}

		private void DrawSecondLayerRebars(DxfDocument document, double minX, double maxX, double y, RebarLayerModel layer)
		{
			if (layer == null || layer.FirstCount <= 0 || layer.SecondCount <= 0 || layer.Diameter <= 0)
			{
				return;
			}

			var radius = RebarPlacementService.GetRebarRadius(layer.Diameter);
			var xs = RebarPlacementService.GetSecondLayerPositions(minX, maxX, layer.FirstCount, layer.SecondCount);

			for (var i = 0; i < xs.Count; i++)
			{
				AddCircle(document, xs[i], y, radius, DxfLayerNames.Rebar);
			}
		}

		private void DrawStirrups(DxfDocument document, SectionDxfLayout layout, GirderCellModel cell)
		{
			if (cell == null || cell.Stirrup == null || cell.Stirrup.Legs <= 0)
			{
				return;
			}

			if (cell.Stirrup.Legs <= 2)
			{
				return;
			}

			if (cell.Top1 == null || cell.Bottom1 == null)
			{
				return;
			}

			if (cell.Top1.FirstCount <= 2 || cell.Bottom1.FirstCount <= 2)
			{
				return;
			}

			var availableLegs = Math.Min(cell.Stirrup.Legs, cell.Bottom1.FirstCount);
			var innerCount = availableLegs - 2;

			if (innerCount <= 0)
			{
				return;
			}

			var topXs = GetLayerRebarXs(layout.BarStartX, layout.BarEndX, cell.Top1.FirstCount);
			var bottomXs = GetLayerRebarXs(layout.BarStartX, layout.BarEndX, cell.Bottom1.FirstCount);
			var usedBottom = new bool[cell.Bottom1.FirstCount];

			for (var i = 1; i <= innerCount; i++)
			{
				var rawIndex = (cell.Top1.FirstCount - 1) * i / (double)(availableLegs - 1);
				var topIndex = (int)Math.Floor(rawIndex + 0.5 - 0.000001);

				if (topIndex <= 0)
				{
					topIndex = 1;
				}

				if (topIndex >= cell.Top1.FirstCount - 1)
				{
					topIndex = cell.Top1.FirstCount - 2;
				}

				var topX = topXs[topIndex];
				var direction = topIndex < cell.Top1.FirstCount / 2.0 ? -1.0 : 1.0;

				var bottomIndex = FindNearestIndex(bottomXs, topX, usedBottom);

				if (bottomIndex < 0)
				{
					continue;
				}

				usedBottom[bottomIndex] = true;

				var bottomX = bottomXs[bottomIndex];

				var topLineX = topX + direction * 9.5;
				var bottomLineX = bottomX + direction * 9.5;

				topLineX = Clamp(topLineX, layout.BarStartX, layout.BarEndX);
				bottomLineX = Clamp(bottomLineX, layout.BarStartX, layout.BarEndX);

				AddLine(document, topLineX, layout.SectionTop, bottomLineX, layout.SectionBottom, DxfLayerNames.Rebar);
			}
		}

		private void DrawSectionDimensions(DxfDocument document, SectionDxfLayout layout)
		{
			AddAlignedDimension(
				document,
				layout.SectionLeft,
				layout.SectionTop,
				layout.SectionRight,
				layout.SectionTop,
				220.0,
				DxfLayerNames.Dim);

			AddAlignedDimension(
				document,
				layout.SectionLeft,
				layout.SectionTop,
				layout.SectionLeft,
				layout.SectionBottom,
				-220.0,
				DxfLayerNames.Dim);
		}

		private double[] GetLayerRebarXs(double startX, double endX, int count)
		{
			if (count <= 0)
			{
				return new double[0];
			}

			if (count == 1)
			{
				return new[] { startX };
			}

			var values = new double[count];
			var spacing = (endX - startX) / (count - 1);

			for (var i = 0; i < count; i++)
			{
				values[i] = startX + spacing * i;
			}

			return values;
		}

		private static int FindNearestIndex(double[] values, double targetX, bool[] used)
		{
			var index = -1;
			var distance = double.MaxValue;

			for (var i = 0; i < values.Length; i++)
			{
				if (used != null && i < used.Length && used[i])
				{
					continue;
				}

				var currentDistance = Math.Abs(values[i] - targetX);

				if (currentDistance >= distance)
				{
					continue;
				}

				index = i;
				distance = currentDistance;
			}

			return index;
		}

		private static double Clamp(double value, double min, double max)
		{
			if (value < min)
			{
				return min;
			}

			if (value > max)
			{
				return max;
			}

			return value;
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

		private double GetSectionScale(double width, double height)
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

		private void AddAlignedDimension(DxfDocument document, double x1, double y1, double x2, double y2, double offset, string layerName)
		{
			var dimension = new AlignedDimension(
				new Vector2(x1, y1),
				new Vector2(x2, y2),
				offset);

			dimension.Layer = document.Layers[layerName];

			if (document.DimensionStyles.Contains("Sejin-dim3"))
			{
				dimension.Style = document.DimensionStyles["Sejin-dim3"];
			}

			document.Entities.Add(dimension);
		}

		private class SectionDxfLayout
		{
			public double OuterLeft { get; set; }
			public double OuterRight { get; set; }
			public double OuterTop { get; set; }
			public double OuterShelfY { get; set; }
			public double OuterBottom { get; set; }

			public double SectionLeft { get; set; }
			public double SectionRight { get; set; }
			public double SectionTop { get; set; }
			public double SectionBottom { get; set; }

			public double BarStartX { get; set; }
			public double BarEndX { get; set; }

			public double TopBarY1 { get; set; }
			public double TopBarY2 { get; set; }
			public double BottomBarY1 { get; set; }
			public double BottomBarY2 { get; set; }

			public double SideWing { get; set; }
			public double RebarCover { get; set; }
		}
	}
}