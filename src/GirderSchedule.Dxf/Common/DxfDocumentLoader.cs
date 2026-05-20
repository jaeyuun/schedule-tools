using netDxf;

namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfDocumentLoader
    {
        public DxfDocument Load(string path)
        {
            return DxfDocument.Load(path);
        }
    }
}