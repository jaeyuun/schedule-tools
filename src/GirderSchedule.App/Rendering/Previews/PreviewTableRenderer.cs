using GirderSchedule.App.Rendering.Formatting;
using GirderSchedule.App.Rendering.Models;
using GirderSchedule.App.Rendering.Section;
using GirderSchedule.Domain.Models;
using ScheduleTools.Wpf.Rendering;
using System.Windows.Controls;

namespace GirderSchedule.App.Rendering.Previews
{
    public sealed class PreviewTableRenderer
    {
        private readonly CanvasDrawer _drawer;
        private readonly RenderBrushSet _brushes;
        private readonly SectionRenderer _sectionRenderer;
        private readonly PreviewTextFormatter _formatter;

        public PreviewTableRenderer()
            : this(new CanvasDrawer(), new RenderBrushSet())
        {
        }

        public PreviewTableRenderer(CanvasDrawer drawer, RenderBrushSet brushes)
        {
            _drawer = drawer;
            _brushes = brushes;
            _sectionRenderer = new SectionRenderer(drawer, brushes);
            _formatter = new PreviewTextFormatter();
        }

        public void DrawSetCell(Canvas canvas, ScheduleSet? set, double x, double y, double width, double height, int row, int column, bool showLabels)
        {
            var layout = new SchedulePreviewCellLayout(x, y, width, height, showLabels);

            DrawOuterBorder(canvas, layout, row, column);
            DrawGridLines(canvas, layout);

            if (layout.ShowLabels)
            {
                DrawLabels(canvas, layout);
            }

            if (set == null)
            {
                return;
            }

            DrawMemberName(canvas, set, layout);
            DrawPartColumns(canvas, set, layout);
        }

        private void DrawOuterBorder(Canvas canvas, SchedulePreviewCellLayout layout, int row, int column)
        {
            if (row == 0)
            {
                _drawer.DrawLine(canvas, layout.X, layout.Y, layout.X + layout.Width, layout.Y, _brushes.Line, 1.0);
            }

            if (column == 0)
            {
                _drawer.DrawLine(canvas, layout.X, layout.Y, layout.X, layout.Y + layout.Height, _brushes.Line, 1.0);
            }

            _drawer.DrawLine(canvas, layout.X + layout.Width, layout.Y, layout.X + layout.Width, layout.Y + layout.Height, _brushes.Line, 1.0);
            _drawer.DrawLine(canvas, layout.X, layout.Y + layout.Height, layout.X + layout.Width, layout.Y + layout.Height, _brushes.Line, 1.0);
        }

        private void DrawGridLines(Canvas canvas, SchedulePreviewCellLayout layout)
        {
            if (layout.ShowLabels)
            {
                _drawer.DrawLine(canvas, layout.LabelEndX, layout.Y, layout.LabelEndX, layout.EndY, _brushes.Line, 0.8);
            }

            _drawer.DrawLine(canvas, layout.X, layout.SectionAreaY, layout.X + layout.Width, layout.SectionAreaY, _brushes.Line, 0.8);
            _drawer.DrawLine(canvas, layout.X, layout.TopRebarY, layout.X + layout.Width, layout.TopRebarY, _brushes.Line, 0.8);
            _drawer.DrawLine(canvas, layout.X, layout.BottomRebarY, layout.X + layout.Width, layout.BottomRebarY, _brushes.Line, 0.8);
            _drawer.DrawLine(canvas, layout.X, layout.StirrupY, layout.X + layout.Width, layout.StirrupY, _brushes.Line, 0.8);
            _drawer.DrawLine(canvas, layout.X, layout.SkinRebarY, layout.X + layout.Width, layout.SkinRebarY, _brushes.Line, 0.8);

            _drawer.DrawLine(canvas, layout.LabelEndX, layout.ForceY, layout.X + layout.Width, layout.ForceY, _brushes.Line, 0.8);
            _drawer.DrawLine(canvas, layout.LabelEndX, layout.SectionY, layout.X + layout.Width, layout.SectionY, _brushes.Line, 0.8);

            _drawer.DrawLine(canvas, layout.CenterX, layout.SectionAreaY, layout.CenterX, layout.EndY, _brushes.Thin, 0.6);
            _drawer.DrawLine(canvas, layout.RightX, layout.SectionAreaY, layout.RightX, layout.EndY, _brushes.Thin, 0.6);
        }

        private void DrawLabels(Canvas canvas, SchedulePreviewCellLayout layout)
        {
            DrawLabel(canvas, "부 재 명\n( B × H )", layout.X, layout.NameY, layout.LabelWidth, layout.NameHeight);
            DrawLabel(canvas, "단    면", layout.X, layout.SectionAreaY, layout.LabelWidth, layout.SectionTotalHeight);
            DrawLabel(canvas, "상 부 근", layout.X, layout.TopRebarY, layout.LabelWidth, layout.TextRowHeight);
            DrawLabel(canvas, "하 부 근", layout.X, layout.BottomRebarY, layout.LabelWidth, layout.TextRowHeight);
            DrawLabel(canvas, "스 터 럽", layout.X, layout.StirrupY, layout.LabelWidth, layout.TextRowHeight);
            DrawLabel(canvas, "표피철근(X)", layout.X, layout.SkinRebarY, layout.LabelWidth, layout.TextRowHeight);
        }

