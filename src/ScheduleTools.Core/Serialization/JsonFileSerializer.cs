using Newtonsoft.Json;
using System.Text;

namespace ScheduleTools.Core.Serialization
{
    public static class JsonFileSerializer
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        public static string Serialize<T>(T value) where T : class
        {
            ArgumentNullException.ThrowIfNull(value);
            return JsonConvert.SerializeObject(value, SerializerSettings);
        }

        public static T Deserialize<T>(string json) where T : class, new()
        {
            return Deserialize(json, () => new T());
        }

        public static T Deserialize<T>(string json, Func<T> defaultFactory) where T : class
        {
            ArgumentNullException.ThrowIfNull(defaultFactory);

            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultFactory();
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(json, SerializerSettings) ?? defaultFactory();
            }
            catch
            {
                return defaultFactory();
            }
        }

        public static void Save<T>(string filePath, T value) where T : class
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

            var json = Serialize(value);
            File.WriteAllText(filePath, json, new UTF8Encoding(false));
        }

        public static T Load<T>(string filePath) where T : class, new()
        {
            return Load(filePath, () => new T());
        }

        public static T Load<T>(string filePath, Func<T> defaultFactory) where T : class
        {
            ArgumentNullException.ThrowIfNull(defaultFactory);

            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    return defaultFactory();
                }

                var json = File.ReadAllText(filePath, Encoding.UTF8);
                return Deserialize(json, defaultFactory);
            }
            catch
            {
                return defaultFactory();
            }
        }
    }
}