using netDxf;
using GirderSchedule.Dxf.Styles.Overrides;

namespace GirderSchedule.Dxf.Documents
{
    public sealed class DxfTemplateDocument
    {
        public DxfDocument Document { get; private set; }
        public DxfOverrideSet Overrides { get; private set; }

        public DxfTemplateDocument(DxfDocument document, DxfOverrideSet overrides)
        {
            Document = document;
            Overrides = overrides;
        }
    }
}