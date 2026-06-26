namespace GirderSchedule.App.Services.Schedule
{
    public sealed class ScheduleMemberNameDisplayService
    {
        public string Format(string floorPrefix, string memberName)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                return string.Empty;
            }

            var prefix = floorPrefix ?? string.Empty;
            var parts = memberName.Split(',');
            var result = string.Empty;

            for (var i = 0; i < parts.Length; i++)
            {
                var name = parts[i].Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(result))
                {
                    result += ", ";
                }

                result += prefix + name;
            }

            return result;
        }
    }
}