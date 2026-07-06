using GirderSchedule.Domain.Models.Export;
using GirderSchedule.Domain.Models.Settings;
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
        private readonly DxfStyleNameSet _styleNames;

        public TitleBlockDrawer(DxfDocument document, TextDrawer textDrawer, DxfEntityDrawer entityDrawer, DxfStyleNameSet styleNames)
        {
            _document = document;
            _textDrawer = textDrawer;
            _entityDrawer = entityDrawer;
            _styleNames = styleNames ?? new DxfStyleNameSet();
        }

        public void Draw(ScheduleExportPage page, double pageBaseX, double pageBaseY)
        {
            DrawBlock(pageBaseX, pageBaseY);
            DrawTitle(page, pageBaseX, pageBaseY);
            DrawDrawingName(page, pageBaseX, pageBaseY);
        }

        private void DrawBlock(double x, double y)
        {
            var blockName = string.IsNullOrWhiteSpace(_styleNames.Block) ? DxfBlocks.TitleBlock : _styleNames.Block;

            if (!_document.Blocks.Contains(blockName))
            {
                return;
            }

            _entityDrawer.AddUnstyledInsert(_document.Blocks[blockName], x, y, TitleBlockLayout.BlockScale, 0.0);
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

            var title = string.IsNullOrWhiteSpace(page.Title) ? "보 일람표-1" : page.Title;
            _textDrawer.DrawFormText(title, pageBaseX + TitleBlockLayout.TitleTextX, pageBaseY + TitleBlockLayout.TitleTextY, TitleBlockLayout.TitleTextHeight);
        }

        private void DrawTitleCircle(double pageBaseX, double pageBaseY)
        {
            _entityDrawer.AddCircle(DxfLayerRole.FormText, pageBaseX + TitleBlockLayout.TitleCircleCenterX, pageBaseY + TitleBlockLayout.TitleCircleCenterY, TitleBlockLayout.TitleCircleRadius);
        }

        private void DrawTitleLine(double pageBaseX, double pageBaseY)
        {
            _entityDrawer.AddLine(DxfLayerRole.FormText, pageBaseX + TitleBlockLayout.TitleLineStartX, pageBaseY + TitleBlockLayout.TitleLineY, pageBaseX + TitleBlockLayout.TitleLineEndX, pageBaseY + TitleBlockLayout.TitleLineY);
        }

        private void DrawDrawingName(ScheduleExportPage page, double pageBaseX, double pageBaseY)
        {
            if (page == null)
            {
                return;
            }

            var drawingName = string.IsNullOrWhiteSpace(page.Title) ? "보 일람표-1" : page.Title;
            _textDrawer.DrawValueText(drawingName, pageBaseX + TitleBlockLayout.DrawingNameTextX, pageBaseY + TitleBlockLayout.DrawingNameTextY, TitleBlockLayout.DrawingNameTextHeight);
        }
    }
}
