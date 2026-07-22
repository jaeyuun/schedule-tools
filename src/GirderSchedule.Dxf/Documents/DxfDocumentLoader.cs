using GirderSchedule.Dxf.Styles.Overrides;
using ScheduleTools.Dxf.Documents;

namespace GirderSchedule.Dxf.Documents
{
    public sealed class DxfDocumentLoader
    {
        private readonly DxfDocumentFile _documentFile = new DxfDocumentFile();

        public DxfTemplateDocument LoadTemplate(string templatePath)
        {
            var document = _documentFile.Load(templatePath);
            var overrides = DxfOverrideSet.Create(document);
            return new DxfTemplateDocument(document, overrides);
        }
    }
}
