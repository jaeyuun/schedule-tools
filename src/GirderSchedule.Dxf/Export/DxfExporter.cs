using GirderSchedule.Domain.Models.Export;
using GirderSchedule.Dxf.Documents;
using GirderSchedule.Dxf.Drawers;
using GirderSchedule.Dxf.Drawers.Common;
using GirderSchedule.Dxf.Layouts;
using GirderSchedule.Dxf.Styles;
using GirderSchedule.Dxf.Styles.Overrides;
using GirderSchedule.Dxf.Validation;
using netDxf;

namespace GirderSchedule.Dxf.Export
{
    public sealed class DxfExporter
    {
        private readonly DxfDocumentLoader _loader;
        private readonly DxfDocumentSaver _saver;
        private readonly DxfResourceValidator _validator;
        private readonly SectionLayoutFactory _layoutFactory;

        public DxfExporter()
        {
            _loader = new DxfDocumentLoader();
            _saver = new DxfDocumentSaver();
            _validator = new DxfResourceValidator();
            _layoutFactory = new SectionLayoutFactory();
        }

        public void Export(List<ScheduleExportPage> pages, string filePath, DxfExportOptions options)
        {
            if (pages == null)
            {
                throw new ArgumentNullException(nameof(pages));
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("DXF 저장 경로가 비어 있습니다.", nameof(filePath));
            }

            if (options == null)
            {
                options = new DxfExportOptions();
            }

            var formColumnCount = PageLayout.NormalizeColumnCount(options.FormColumnCount);

            var template = _loader.LoadTemplate(options.TemplatePath);
            var document = template.Document;

            ClearModelSpaceEntities(document);

            _validator.Ensure(document);

            var scheduleDrawer = CreateScheduleDrawer(document, options, template.Overrides);

            for (var pageIndex = 0; pageIndex < pages.Count; pageIndex++)
            {
                var pageBaseX = PageLayout.GetBaseX(pageIndex, formColumnCount);
                var pageBaseY = PageLayout.GetBaseY(pageIndex, formColumnCount);

                scheduleDrawer.Draw(pages[pageIndex], pageBaseX, pageBaseY, options);
            }

            _saver.Save(document, filePath);
        }

        private ScheduleDrawer CreateScheduleDrawer(DxfDocument document, DxfExportOptions options, DxfOverrideSet overrides)
        {
            var layerNames = options.LayerNames ?? new DxfLayerNameSet();
            var styler = new DxfEntityStyler(document, layerNames, overrides);

            var entityDrawer = new DxfEntityDrawer(document, styler);
            var textDrawer = new TextDrawer(document, styler);
            var tableDrawer = new TableDrawer(textDrawer, entityDrawer);
            var titleBlockDrawer = new TitleBlockDrawer(document, textDrawer, entityDrawer);
            var sectionDrawer = new SectionDrawer(entityDrawer);
            var rebarDrawer = new RebarDrawer(document, styler);
            var dimensionDrawer = new DimensionDrawer(document, styler);
            var rebarTextDrawer = new RebarTextDrawer(textDrawer);
            var scheduleDrawer = new ScheduleDrawer(textDrawer, tableDrawer, titleBlockDrawer, sectionDrawer, rebarDrawer, dimensionDrawer, rebarTextDrawer, _layoutFactory);

            return scheduleDrawer;
        }

        private static void ClearModelSpaceEntities(DxfDocument document)
        {
            #if RELEASE
            foreach (var entity in document.Entities.All.ToList())
            {
                document.Entities.Remove(entity);
            }
            #endif
        }
    }
}