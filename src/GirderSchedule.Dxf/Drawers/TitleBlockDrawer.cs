using GirderSchedule.Domain.Models;
using GirderSchedule.Dxf.Constants;
using GirderSchedule.Dxf.Drawers.Common;
using GirderSchedule.Dxf.Layouts;
using GirderSchedule.Dxf.Styles;
using netDxf;

namespace GirderSchedule.Dxf.Drawers
{
    public sealed class TitleBlockDrawer
    {
        private readonly DxfDocument _document;
        private readonly TextDrawer _textDrawer;
        private readonly DxfEntityDrawer _entityDrawer;

        public TitleBlockDrawer(DxfDocument document, TextDrawer textDrawer, DxfEntityDrawer entityDrawer)
        {
            _document = document;
            _textDrawer = textDrawer;
            _entityDrawer = entityDrawer;
        }

        public void Draw(ScheduleExportPage page, double pageBaseX, double pageBaseY)
        {
            DrawBlock(pageBaseX, pageBaseY);
            DrawTitle(page, pageBaseX, pageBaseY);
            DrawDrawingName(page, pageBaseX, pageBaseY);
        }

        private void DrawBlock(double x, double y)
        {
            if (!_document.Blocks.Contains(DxfBlocks.TitleBlock))
            {
                return;
            }

            _entityDrawer.AddUnstyledInsert(_document.Blocks[DxfBlocks.TitleBlock], x, y, TitleBlockLayout.BlockScale, 0.0);
        }

        private void DrawTitle(ScheduleExportPage page, double pageBaseX, double pageBaseY)
        {
            if (page == null)
            {
                return;
            }

            DrawTitleCircle(pageBaseX, pageBaseY);
            DrawTitleLine(pageBaseX, pageBaseY);

            _textDrawer.DrawFormText("1", pageBaseX + TitleBlockLayout.TitleNumberX, pageBaseY + TitleBlockLayout.TitleNumberY, TitleBlockLayout.TitleNumberHeight);

            var title = string.IsNullOrWhiteSpace(page.SheetTitle) ? "보 일람표-1" : page.SheetTitle;
            _textDrawer.DrawFormText(title, pageBaseX + TitleBlockLayout.TitleTextX, pageBaseY + TitleBlockLayout.TitleTextY, TitleBlockLayout.TitleTextHeight);
        }

        private void DrawTitleCircle(double pageBaseX, double pageBaseY)
        {
            _entityDrawer.AddCircle(DxfStyleRole.TitleText, pageBaseX + TitleBlockLayout.TitleCircleCenterX, pageBaseY + TitleBlockLayout.TitleCircleCenterY, TitleBlockLayout.TitleCircleRadius);
        }

        private void DrawTitleLine(double pageBaseX, double pageBaseY)
        {
            _entityDrawer.AddLine(DxfStyleRole.TitleText, pageBaseX + TitleBlockLayout.TitleLineStartX, pageBaseY + TitleBlockLayout.TitleLineY, pageBaseX + TitleBlockLayout.TitleLineEndX, pageBaseY + TitleBlockLayout.TitleLineY);
        }

        private void DrawDrawingName(ScheduleExportPage page, double pageBaseX, double pageBaseY)
        {
            if (page == null)
            {
                return;
            }

            var drawingName = string.IsNullOrWhiteSpace(page.SheetTitleName) ? "보 일람표-1" : page.SheetTitleName;
            _textDrawer.DrawValueText(drawingName, pageBaseX + TitleBlockLayout.DrawingNameTextX, pageBaseY + TitleBlockLayout.DrawingNameTextY, TitleBlockLayout.DrawingNameTextHeight);
        }
    }
}
