using netDxf;

namespace ScheduleTools.Dxf.Documents
{
    public sealed class DxfDocumentFile
    {
        public DxfDocument Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("DXF 파일 경로가 비어 있습니다.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("DXF 파일을 찾을 수 없습니다.", filePath);
            }

            return DxfDocument.Load(filePath)
                ?? throw new InvalidOperationException("DXF 문서를 불러올 수 없습니다.");
        }

        public void Save(DxfDocument document, string filePath)
        {
            ArgumentNullException.ThrowIfNull(document);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("저장 경로가 비어 있습니다.", nameof(filePath));
            }

            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var tempPath = filePath + ".tmp";

            try
            {
                TryDelete(tempPath);
                document.Save(tempPath);
                File.Move(tempPath, filePath, true);
            }
            catch
            {
                TryDelete(tempPath);
                throw;
            }
        }

        private static void TryDelete(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // 원래 예외를 보존하기 위해 임시 파일 정리 실패는 무시한다.
            }
        }
    }
}
