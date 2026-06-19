namespace GirderSchedule.Excel.Common
{
    public static class ColumnUtil
    {
        public static string ToName(int columnNumber)
        {
            var value = columnNumber;
            var result = string.Empty;

            while (value > 0)
            {
                value--;
                result = (char)('A' + value % 26) + result;
                value /= 26;
            }

            return result;
        }
    }
}