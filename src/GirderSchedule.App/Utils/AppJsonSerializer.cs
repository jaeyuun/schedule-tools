using System.IO;
using System.Text.Json;

namespace GirderSchedule.App.Common
{
    public static class AppJsonSerializer
    {
        public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, Options);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, Options);
        }

        public static T Read<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return Deserialize<T>(json);
        }

        public static void Write<T>(string filePath, T value)
        {
            var json = Serialize(value);
            File.WriteAllText(filePath, json);
        }
    }
}