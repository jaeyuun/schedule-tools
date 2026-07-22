namespace ScheduleTools.Excel
{
    public static class ExcelWorksheetName
    {
        private static readonly char[] InvalidCharacters = { ':', '\\', '/', '?', '*', '[', ']' };

        public static string Sanitize(string? name, string defaultName)
        {
            if (string.IsNullOrWhiteSpace(defaultName))
            {
                throw new ArgumentException("기본 워크시트 이름이 비어 있습니다.", nameof(defaultName));
            }

            var result = string.IsNullOrWhiteSpace(name) ? defaultName : name.Trim();

            foreach (var invalidCharacter in InvalidCharacters)
            {
                result = result.Replace(invalidCharacter.ToString(), string.Empty);
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                result = defaultName;
            }

            return result.Length > 31 ? result[..31] : result;
        }
    }
}
