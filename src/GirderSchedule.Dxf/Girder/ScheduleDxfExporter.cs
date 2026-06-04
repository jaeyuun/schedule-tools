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
                DrawItems(row, options, sectionLayoutService, sectionDrawer, rebarDrawer, dimensionDrawer, textDrawer);
            }

            _saver.Save(document, filePath);
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
            DrawSlot(set, setIndex, ScheduleSlotType.Center, true, options, layoutService, sectionDrawer, rebarDrawer, dimensionDrawer, textDrawer);
            DrawSlot(set, setIndex, ScheduleSlotType.Right, options.IncludeRight, options, layoutService, sectionDrawer, rebarDrawer, dimensionDrawer, textDrawer);
        }

        private void DrawMemberName(ScheduleSet set, int setIndex, TextDrawer textDrawer)
        {
            var box = DxfLayout.GetMemberNameBox(setIndex);
            var item = GetHeaderItem(set);

            textDrawer.DrawValueText(set.MemberName, box.CenterX, DxfLayout.HeaderMemberNameTextY, DxfLayout.HeaderTextHeight);

            if (item == null || item.Section == null)
            {
                return;
            }

            DrawSectionSizeValues(textDrawer, item, box.CenterX);
        }

        private void DrawSectionSizeValues(TextDrawer textDrawer, ScheduleItem item, double centerX)
        {
            textDrawer.DrawValueText(
                item.Section.Width.ToString("0"),
                centerX + DxfLayout.HeaderWidthValueOffsetX,
                DxfLayout.HeaderSectionSizeTextY,
                DxfLayout.HeaderTextHeight);

            textDrawer.DrawValueText(
                item.Section.Height.ToString("0"),
                centerX + DxfLayout.HeaderHeightValueOffsetX,
                DxfLayout.HeaderSectionSizeTextY,
                DxfLayout.HeaderTextHeight);
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

            textDrawer.DrawValueText(item.Position, positionBox.CenterX, positionBox.CenterY, DxfLayout.RebarTextHeight);
        }

        private void DrawMemberForces(TextDrawer textDrawer, ScheduleItem item, DxfBox sectionBox)
        {
            var moment = FormatMoment(item);
            var shear = FormatShear(item);

            var y = DxfLayout.MemberForceCenterY;

            if (!string.IsNullOrWhiteSpace(moment))
            {
                var mX = sectionBox.Left + sectionBox.Width / 4.0;
                textDrawer.DrawDefPointText(moment, mX, y, DxfLayout.RebarTextHeight);
            }

            if (!string.IsNullOrWhiteSpace(shear))
            {
                var vX = sectionBox.Left + sectionBox.Width * 3.0 / 4.0;
                textDrawer.DrawDefPointText(shear, vX, y, DxfLayout.RebarTextHeight);
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

        private void DrawRebarRows(
            TextDrawer textDrawer,
            ScheduleItem item,
            DxfBox topBox,
            DxfBox bottomBox,
            DxfBox stirrupBox,
            DxfBox skinBox)
        {
            if (item == null)
            {
                return;
            }

            DrawRebarSetValues(textDrawer, item.TopRebar, topBox);
            DrawRebarSetValues(textDrawer, item.BottomRebar, bottomBox);
            DrawStirrupValues(textDrawer, item.Stirrup, stirrupBox);
            DrawSkinRebarValue(textDrawer, item, skinBox);
        }

        private void DrawRebarSetValues(TextDrawer textDrawer, RebarSet rebar, DxfBox box)
        {
            if (rebar == null)
            {
                return;
            }

            var totalCount = GetTotalRebarCount(rebar);

            if (totalCount <= 0)
            {
                return;
            }

            var countText = totalCount.ToString("0");
            var diameterText = rebar.Diameter.ToString("0");

            var dashHdCenterX = box.CenterX + DxfLayout.RebarDashHdCenterOffsetX;
            var countX = GetLeftValueCenterX(dashHdCenterX, DxfLayout.RebarDashHdTextWidth, countText) + DxfLayout.RebarCountAdjustX;
            var diameterX = GetRightValueCenterX(dashHdCenterX, DxfLayout.RebarDashHdTextWidth, diameterText) + DxfLayout.RebarDiameterAdjustX;
            var y = box.CenterY + DxfLayout.RebarValueOffsetY;

            textDrawer.DrawValueText(countText, countX, y, DxfLayout.RebarTextHeight);
            textDrawer.DrawValueText(diameterText, diameterX, y, DxfLayout.RebarTextHeight);
        }

        private int GetTotalRebarCount(RebarSet rebar)
        {
            var count = 0;

            if (rebar.FirstLayer != null)
            {
                count += rebar.FirstLayer.Count;
            }

            if (rebar.SecondLayer != null)
            {
                count += rebar.SecondLayer.Count;
            }

            return count;
        }

        private void DrawStirrupValues(TextDrawer textDrawer, StirrupData stirrup, DxfBox box)
        {
            if (stirrup == null)
            {
                return;
            }

            var legText = stirrup.Legs.ToString("0");
            var diameterText = stirrup.Diameter.ToString("0");
            var spacingText = stirrup.Spacing.ToString("0");

            var dashHdCenterX = box.CenterX + DxfLayout.StirrupDashHdCenterOffsetX;
            var atCenterX = box.CenterX + DxfLayout.StirrupAtCenterOffsetX;

            var legX = GetLeftValueCenterX(dashHdCenterX, DxfLayout.StirrupDashHdTextWidth, legText) + DxfLayout.StirrupLegAdjustX;
            var diameterX = GetRightValueCenterX(dashHdCenterX, DxfLayout.StirrupDashHdTextWidth, diameterText) + DxfLayout.StirrupDiameterAdjustX;
            var spacingX = GetRightValueCenterX(atCenterX, DxfLayout.StirrupAtTextWidth, spacingText) + DxfLayout.StirrupSpacingAdjustX;
            var y = box.CenterY + DxfLayout.StirrupValueOffsetY;

            textDrawer.DrawValueText(legText, legX, y, DxfLayout.RebarTextHeight);
            textDrawer.DrawValueText(diameterText, diameterX, y, DxfLayout.RebarTextHeight);
            textDrawer.DrawValueText(spacingText, spacingX, y, DxfLayout.RebarTextHeight);
        }

        private double EstimateNumberTextWidth(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0.0;
            }

            return value.Length * DxfLayout.RebarTextHeight * DxfLayout.NumberTextWidthFactor;
        }

        private double GetLeftValueCenterX(double fixedCenterX, double fixedTextWidth, string value)
        {
            return fixedCenterX - fixedTextWidth / 2.0 - DxfLayout.ValueTextGap - EstimateNumberTextWidth(value) / 2.0;
        }

        private double GetRightValueCenterX(double fixedCenterX, double fixedTextWidth, string value)
        {
            return fixedCenterX + fixedTextWidth / 2.0 + DxfLayout.ValueTextGap + EstimateNumberTextWidth(value) / 2.0;
        }

        private void DrawSkinRebarValue(TextDrawer textDrawer, ScheduleItem item, DxfBox skinBox)
        {
            if (string.IsNullOrWhiteSpace(item.SkinRebarText))
            {
                return;
            }

            textDrawer.DrawValueText(item.SkinRebarText, skinBox.CenterX, skinBox.CenterY, DxfLayout.RebarTextHeight);
        }
    }
}