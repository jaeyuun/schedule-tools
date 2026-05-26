using netDxf;
using netDxf.Entities;
using GirderSchedule.Dxf.Common;

namespace GirderSchedule.Dxf.Girder
{
    public sealed class FormDrawer
    {
        private readonly DxfDocument _document;
        private readonly DxfPointConverter _pointConverter;
        private readonly TextDrawer _textDrawer;

        public FormDrawer(DxfDocument document, DxfPointConverter pointConverter, TextDrawer textDrawer)
        {
            _document = document;
            _pointConverter = pointConverter;
            _textDrawer = textDrawer;
        }

        public void Draw()
        {
            DrawHorizontalLines();
            DrawVerticalLines();
            DrawLabels();
        }

        private void DrawHorizontalLines()
        {
            AddLine(0.0, 0.0, DxfLayout.SheetWidth, 0.0);
            AddLine(0.0, DxfLayout.HeaderBottomY, DxfLayout.SheetWidth, DxfLayout.HeaderBottomY);
            AddLine(0.0, DxfLayout.SectionBottomY, DxfLayout.SheetWidth, DxfLayout.SectionBottomY);
            AddLine(0.0, DxfLayout.TopRebarBottomY, DxfLayout.SheetWidth, DxfLayout.TopRebarBottomY);
            AddLine(0.0, DxfLayout.BottomRebarBottomY, DxfLayout.SheetWidth, DxfLayout.BottomRebarBottomY);
            AddLine(0.0, DxfLayout.StirrupBottomY, DxfLayout.SheetWidth, DxfLayout.StirrupBottomY);
            AddLine(0.0, DxfLayout.SkinBottomY, DxfLayout.SheetWidth, DxfLayout.SkinBottomY);
        }

        private void DrawVerticalLines()
        {
            AddLine(0.0, 0.0, 0.0, DxfLayout.SkinBottomY);
            AddLine(DxfLayout.LabelWidth, 0.0, DxfLayout.LabelWidth, DxfLayout.SkinBottomY);

            AddLine(DxfLayout.LabelWidth + DxfLayout.SectionGroupWidth, 0.0, DxfLayout.LabelWidth + DxfLayout.SectionGroupWidth, DxfLayout.SkinBottomY);
            AddLine(DxfLayout.LabelWidth + DxfLayout.SectionGroupWidth * 2.0, 0.0, DxfLayout.LabelWidth + DxfLayout.SectionGroupWidth * 2.0, DxfLayout.SkinBottomY);
            AddLine(DxfLayout.SheetWidth, 0.0, DxfLayout.SheetWidth, DxfLayout.SkinBottomY);
        }

        private void DrawLabels()
        {
            _textDrawer.DrawFormText("부 재 명", 600.0, -325.0, 120.0);
            _textDrawer.DrawFormText("( B × H )", 600.0, -620.0, 90.0);
            _textDrawer.DrawFormText("단    면", 600.0, -2450.0, 90.0);
            _textDrawer.DrawFormText("상 부 근", 600.0, -4200.0, 90.0);
            _textDrawer.DrawFormText("하 부 근", 600.0, -4500.0, 90.0);
            _textDrawer.DrawFormText("스 터 럽", 600.0, -4800.0, 90.0);
            _textDrawer.DrawFormText("표피철근(X)", 600.0, -5100.0, 90.0);
        }

        private void AddLine(double x1, double y1, double x2, double y2)
        {
            var line = new Line(_pointConverter.ToVector3(x1, y1), _pointConverter.ToVector3(x2, y2));
            DxfEntityStyle.ApplyByLayer(line, _document.Layers[DxfLayers.FormLine]);
            _document.Entities.Add(line);
        }
    }
}