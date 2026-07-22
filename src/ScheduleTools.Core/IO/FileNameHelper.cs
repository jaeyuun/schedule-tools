namespace ScheduleTools.Core.IO
{
    public static class FileNameHelper
    {
        public static string Sanitize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            var result = value.Trim();

            for (var i = 0; i < invalidChars.Length; i++)
            {
                result = result.Replace(invalidChars[i].ToString(), string.Empty);
            }

            return result;
        }
    }
}
