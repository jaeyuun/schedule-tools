using netDxf;
using GirderSchedule.Dxf.Common.Overrides;

namespace GirderSchedule.Dxf.Common.Templates
{
    public sealed class DxfTemplateDocument
    {
        public DxfDocument Document { get; private set; }
        public DxfOverrideTemplateSet Overrides { get; private set; }

        public DxfTemplateDocument(DxfDocument document, DxfOverrideTemplateSet overrides)
        {
            Document = document;
            Overrides = overrides;
        }
    }
}