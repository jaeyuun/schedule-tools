using System.Globalization;
using System.Windows.Data;

namespace ScheduleTools.Wpf.Converters
{
    public sealed class SearchIconTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            return string.IsNullOrEmpty(text) ? "🔍︎" : "❌";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}