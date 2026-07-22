using System.IO.Compression;
using System.Text;

namespace ScheduleTools.Core.Serialization
{
    public sealed class VersionedCompressedJsonStore<T> where T : class
    {
        private readonly string _header;
        private readonly Func<T> _defaultFactory;

        public VersionedCompressedJsonStore(string header, Func<T> defaultFactory)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                throw new ArgumentException("파일 헤더가 비어 있습니다.", nameof(header));
            }

            _header = header;
            _defaultFactory = defaultFactory ?? throw new ArgumentNullException(nameof(defaultFactory));
        }

        public void Save(string filePath, T value)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("파일 경로가 비어 있습니다.", nameof(filePath));
            }

            ArgumentNullException.ThrowIfNull(value);
            var directoryPath = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var jsonBytes = Encoding.UTF8.GetBytes(JsonFileSerializer.Serialize(value));

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new BinaryWriter(fileStream, Encoding.UTF8, true);
            writer.Write(_header);
            writer.Flush();

            using var gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal, true);
            gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
        }

        public T Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("파일 경로가 비어 있습니다.", nameof(filePath));
            }

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fileStream, Encoding.UTF8, true);
            var actualHeader = reader.ReadString();

            if (!string.Equals(actualHeader, _header, StringComparison.Ordinal))
            {
                throw new InvalidDataException("지원하지 않는 파일 형식입니다.");
            }

            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var memoryStream = new MemoryStream();
            gzipStream.CopyTo(memoryStream);

            var json = Encoding.UTF8.GetString(memoryStream.ToArray());
            return JsonFileSerializer.Deserialize(json, _defaultFactory);
        }
    }
}
