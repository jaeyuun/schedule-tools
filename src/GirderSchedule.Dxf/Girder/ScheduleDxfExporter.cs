using GirderSchedule.Dxf.Common;
using GirderSchedule.Domain.Girder.Layout;
using GirderSchedule.Domain.Girder.Models;
using netDxf;

namespace GirderSchedule.Dxf.Girder
{
    public sealed class ScheduleDxfExporter
    {
        private readonly DxfDocumentLoader _loader;
        private readonly DxfDocumentSaver _saver;
        private readonly DxfResourceValidator _validator;

        public ScheduleDxfExporter()
        {
            _loader = new DxfDocumentLoader();
            _saver = new DxfDocumentSaver();
            _validator = new DxfResourceValidator();
        }

        public void Export(string templatePath, string outputPath, ScheduleSheet sheet)
        {
            var document = _loader.Load(templatePath);

            _validator.Validate(document);

            var layout = new DxfLayout();
            var pointConverter = new DxfPointConverter(layout);
            var scheduleLayout = new ScheduleLayout();

            var formDrawer = new FormDrawer(document, pointConverter);
            var textDrawer = new TextDrawer(document, pointConverter);
            var sectionDrawer = new SectionDrawer(document, pointConverter);
            var rebarDrawer = new RebarDrawer(document, pointConverter);
            var dimensionDrawer = new DimensionDrawer(document);

            formDrawer.Draw(sheet);

            for (var rowIndex = 0; rowIndex < sheet.Rows.Count; rowIndex++)
            {
                var row = sheet.Rows[rowIndex];

                for (var itemIndex = 0; itemIndex < row.Items.Count; itemIndex++)
                {
                    var item = row.Items[itemIndex];
                    var itemBox = scheduleLayout.GetItemBox(rowIndex, itemIndex);
                    var sectionBox = scheduleLayout.GetSectionBox(rowIndex, itemIndex);

                    textDrawer.DrawValueText(item.Name, itemBox.CenterX, itemBox.Y + 4700.0, 120.0);
                    textDrawer.DrawValueText(item.Position, itemBox.CenterX, itemBox.Y + 4275.0, 90.0);

                    sectionDrawer.Draw(sectionBox, item);
                    rebarDrawer.Draw(sectionBox, item);
                    dimensionDrawer.Draw(sectionBox, item);

                    DrawRebarTexts(textDrawer, scheduleLayout, rowIndex, itemIndex, item);
                }
            }

            _saver.Save(document, outputPath);
        }

        private void DrawRebarTexts(TextDrawer textDrawer, ScheduleLayout layout, int rowIndex, int itemIndex, ScheduleItem item)
        {
            var topBox = layout.GetTopRebarTextBox(rowIndex, itemIndex);
            var bottomBox = layout.GetBottomRebarTextBox(rowIndex, itemIndex);
            var stirrupBox = layout.GetStirrupTextBox(rowIndex, itemIndex);
            var skinBox = layout.GetSkinRebarTextBox(rowIndex, itemIndex);

            textDrawer.DrawRebarValueText(FormatRebar(item.TopRebar.FirstLayer.Count, item.TopRebar.Diameter), topBox.CenterX - 120.0, topBox.CenterY, 90.0);
            textDrawer.DrawRebarValueText(FormatRebar(item.BottomRebar.FirstLayer.Count, item.BottomRebar.Diameter), bottomBox.CenterX - 120.0, bottomBox.CenterY, 90.0);
            textDrawer.DrawRebarValueText(FormatStirrup(item), stirrupBox.CenterX, stirrupBox.CenterY, 90.0);
            textDrawer.DrawRebarValueText(item.SkinRebarText, skinBox.CenterX, skinBox.CenterY, 90.0);
        }

        private string FormatRebar(int count, int diameter)
        {
            if (count <= 0 || diameter <= 0)
            {
                return "-";
            }

            return count + "-HD" + diameter;
        }

        private string FormatRebarSet(RebarSet rebar)
        {
            if (rebar.SecondLayer.Count <= 0)
            {
                return FormatRebar(rebar.FirstLayer.Count, rebar.Diameter);
            }

            return rebar.FirstLayer.Count + "/" + rebar.SecondLayer.Count + "-HD" + rebar.Diameter;
        }

        private string FormatStirrup(ScheduleItem item)
        {
            if (item.Stirrup.Legs <= 0 || item.Stirrup.Diameter <= 0 || item.Stirrup.Spacing <= 0)
            {
                return "-";
            }

            return item.Stirrup.Legs + "-HD" + item.Stirrup.Diameter + "@" + item.Stirrup.Spacing;
        }
    }
}