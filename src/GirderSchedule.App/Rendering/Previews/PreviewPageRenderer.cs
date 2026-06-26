using GirderSchedule.App.Rendering.Models;
using System.Windows.Controls;
using System.Windows.Media;

namespace GirderSchedule.App.Rendering.Previews
{
    public sealed class PreviewPageRenderer
    {
        private readonly CanvasDrawer _drawer;
        private readonly RenderBrushSet _brushes;

        public PreviewPageRenderer()
            : this(new CanvasDrawer(), new RenderBrushSet())
        {
        }

        public PreviewPageRenderer(CanvasDrawer drawer, RenderBrushSet brushes)
        {
            _drawer = drawer;
            _brushes = brushes;
        }

        public void Draw(Canvas canvas, int pageNumber, int pageCount)
        {
            if (canvas == null)
            {
                return;
            }

            _drawer.DrawRectangle(canvas, SchedulePreviewLayout.PageX, SchedulePreviewLayout.PageY, SchedulePreviewLayout.PageWidth, SchedulePreviewLayout.PageHeight, _brushes.Line, 1.0, Brushes.White);
            _drawer.DrawText(canvas, "보 일람표-" + pageNumber, SchedulePreviewLayout.PageX + 20.0, SchedulePreviewLayout.PageY + 14.0, 20.0, _brushes.Text);
            _drawer.DrawText(canvas, pageNumber + " / " + pageCount, SchedulePreviewLayout.PageX + SchedulePreviewLayout.PageWidth - 80.0, SchedulePreviewLayout.PageY + 18.0, 14.0, _brushes.Thin);
            _drawer.DrawLine(canvas, SchedulePreviewLayout.PageX, SchedulePreviewLayout.PageY + SchedulePreviewLayout.TitleHeight, SchedulePreviewLayout.PageX + SchedulePreviewLayout.PageWidth, SchedulePreviewLayout.PageY + SchedulePreviewLayout.TitleHeight, _brushes.Line, 1.0);
        }
    }
}