        private void DrawLabel(Canvas canvas, string text, double x, double y, double width, double height)
        {
            _drawer.DrawTextBox(canvas, text, x, y, width, height, 8.0, _brushes.Thin);
        }

        private void DrawMemberName(Canvas canvas, ScheduleSet set, SchedulePreviewCellLayout layout)
        {
            _drawer.DrawTextBox(canvas, _formatter.FormatMemberName(set), layout.LabelEndX, layout.NameY, layout.ContentWidth, layout.NameHeight, 8.0, _brushes.Text);
        }

        private void DrawPartColumns(Canvas canvas, ScheduleSet set, SchedulePreviewCellLayout layout)
        {
            if (set == null)
            {
                return;
            }

            DrawPartColumn(canvas, GetPosition(set.Left), set.Left, layout.LeftX, layout);
            DrawPartColumn(canvas, GetPosition(set.Center), set.Center, layout.CenterX, layout);
            DrawPartColumn(canvas, GetPosition(set.Right), set.Right, layout.RightX, layout);
        }

        private string GetPosition(ScheduleItem item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            return item.Position;
        }

        private void DrawPartColumn(Canvas canvas, string title, ScheduleItem? item, double x, SchedulePreviewCellLayout layout)
        {
            _drawer.DrawTextBox(canvas, title, x, layout.SectionAreaY, layout.PartWidth, layout.PartHeaderHeight, 8.0, _brushes.Thin);

            var isDisabled = item == null || !item.IsSectionEnabled || item.Section == null;

            DrawMemberForce(canvas, item, x, layout.ForceY, layout.PartWidth, layout.ForceHeight, isDisabled);

            if (item == null || isDisabled)
            {
                DrawDisabledSection(canvas, x, layout);
                return;
            }

            DrawSection(canvas, item, x, layout);
            DrawRebarTexts(canvas, item, x, layout);
        }

        private void DrawMemberForce(Canvas canvas, ScheduleItem? item, double x, double y, double width, double height, bool isDisabled)
        {
            var middleX = x + width / 2.0;
            var halfWidth = width / 2.0;
            var brush = isDisabled ? _brushes.Thin : _brushes.Text;

            _drawer.DrawLine(canvas, middleX, y, middleX, y + height, _brushes.Thin, 0.6);
            _drawer.DrawTextBox(canvas, _formatter.FormatMomentForce(item), x, y, halfWidth, height, 8.0, brush);
            _drawer.DrawTextBox(canvas, _formatter.FormatShearForce(item), middleX, y, halfWidth, height, 8.0, brush);
        }

        private void DrawDisabledSection(Canvas canvas, double x, SchedulePreviewCellLayout layout)
        {
            _drawer.DrawLine(canvas, x + 8.0, layout.SectionY + 8.0, x + layout.PartWidth - 8.0, layout.SectionY + layout.SectionHeight - 8.0, _brushes.Disabled, 0.9);
        }

        private void DrawSection(Canvas canvas, ScheduleItem item, double x, SchedulePreviewCellLayout layout)
        {
            var drawingX = x + 2.0;
            var drawingY = layout.SectionY + 3.0;
            var drawingWidth = layout.PartWidth - 4.0;
            var drawingHeight = layout.SectionHeight - 6.0;

            _sectionRenderer.Draw(canvas, item, drawingX, drawingY, drawingWidth, drawingHeight, false, false, PreviewSectionRenderOptions.Export);

            var noteHeight = 8.0;
            var noteBottomPadding = 4.0;
            var noteY = layout.SectionY + layout.SectionHeight - noteHeight - noteBottomPadding;

            _drawer.DrawTextBox(canvas, _formatter.FormatSectionNote(item), x, noteY, layout.PartWidth, noteHeight, 8.0, _brushes.Text);
        }

        private void DrawRebarTexts(Canvas canvas, ScheduleItem item, double x, SchedulePreviewCellLayout layout)
        {
            _drawer.DrawTextBox(canvas, _formatter.FormatTop(item), x, layout.TopRebarY, layout.PartWidth, layout.TextRowHeight, 8.0, _brushes.Text);
            _drawer.DrawTextBox(canvas, _formatter.FormatBottom(item), x, layout.BottomRebarY, layout.PartWidth, layout.TextRowHeight, 8.0, _brushes.Text);
            _drawer.DrawTextBox(canvas, _formatter.FormatStirrup(item), x, layout.StirrupY, layout.PartWidth, layout.TextRowHeight, 8.0, _brushes.Text);
            _drawer.DrawTextBox(canvas, _formatter.FormatSkinRebar(item), x, layout.SkinRebarY, layout.PartWidth, layout.TextRowHeight, 8.0, _brushes.Text);
        }
    }
}
