using System;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Dxf.Common;

namespace GirderSchedule.Dxf.Girder
{
    public sealed class RebarDrawer
    {
        private const double StirrupCover = 40.0;
        private const double RebarRadius = 12.5;

        private readonly DxfDocument _document;
        private readonly DxfPointConverter _pointConverter;

        public RebarDrawer(DxfDocument document, DxfPointConverter pointConverter)
        {
            _document = document;
            _pointConverter = pointConverter;
        }

        public void Draw(SectionDxfLayout layout, ScheduleItem item)
        {
            DrawOuterStirrup(layout);
            DrawInternalStirrups(layout, item);
            DrawMainRebars(layout, item);
        }

        private void DrawOuterStirrup(SectionDxfLayout layout)
        {
            var left = layout.SectionLeft + StirrupCover;
            var right = layout.SectionRight - StirrupCover;
            var top = layout.OuterTop - StirrupCover;
            var bottom = layout.OuterBottom + StirrupCover;

            AddClosedPolyline(
                left, top,
                right, top,
                right, bottom,
                left, bottom);
        }

        private void DrawInternalStirrups(SectionDxfLayout layout, ScheduleItem item)
        {
            var legs = item.Stirrup.Legs;

            if (legs <= 2)
            {
                return;
            }

            var topCount = item.TopRebar.FirstLayer.Count;
            var bottomCount = item.BottomRebar.FirstLayer.Count;

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

            var stirrupTop = layout.OuterTop - StirrupCover;
            var stirrupBottom = layout.OuterBottom + StirrupCover;

            for (var i = 1; i <= innerCount; i++)
            {
                var rawIndex = (topCount - 1) * i / (double)(innerCount + 1);
                var topIndex = (int)Math.Floor(rawIndex + 0.5 - 0.000001);

                if (topIndex <= 0)
                {
                    topIndex = 1;
                }

                if (topIndex >= topCount - 1)
                {
                    topIndex = topCount - 2;
                }

                var topX = topXs[topIndex];
                var bottomIndex = FindNearestIndex(bottomXs, topX, usedBottom);

                if (bottomIndex < 0)
                {
                    continue;
                }

                usedBottom[bottomIndex] = true;

                var bottomX = bottomXs[bottomIndex];
                var offset = GetInternalStirrupOffset(topIndex, topCount);

                AddOpenPolyline(
                    topX + offset, stirrupTop,
                    bottomX + offset, stirrupBottom);
            }
        }

        private double GetInternalStirrupOffset(int index, int count)
        {
            if (count <= 1)
            {
                return 0.0;
            }

            return index < count / 2.0 ? -RebarRadius : RebarRadius;
        }

        private void DrawMainRebars(SectionDxfLayout layout, ScheduleItem item)
        {
            var top1 = SafeCount(item.TopRebar.FirstLayer.Count);
            var top2 = ClampCount(item.TopRebar.SecondLayer.Count, top1);
            var bottom1 = SafeCount(item.BottomRebar.FirstLayer.Count);
            var bottom2 = ClampCount(item.BottomRebar.SecondLayer.Count, bottom1);

            DrawFirstLayerRebars(layout.BarStartX, layout.BarEndX, layout.TopBarY1, top1);
            DrawSecondLayerRebars(layout.BarStartX, layout.BarEndX, layout.TopBarY2, top1, top2);

            DrawFirstLayerRebars(layout.BarStartX, layout.BarEndX, layout.BottomBarY1, bottom1);
            DrawSecondLayerRebars(layout.BarStartX, layout.BarEndX, layout.BottomBarY2, bottom1, bottom2);
        }

        private void DrawFirstLayerRebars(double startX, double endX, double y, int count)
        {
            if (count <= 0)
            {
                return;
            }

            if (count == 1)
            {
                AddBar(startX, y);
                return;
            }

            var spacing = (endX - startX) / (count - 1);

            for (var i = 0; i < count; i++)
            {
                AddBar(startX + spacing * i, y);
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

            if (secondLayerCount > firstLayerCount)
            {
                secondLayerCount = firstLayerCount;
            }

            var spacing = (endX - startX) / (firstLayerCount - 1);
            var leftCount = (secondLayerCount + 1) / 2;
            var rightCount = secondLayerCount / 2;
            var drawn = new bool[firstLayerCount];

            for (var i = 0; i < leftCount && i < firstLayerCount; i++)
            {
                drawn[i] = true;
            }

            for (var i = 0; i < rightCount && i < firstLayerCount; i++)
            {
                drawn[firstLayerCount - 1 - i] = true;
            }

            for (var i = 0; i < firstLayerCount; i++)
            {
                if (!drawn[i])
                {
                    continue;
                }

                AddBar(startX + spacing * i, y);
            }
        }

        private void AddBar(double x, double y)
        {
            if (_document.Blocks.Contains(DxfBlocks.RebarD19))
            {
                var block = _document.Blocks[DxfBlocks.RebarD19];
                var blockCircle = GetBlockCircleInfo(block);
                var blockRadius = blockCircle.Radius <= 0.0 ? RebarRadius : blockCircle.Radius;
                var insertScale = RebarRadius / blockRadius;

                var insertX = x - blockCircle.Center.X * insertScale;
                var insertY = y - blockCircle.Center.Y * insertScale;

                var insert = new Insert(block, _pointConverter.ToVector3(insertX, insertY));
                insert.Scale = new Vector3(insertScale, insertScale, insertScale);
                insert.Rotation = 0.0;

                DxfEntityStyle.ApplyRebar(insert, _document);
                _document.Entities.Add(insert);
                return;
            }

            AddFilledCircle(x, y, RebarRadius);
        }

        private BlockCircleInfo GetBlockCircleInfo(Block block)
        {
            foreach (var entity in block.Entities)
            {
                var circle = entity as Circle;

                if (circle == null)
                {
                    continue;
                }

                return new BlockCircleInfo(new Vector2(circle.Center.X, circle.Center.Y), circle.Radius);
            }

            return new BlockCircleInfo(Vector2.Zero, RebarRadius);
        }

        private void AddFilledCircle(double x, double y, double radius)
        {
            var circle = new Circle(_pointConverter.ToVector3(x, y), radius);
            DxfEntityStyle.ApplyRebar(circle, _document);

            var hatch = new Hatch(HatchPattern.Solid, false);
            DxfEntityStyle.ApplyRebar(hatch, _document);

            var boundary = new HatchBoundaryPath(new EntityObject[] { circle });
            hatch.BoundaryPaths.Add(boundary);

            _document.Entities.Add(hatch);
            _document.Entities.Add(circle);
        }

        private void AddOpenPolyline(params double[] values)
        {
            AddPolyline(false, values);
        }

        private void AddClosedPolyline(params double[] values)
        {
            AddPolyline(true, values);
        }

        private void AddPolyline(bool isClosed, params double[] values)
        {
            if (values == null || values.Length == 0)
            {
                return;
            }

            if (values.Length % 2 != 0)
            {
                return;
            }

            var polyline = new Polyline2D();

            for (var i = 0; i < values.Length; i += 2)
            {
                polyline.Vertexes.Add(new Polyline2DVertex(_pointConverter.ToVector2(values[i], values[i + 1])));
            }

            polyline.IsClosed = isClosed;

            DxfEntityStyle.ApplyRebar(polyline, _document);
            _document.Entities.Add(polyline);
        }

        private int SafeCount(int count)
        {
            return count < 0 ? 0 : count;
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