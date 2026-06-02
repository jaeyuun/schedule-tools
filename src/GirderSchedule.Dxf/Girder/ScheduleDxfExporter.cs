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
            for (var setIndex = 0; setIndex < row.Sets.Count; setIndex++)
            {
                DrawSet(row.Sets[setIndex], setIndex, options, layoutService, sectionDrawer, rebarDrawer, dimensionDrawer, textDrawer);
            }
        }

        private void DrawSet(
            ScheduleSet set,
            int setIndex,
            ScheduleDxfExportOptions options,
            SectionDxfLayoutService layoutService,
            SectionDrawer sectionDrawer,
            RebarDrawer rebarDrawer,
            DimensionDrawer dimensionDrawer,
            TextDrawer textDrawer)
        {
            DrawMemberName(set, setIndex, textDrawer);

            DrawSlot(set, setIndex, ScheduleSlotType.Left, options.IncludeLeft, options, layoutService, sectionDrawer, rebarDrawer, dimensionDrawer, textDrawer);
            DrawSlot(set, setIndex, ScheduleSlotType.Center, options.IncludeCenter, options, layoutService, sectionDrawer, rebarDrawer, dimensionDrawer, textDrawer);
            DrawSlot(set, setIndex, ScheduleSlotType.Right, options.IncludeRight, options, layoutService, sectionDrawer, rebarDrawer, dimensionDrawer, textDrawer);
        }

        private void DrawMemberName(ScheduleSet set, int setIndex, TextDrawer textDrawer)
        {
            var box = DxfLayout.GetMemberNameBox(setIndex);
            var item = GetHeaderItem(set);

            textDrawer.DrawValueText(set.MemberName, box.CenterX, DxfLayout.HeaderMemberNameTextY, DxfLayout.HeaderTextHeight);

            if (item == null)
            {
                return;
            }

            textDrawer.DrawValueText(FormatSectionSize(item), box.CenterX, DxfLayout.HeaderSectionSizeTextY, DxfLayout.HeaderTextHeight);
        }

        private ScheduleItem GetHeaderItem(ScheduleSet set)
        {
            if (set.Center != null)
            {
                return set.Center;
            }

            if (set.Left != null)
            {
                return set.Left;
            }

            return set.Right;
        }

        private string FormatSectionSize(ScheduleItem item)
        {
            if (item == null || item.Section == null)
            {
                return string.Empty;
            }

            return "( " + item.Section.Width.ToString("0") + " × " + item.Section.Height.ToString("0") + " )";
        }

        private void DrawSlot(
            ScheduleSet set,
            int setIndex,
            ScheduleSlotType type,
            bool enabled,
            ScheduleDxfExportOptions options,
            SectionDxfLayoutService layoutService,
            SectionDrawer sectionDrawer,
            RebarDrawer rebarDrawer,
            DimensionDrawer dimensionDrawer,
            TextDrawer textDrawer)
        {
            var sectionBox = DxfLayout.GetSectionBox(setIndex, type);

            if (!enabled)
            {
                sectionDrawer.DrawDisabled(sectionBox);
                return;
            }

            var item = set.GetItem(type);

            if (item == null)
            {
                return;
            }

            var positionBox = DxfLayout.GetPositionBox(setIndex, type);
            var topBox = DxfLayout.GetTopRebarBox(setIndex, type);
            var bottomBox = DxfLayout.GetBottomRebarBox(setIndex, type);
            var stirrupBox = DxfLayout.GetStirrupBox(setIndex, type);
            var skinBox = DxfLayout.GetSkinBox(setIndex, type);

            DrawPositionText(textDrawer, item, positionBox);
            DrawMemberForces(textDrawer, item, sectionBox);

            var layout = layoutService.Create(sectionBox, item);

            sectionDrawer.Draw(layout);
            rebarDrawer.Draw(layout, item);
            dimensionDrawer.Draw(layout, item, options.IsWidthOver, options.IsHeightOver);

            DrawRebarRows(textDrawer, item, topBox, bottomBox, stirrupBox, skinBox);
        }

        private void DrawPositionText(TextDrawer textDrawer, ScheduleItem item, DxfBox positionBox)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Position))
            {
                return;
            }

            textDrawer.DrawValueText(item.Position, positionBox.CenterX, positionBox.CenterY, 90.0);
        }

        private void DrawMemberForces(TextDrawer textDrawer, ScheduleItem item, DxfBox sectionBox)
        {
            var moment = FormatMoment(item);
            var shear = FormatShear(item);

            var y = DxfLayout.MemberForceCenterY;

            if (!string.IsNullOrWhiteSpace(moment))
            {
                var mX = sectionBox.Left + sectionBox.Width / 4.0;
                textDrawer.DrawDefPointText(moment, mX, y, 90.0);
            }

            if (!string.IsNullOrWhiteSpace(shear))
            {
                var vX = sectionBox.Left + sectionBox.Width * 3.0 / 4.0;
                textDrawer.DrawDefPointText(shear, vX, y, 90.0);
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
    }
}