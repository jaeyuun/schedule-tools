using System;
using System.IO;
using System.Linq;
using netDxf;

namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfDocumentLoader
    {
        public DxfDocument LoadTemplateResourcesOnly(string templatePath)
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

            //ClearModelEntities(document);

            return document;
        }

        private void ClearModelEntities(DxfDocument document)
        {
            var entities = document.Entities.All.ToList();

            for (var i = 0; i < entities.Count; i++)
            {
                document.Entities.Remove(entities[i]);
            }
        }
    }
}