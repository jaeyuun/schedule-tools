using System.IO;
using netDxf;

namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfDocumentSaver
    {
        public void Save(DxfDocument document, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            document.Save(filePath);
        }
    }
}