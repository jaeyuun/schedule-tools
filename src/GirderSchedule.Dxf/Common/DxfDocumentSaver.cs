using System;
using System.IO;
using netDxf;

namespace GirderSchedule.Dxf.Common
{
    public sealed class DxfDocumentSaver
    {
        public void Save(DxfDocument document, string filePath)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("저장 경로가 비어 있습니다.", nameof(filePath));
            }

            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var tempPath = filePath + ".tmp";

            try
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }

                document.Save(tempPath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                File.Move(tempPath, filePath);
            }
            catch (IOException ex)
            {
                TryDeleteTempFile(tempPath);

                throw new InvalidOperationException(
                    "DXF 파일을 저장할 수 없습니다.\r\n\r\n" +
                    "저장하려는 파일이 다른 프로그램에서 열려 있을 수 있습니다.\r\n" +
                    "AutoCAD, GstarCAD, 뷰어 또는 탐색기 미리보기를 닫은 뒤 다시 저장해 주세요.\r\n\r\n" +
                    "파일 경로: " + filePath,
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                TryDeleteTempFile(tempPath);

                throw new InvalidOperationException(
                    "DXF 파일을 저장할 권한이 없습니다.\r\n\r\n" +
                    "쓰기 권한이 있는 폴더인지 확인하거나 다른 위치에 저장해 주세요.\r\n\r\n" +
                    "파일 경로: " + filePath,
                    ex);
            }
        }

        private void TryDeleteTempFile(string tempPath)
        {
            try
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
            catch
            {
            }
        }
    }
}