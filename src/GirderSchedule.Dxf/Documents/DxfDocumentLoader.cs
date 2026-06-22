using netDxf;
using GirderSchedule.Dxf.Styles.Overrides;

namespace GirderSchedule.Dxf.Documents
{
    public sealed class DxfDocumentLoader
    {
        public DxfTemplateDocument LoadTemplate(string templatePath)
        {
            if (string.IsNullOrWhiteSpace(templatePath))
            {
                throw new ArgumentException("DXF 템플릿 경로가 비어 있습니다.", nameof(templatePath));
            }

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("DXF 템플릿 파일을 찾을 수 없습니다.", templatePath);
            }

            var document = DxfDocument.Load(templatePath);

            if (document == null)
            {
                throw new InvalidOperationException("DXF 템플릿을 로드하지 못했습니다.");
            }

            var overrides = DxfOverrideSet.Create(document);

            return new DxfTemplateDocument(document, overrides);
        }
    }
}