using netDxf;

namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfDocumentSaver
    {
        public void Save(DxfDocument document, string path)
        {
            document.Save(path);
        }
    }
}