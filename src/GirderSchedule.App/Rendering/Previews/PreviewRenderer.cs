using GirderSchedule.App.Rendering.Formatting;
using GirderSchedule.App.Rendering.Models;
using GirderSchedule.Domain.Models;
using GirderSchedule.Domain.Models.Export;
using ScheduleTools.Wpf.Rendering;
using System.Collections.Generic;
using System.Windows.Controls;

namespace GirderSchedule.App.Rendering.Previews
{
    public sealed class PreviewRenderer
    {
        private readonly CanvasDrawer _drawer;
        private readonly RenderBrushSet _brushes;
        private readonly PreviewPageRenderer _pageRenderer;
        private readonly PreviewTableRenderer _tableRenderer;

        public PreviewRenderer()
            : this(new CanvasDrawer(), new RenderBrushSet())
        {
        }

        public PreviewRenderer(CanvasDrawer drawer, RenderBrushSet brushes)
        {
            _drawer = drawer ?? new CanvasDrawer();
            _brushes = brushes ?? new RenderBrushSet();
            _pageRenderer = new PreviewPageRenderer(_drawer, _brushes);
            _tableRenderer = new PreviewTableRenderer(_drawer, _brushes);
        }

        public void Draw(Canvas canvas, ScheduleExportPage? page, int pageNumber, int pageCount)
        {
            if (canvas == null)
            {
                return;
            }

            canvas.Width = SchedulePreviewLayout.CanvasWidth;
            canvas.Height = SchedulePreviewLayout.CanvasHeight;
            canvas.Children.Clear();

            _pageRenderer.Draw(canvas, page, pageNumber, pageCount);

            if (page == null || page.Sets == null || page.Sets.Count == 0)
            {
                DrawEmptyMessage(canvas);
                return;
            }

            DrawSets(canvas, page.Sets);
        }

        private void DrawEmptyMessage(Canvas canvas)
        {
            _drawer.DrawText(canvas, "미리보기할 부재가 없습니다. 오른쪽 프로젝트 목록에서 층과 부재를 체크하세요.", 310.0, 390.0, 18.0, _brushes.Disabled);
        }

        private void DrawSets(Canvas canvas, IList<ScheduleSet?> sets)
        {
            var count = sets.Count;

            if (count > SchedulePreviewLayout.MaxSetCount)
            {
                count = SchedulePreviewLayout.MaxSetCount;
            }

            for (var i = 0; i < count; i++)
            {
                DrawSet(canvas, sets[i], i);
            }
        }

        private void DrawSet(Canvas canvas, ScheduleSet? set, int index)
        {
            var row = index / SchedulePreviewLayout.ColumnCount;
            var column = index % SchedulePreviewLayout.ColumnCount;
            var x = SchedulePreviewLayout.GetCellX(column);
            var y = SchedulePreviewLayout.GetCellY(row);
            var showLabels = column == 0;

            _tableRenderer.DrawSetCell(canvas, set, x, y, SchedulePreviewLayout.CellWidth, SchedulePreviewLayout.CellHeight, row, column, showLabels);
        }
    }
}
