using System;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Dxf.Constants;
using GirderSchedule.Dxf.Geometry;
using GirderSchedule.Dxf.Layouts;
using GirderSchedule.Dxf.Styles;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class RebarDrawer
    {
        private readonly DxfDocument _document;
        private readonly DxfEntityStyler _styler;

        private BlockCircleInfo _rebarBlockCircleInfo;

        public RebarDrawer(DxfDocument document, DxfEntityStyler styler)
        {
            _document = document;
            _styler = styler;
        }

        public void Draw(SectionLayout layout, ScheduleItem item)
        {
            if (layout == null || item == null)
            {
                return;
            }

            DrawOuterStirrup(layout);
            DrawInternalStirrups(layout, item);
            DrawMainRebars(layout, item);
            DrawSkinRebarMarks(layout, item);
        }

        private void DrawOuterStirrup(SectionLayout layout)
        {
            var left = layout.SectionLeft + SectionRebarLayout.StirrupCover;
            var right = layout.SectionRight - SectionRebarLayout.StirrupCover;
            var top = layout.OuterTop - SectionRebarLayout.StirrupCover;
            var bottom = layout.OuterBottom + SectionRebarLayout.StirrupCover;

            AddClosedPolyline(left, top, right, top, right, bottom, left, bottom);
        }

        private void DrawInternalStirrups(SectionLayout layout, ScheduleItem item)
        {
            if (item.Stirrup == null || item.TopRebar == null || item.BottomRebar == null)
            {
                return;
            }

            var legs = item.Stirrup.Legs;

            if (legs <= 2)
            {
                return;
            }

            var topCount = GetFirstLayerCount(item.TopRebar);
            var bottomCount = GetFirstLayerCount(item.BottomRebar);

            if (topCount <= 2 || bottomCount <= 2)
            {
                return;
            }

            var innerCount = Math.Min(legs, bottomCount) - 2;

            if (innerCount <= 0)
            {
                return;
            }

            var topXs = GetLayerRebarXs(layout.BarStartX, layout.BarEndX, topCount);
            var bottomXs = GetLayerRebarXs(layout.BarStartX, layout.BarEndX, bottomCount);
            var usedBottom = new bool[bottomXs.Length];
            var stirrupTop = layout.OuterTop - SectionRebarLayout.StirrupCover;
            var stirrupBottom = layout.OuterBottom + SectionRebarLayout.StirrupCover;

            for (var i = 1; i <= innerCount; i++)
            {
                var topIndex = GetDistributedInnerIndex(topCount, innerCount, i);
                var topX = topXs[topIndex];
                var bottomIndex = FindNearestIndex(bottomXs, topX, usedBottom);

                if (bottomIndex < 0)
                {
                    continue;
                }

                usedBottom[bottomIndex] = true;

                var offset = GetInternalStirrupOffset(topIndex, topCount);
                AddOpenPolyline(topX + offset, stirrupTop, bottomXs[bottomIndex] + offset, stirrupBottom);
            }
        }

        private void DrawMainRebars(SectionLayout layout, ScheduleItem item)
        {
            var top1 = GetFirstLayerCount(item.TopRebar);
            var top2 = ClampCount(GetSecondLayerCount(item.TopRebar), top1);
            var bottom1 = GetFirstLayerCount(item.BottomRebar);
            var bottom2 = ClampCount(GetSecondLayerCount(item.BottomRebar), bottom1);

            DrawFirstLayerRebars(layout.BarStartX, layout.BarEndX, layout.TopBarY1, top1);
            DrawSecondLayerRebars(layout.BarStartX, layout.BarEndX, layout.TopBarY2, top1, top2);
            DrawFirstLayerRebars(layout.BarStartX, layout.BarEndX, layout.BottomBarY1, bottom1);
            DrawSecondLayerRebars(layout.BarStartX, layout.BarEndX, layout.BottomBarY2, bottom1, bottom2);
        }

        private void DrawFirstLayerRebars(double startX, double endX, double y, int count)
        {
            var xs = GetLayerRebarXs(startX, endX, count);

            for (var i = 0; i < xs.Length; i++)
            {
                AddBar(xs[i], y);
            }
        }

        private void DrawSecondLayerRebars(double startX, double endX, double y, int firstLayerCount, int secondLayerCount)
        {
            if (firstLayerCount <= 0 || secondLayerCount <= 0)
            {
                return;
            }

            if (firstLayerCount == 1)
            {
                AddBar(startX, y);
                return;
            }

            secondLayerCount = ClampCount(secondLayerCount, firstLayerCount);

            var xs = GetLayerRebarXs(startX, endX, firstLayerCount);
            var drawn = GetSecondLayerIndexFlags(firstLayerCount, secondLayerCount);

            for (var i = 0; i < xs.Length; i++)
            {
                if (drawn[i])
                {
                    AddBar(xs[i], y);
                }
            }
        }

        private bool[] GetSecondLayerIndexFlags(int firstLayerCount, int secondLayerCount)
        {
            var result = new bool[firstLayerCount];
            var leftCount = (secondLayerCount + 1) / 2;
            var rightCount = secondLayerCount / 2;

            for (var i = 0; i < leftCount && i < firstLayerCount; i++)
            {
                result[i] = true;
            }

            for (var i = 0; i < rightCount && i < firstLayerCount; i++)
            {
                result[firstLayerCount - 1 - i] = true;
            }

            return result;
        }

        private void DrawSkinRebarMarks(SectionLayout layout, ScheduleItem item)
        {
            if (item.Section == null || item.Section.Height < SectionRebarLayout.SkinRebarApplyHeight)
            {
                return;
            }

            var halfSize = SectionRebarLayout.SkinMarkSize / 2.0;
            var halfWidth = SectionRebarLayout.SkinMarkWidth / 2.0;
            var clearance = halfWidth / Math.Sqrt(2.0);
            var inset = halfSize + clearance;
            var left = layout.SectionLeft + SectionRebarLayout.StirrupCover + inset;
            var right = layout.SectionRight - SectionRebarLayout.StirrupCover - inset;
            var centerY = (layout.OuterTop + layout.OuterBottom) / 2.0;

            AddXMark(left, centerY, SectionRebarLayout.SkinMarkSize, SectionRebarLayout.SkinMarkWidth);
            AddXMark(right, centerY, SectionRebarLayout.SkinMarkSize, SectionRebarLayout.SkinMarkWidth);
        }

        private void AddXMark(double x, double y, double size, double width)
        {
            var half = size / 2.0;

            AddRebarPolylineWithWidth(width, x - half, y + half, x + half, y - half);
            AddRebarPolylineWithWidth(width, x - half, y - half, x + half, y + half);
        }

        private void AddBar(double x, double y)
        {
            if (_document.Blocks.Contains(DxfBlocks.RebarD19))
            {
                AddRebarBlock(x, y);
                return;
            }

            AddFilledCircle(x, y, SectionRebarLayout.RebarRadius);
        }

        private void AddRebarBlock(double x, double y)
        {
            var block = _document.Blocks[DxfBlocks.RebarD19];
            var blockCircle = GetRebarBlockCircleInfo(block);
            var blockRadius = blockCircle.Radius <= 0.0 ? SectionRebarLayout.RebarRadius : blockCircle.Radius;
            var insertScale = SectionRebarLayout.RebarRadius / blockRadius;
            var insertX = x - blockCircle.Center.X * insertScale;
            var insertY = y - blockCircle.Center.Y * insertScale;
            var insert = new Insert(block, DxfPointConverter.ToVector3(insertX, insertY));

            insert.Scale = new Vector3(insertScale, insertScale, insertScale);
            insert.Rotation = 0.0;

            _styler.Apply(insert, DxfStyleRole.Rebar);
            _document.Entities.Add(insert);
        }

        private BlockCircleInfo GetRebarBlockCircleInfo(Block block)
        {
            if (_rebarBlockCircleInfo != null)
            {
                return _rebarBlockCircleInfo;
            }

            _rebarBlockCircleInfo = GetBlockCircleInfo(block);
            return _rebarBlockCircleInfo;
        }

        private BlockCircleInfo GetBlockCircleInfo(Block block)
        {
            foreach (var entity in block.Entities)
            {
                var circle = entity as Circle;

                if (circle != null)
                {
                    return new BlockCircleInfo(new Vector2(circle.Center.X, circle.Center.Y), circle.Radius);
                }
            }

            return new BlockCircleInfo(Vector2.Zero, SectionRebarLayout.RebarRadius);
        }

        private void AddFilledCircle(double x, double y, double radius)
        {
            var circle = new Circle(DxfPointConverter.ToVector3(x, y), radius);
            var hatch = new Hatch(HatchPattern.Solid, false);
            var boundary = new HatchBoundaryPath(new EntityObject[] { circle });

            _styler.Apply(circle, DxfStyleRole.Rebar);
            _styler.Apply(hatch, DxfStyleRole.Rebar);

            hatch.BoundaryPaths.Add(boundary);

            _document.Entities.Add(hatch);
            _document.Entities.Add(circle);
        }

        private void AddOpenPolyline(params double[] values)
        {
            AddPolyline(false, 0.0, values);
        }

        private void AddClosedPolyline(params double[] values)
        {
            AddPolyline(true, 0.0, values);
        }

        private void AddRebarPolylineWithWidth(double width, params double[] values)
        {
            AddPolyline(false, width, values);
        }

        private void AddPolyline(bool isClosed, double width, params double[] values)
        {
            if (values == null || values.Length == 0 || values.Length % 2 != 0)
            {
                return;
            }

            var polyline = new Polyline2D();

            for (var i = 0; i < values.Length; i += 2)
            {
                var vertex = new Polyline2DVertex(DxfPointConverter.ToVector2(values[i], values[i + 1]));
                vertex.StartWidth = width;
                vertex.EndWidth = width;
                polyline.Vertexes.Add(vertex);
            }

            polyline.IsClosed = isClosed;
            _styler.Apply(polyline, DxfStyleRole.Rebar);

            if (width > 0.0)
            {
                for (var i = 0; i < polyline.Vertexes.Count; i++)
                {
                    polyline.Vertexes[i].StartWidth = width;
                    polyline.Vertexes[i].EndWidth = width;
                }
            }

            _document.Entities.Add(polyline);
        }

        private int GetDistributedInnerIndex(int count, int innerCount, int oneBasedIndex)
        {
            var rawIndex = (count - 1) * oneBasedIndex / (double)(innerCount + 1);
            var index = (int)Math.Floor(rawIndex + 0.5 - 0.000001);

            if (index <= 0)
            {
                return 1;
            }

            if (index >= count - 1)
            {
                return count - 2;
            }

            return index;
        }

        private double GetInternalStirrupOffset(int index, int count)
        {
            if (count <= 1)
            {
                return 0.0;
            }

            return index < count / 2.0 ? -SectionRebarLayout.RebarRadius : SectionRebarLayout.RebarRadius;
        }

        private int GetFirstLayerCount(RebarSet rebar)
        {
            if (rebar == null || rebar.FirstLayer == null || rebar.FirstLayer.Count < 0)
            {
                return 0;
            }

            return rebar.FirstLayer.Count;
        }

        private int GetSecondLayerCount(RebarSet rebar)
        {
            if (rebar == null || rebar.SecondLayer == null || rebar.SecondLayer.Count < 0)
            {
                return 0;
            }

            return rebar.SecondLayer.Count;
        }

        private int ClampCount(int count, int max)
        {
            if (count < 0)
            {
                return 0;
            }

            return count > max ? max : count;
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

            var result = new double[count];
            var spacing = (endX - startX) / (count - 1);

            for (var i = 0; i < count; i++)
            {
                result[i] = startX + spacing * i;
            }

            return result;
        }

        private int FindNearestIndex(double[] values, double targetX, bool[] used)
        {
            var index = -1;
            var distance = double.MaxValue;

            for (var i = 0; i < values.Length; i++)
            {
                if (used != null && i < used.Length && used[i])
                {
                    continue;
                }

                var current = Math.Abs(values[i] - targetX);

                if (current >= distance)
                {
                    continue;
                }

                index = i;
                distance = current;
            }

            return index;
        }

        private sealed class BlockCircleInfo
        {
            public Vector2 Center { get; private set; }
            public double Radius { get; private set; }

            public BlockCircleInfo(Vector2 center, double radius)
            {
                Center = center;
                Radius = radius;
            }
        }
    }
}
