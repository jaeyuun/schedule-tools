using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using GirderSchedule.App.Preview.Models;
using GirderSchedule.App.Preview.Formatting;
using GirderSchedule.App.Preview.Rendering;
using GirderSchedule.Domain.Girder.Models;

namespace GirderSchedule.App.Preview
{
    public sealed class SchedulePreviewRenderer
    {
        private readonly PreviewCanvasDrawer _drawer;
        private readonly PreviewBrushSet _brushes;
        private readonly SchedulePreviewTableRenderer _tableRenderer;

        public SchedulePreviewRenderer()
            : this(new PreviewCanvasDrawer(), new PreviewBrushSet())
        {
        }

        public SchedulePreviewRenderer(PreviewCanvasDrawer drawer, PreviewBrushSet brushes)
        {
            _drawer = drawer;
            _brushes = brushes;
            _tableRenderer = new SchedulePreviewTableRenderer(_drawer, _brushes, new PreviewSectionRenderer(), new SchedulePreviewTextFormatter());
        }

        public void Draw(Canvas canvas, IList<ScheduleSet> sets, int pageNumber, int pageCount)
        {
            if (canvas == null)
            {
                return;
            }

            canvas.Width = SchedulePreviewLayout.CanvasWidth;
            canvas.Height = SchedulePreviewLayout.CanvasHeight;

            DrawPage(canvas, pageNumber, pageCount);

            if (sets == null || sets.Count == 0)
            {
                _drawer.DrawText(canvas, "미리보기할 부재가 없습니다. 오른쪽 프로젝트 목록에서 층과 부재를 체크하세요.", 310.0, 390.0, 18.0, _brushes.Disabled);
                return;
            }

            DrawSets(canvas, sets);
        }

        private void DrawPage(Canvas canvas, int pageNumber, int pageCount)
        {
            _drawer.DrawRectangle(canvas, SchedulePreviewLayout.PageX, SchedulePreviewLayout.PageY, SchedulePreviewLayout.PageWidth, SchedulePreviewLayout.PageHeight, _brushes.Line, 1.0, Brushes.White);
            _drawer.DrawText(canvas, "보 일람표-" + pageNumber, SchedulePreviewLayout.PageX + 20.0, SchedulePreviewLayout.PageY + 14.0, 20.0, _brushes.Text);
            _drawer.DrawText(canvas, pageNumber + " / " + pageCount, SchedulePreviewLayout.PageX + SchedulePreviewLayout.PageWidth - 80.0, SchedulePreviewLayout.PageY + 18.0, 14.0, _brushes.Thin);
            _drawer.DrawLine(canvas, SchedulePreviewLayout.PageX, SchedulePreviewLayout.PageY + SchedulePreviewLayout.TitleHeight, SchedulePreviewLayout.PageX + SchedulePreviewLayout.PageWidth, SchedulePreviewLayout.PageY + SchedulePreviewLayout.TitleHeight, _brushes.Line, 1.0);
        }

        private void DrawSets(Canvas canvas, IList<ScheduleSet> sets)
        {
            var count = sets.Count;

            if (count > SchedulePreviewLayout.MaxSetCount)
            {
                count = SchedulePreviewLayout.MaxSetCount;
            }

            for (var i = 0; i < count; i++)
            {
                var row = i / SchedulePreviewLayout.ColumnCount;
                var column = i % SchedulePreviewLayout.ColumnCount;
                var x = SchedulePreviewLayout.GetCellX(column);
                var y = SchedulePreviewLayout.GetCellY(row);

                var showLabels = column == 0;
                _tableRenderer.DrawSetCell(canvas, sets[i], x, y, SchedulePreviewLayout.CellWidth, SchedulePreviewLayout.CellHeight, row, column, showLabels);
            }
        }
    }
}
