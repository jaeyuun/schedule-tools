using System.Windows;

namespace ScheduleTools.Wpf.Settings
{
    public static class AppFontSizeService
    {
        public static void Apply(double fontSize, Func<double, double> normalizeFontSize)
        {
            ArgumentNullException.ThrowIfNull(normalizeFontSize);

            if (Application.Current == null)
            {
                return;
            }

            var normalizedFontSize = normalizeFontSize(fontSize);

            Application.Current.Resources["DefaultFontSize"] = normalizedFontSize;
            Application.Current.Resources["SectionFontSize"] = normalizedFontSize + 2;
            Application.Current.Resources["TitleFontSize"] = normalizedFontSize + 6;
            Application.Current.Resources["IconFontSize"] = normalizedFontSize + 12;
        }
    }
}