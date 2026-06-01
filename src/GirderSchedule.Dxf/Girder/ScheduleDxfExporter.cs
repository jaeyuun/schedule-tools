using GirderSchedule.Domain.Girder.Layout;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Dxf.Common;
using netDxf;
using System;

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

        public void Export(ScheduleSheet sheet, string filePath)
        {
            Export(sheet, filePath, new ScheduleDxfExportOptions());
        }

        public void Export(ScheduleSheet sheet, string filePath, ScheduleDxfExportOptions options)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet));
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("DXF 저장 경로가 비어 있습니다.", nameof(filePath));
            }

            if (options == null)
            {
                options = new ScheduleDxfExportOptions();
            }

            var template = _loader.LoadTemplateResourcesOnly(options.TemplatePath);
            var document = template.Document;

            _validator.Ensure(document);

            var pointConverter = new DxfPointConverter();
            var textDrawer = new TextDrawer(document, pointConverter, template.Overrides);
            var formDrawer = new FormDrawer(document, pointConverter, textDrawer, template.Overrides);
            var sectionLayoutService = new SectionDxfLayoutService();
            var sectionDrawer = new SectionDrawer(document, pointConverter, template.Overrides);
            var rebarDrawer = new RebarDrawer(document, pointConverter, template.Overrides);
            var dimensionDrawer = new DimensionDrawer(document, pointConverter, template.Overrides);

            formDrawer.Draw();

            var row = sheet.Rows.Count > 0 ? sheet.Rows[0] : null;

            if (row != null)
            {
                //DrawHeaderTexts(textDrawer, row, options);
                DrawItems(row, options, sectionLayoutService, sectionDrawer, rebarDrawer, dimensionDrawer, textDrawer);
            }

            _saver.Save(document, filePath);
        }

        private void DrawHeaderTexts(TextDrawer textDrawer, ScheduleRow row, ScheduleDxfExportOptions options)
        {
            //var firstItem = row.Items.Count > 0 ? row.Items[0] : null;
            //
            //if (firstItem != null)
            //{
            //    textDrawer.DrawValueText(firstItem.Name, DxfLayout.LabelWidth + DxfLayout.SectionGroupWidth * 1.5, -300.0, 120.0);
            //    textDrawer.DrawValueText(FormatSectionText(firstItem.Section.Width, firstItem.Section.Height, options), DxfLayout.LabelWidth + DxfLayout.SectionGroupWidth * 1.5, -600.0, 120.0);
            //}
        }

        private string FormatSectionText(double width, double height, ScheduleDxfExportOptions options)
        {
            var widthText = width.ToString("0");
            var heightText = height.ToString("0");

            if (options.IsWidthOver)
            {
                widthText += " 이상";
            }

            if (options.IsHeightOver)
            {
                heightText += " 이상";
            }

            return "( " + widthText + " × " + heightText + " )";
        }

        private void DrawItems(
            ScheduleRow row,
            ScheduleDxfExportOptions options,
            SectionDxfLayoutService layoutService,
            SectionDrawer sectionDrawer,
            RebarDrawer rebarDrawer,
            DimensionDrawer dimensionDrawer,
            TextDrawer textDrawer)
        {
            var count = row.Items.Count;

            if (count > DxfLayout.SlotCount)
            {
                count = DxfLayout.SlotCount;
            }

            for (var i = 0; i < count; i++)
            {
                DrawItem(row, i, true, options, layoutService, sectionDrawer, rebarDrawer, dimensionDrawer, textDrawer);
            }
        }

        private void DrawItem(
            ScheduleRow row,
            int index,
            bool enabled,
            ScheduleDxfExportOptions options,
            SectionDxfLayoutService layoutService,
            SectionDrawer sectionDrawer,
            RebarDrawer rebarDrawer,
            DimensionDrawer dimensionDrawer,
            TextDrawer textDrawer)
        {
            if (index < 0 || index >= row.Items.Count)
            {
                return;
            }

            if (index < 0 || index >= DxfLayout.SlotCount)
            {
                return;
            }

            var item = row.Items[index];
            var headerBox = DxfLayout.GetHeaderBox(index);
            var sectionBox = DxfLayout.GetSectionBox(index);
            var topBox = DxfLayout.GetTopRebarBox(index);
            var bottomBox = DxfLayout.GetBottomRebarBox(index);
            var stirrupBox = DxfLayout.GetStirrupBox(index);
            var skinBox = DxfLayout.GetSkinBox(index);

            if (!enabled)
            {
                DrawDisabledCell(textDrawer, sectionBox);
                return;
            }

            textDrawer.DrawValueText(item.Position, headerBox.CenterX, -760.0, 90.0);
            DrawMemberForces(textDrawer, item, sectionBox);

            var layout = layoutService.Create(sectionBox, item);

            sectionDrawer.Draw(layout);
            rebarDrawer.Draw(layout, item);
            dimensionDrawer.Draw(layout, item, options.IsWidthOver, options.IsHeightOver);

            DrawRebarRows(textDrawer, item, topBox, bottomBox, stirrupBox, skinBox);
        }

        private void DrawMemberForces(TextDrawer textDrawer, ScheduleItem item, DxfBox sectionBox)
        {
            var moment = FormatMoment(item);
            var shear = FormatShear(item);

            var y = DxfLayout.MemberForceCenterY;

            if (!string.IsNullOrWhiteSpace(moment))
            {
                var mX = sectionBox.Left + sectionBox.Width / 4.0;
                textDrawer.DrawValueText(moment, mX, y, 90.0);
            }

            if (!string.IsNullOrWhiteSpace(shear))
            {
                var vX = sectionBox.Left + sectionBox.Width * 3.0 / 4.0;
                textDrawer.DrawValueText(shear, vX, y, 90.0);
            }
        }

        private string FormatMoment(ScheduleItem item)
        {
            if (item == null || item.MemberForce == null || !item.MemberForce.Moment.HasValue)
            {
                return string.Empty;
            }

            return "M = " + item.MemberForce.Moment.Value.ToString("0");
        }

        private string FormatShear(ScheduleItem item)
        {
            if (item == null || item.MemberForce == null || !item.MemberForce.Shear.HasValue)
            {
                return string.Empty;
            }

            return "V = " + item.MemberForce.Shear.Value.ToString("0");
        }

        private void DrawRebarRows(TextDrawer textDrawer, ScheduleItem item, DxfBox topBox, DxfBox bottomBox, DxfBox stirrupBox, DxfBox skinBox)
        {
            DrawRebarSet(textDrawer, item.TopRebar, topBox.CenterX, topBox.CenterY);
            DrawRebarSet(textDrawer, item.BottomRebar, bottomBox.CenterX, bottomBox.CenterY);

            textDrawer.DrawRebarValueText(FormatStirrup(item.Stirrup), stirrupBox.CenterX, stirrupBox.CenterY, 90.0);

            if (!string.IsNullOrEmpty(item.SkinRebarText))
            {
                textDrawer.DrawRebarValueText(item.SkinRebarText, skinBox.CenterX, skinBox.CenterY, 90.0);
            }
            else
            {
                textDrawer.DrawRebarValueText("-", skinBox.CenterX, skinBox.CenterY, 90.0);
            }
        }

        private void DrawRebarSet(TextDrawer textDrawer, RebarSet rebar, double centerX, double centerY)
        {
            var total = rebar.FirstLayer.Count + rebar.SecondLayer.Count;

            if (total <= 0 || rebar.Diameter <= 0)
            {
                textDrawer.DrawRebarValueText("-", centerX, centerY, 90.0);
                return;
            }

            textDrawer.DrawRebarValueText(total.ToString("0"), centerX - 360.0, centerY, 90.0);
            textDrawer.DrawRebarValueText("- HD", centerX - 40.0, centerY, 90.0);
            textDrawer.DrawRebarValueText(rebar.Diameter.ToString("0"), centerX + 300.0, centerY, 90.0);
        }

        private string FormatStirrup(StirrupData stirrup)
        {
            if (stirrup == null || stirrup.Legs <= 0 || stirrup.Diameter <= 0 || stirrup.Spacing <= 0)
            {
                return "-";
            }

            return stirrup.Legs + "-HD" + stirrup.Diameter + "@" + stirrup.Spacing;
        }

        private void DrawDisabledCell(TextDrawer textDrawer, DxfBox sectionBox)
        {
            textDrawer.DrawValueText("-", sectionBox.CenterX, sectionBox.CenterY, 120.0);
        }
    }
}