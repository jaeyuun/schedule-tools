using System.Windows.Media;

namespace GirderSchedule.App.Preview.Rendering
{
    public sealed class PreviewBrushSet
    {
        public PreviewBrushSet()
        {
            Line = new SolidColorBrush(Color.FromRgb(25, 25, 25));
            Thin = new SolidColorBrush(Color.FromRgb(120, 120, 120));
            Text = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            Bar = new SolidColorBrush(Color.FromRgb(70, 70, 70));
            Disabled = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        }

        public Brush Line { get; private set; }
        public Brush Thin { get; private set; }
        public Brush Text { get; private set; }
        public Brush Bar { get; private set; }
        public Brush Disabled { get; private set; }
    }
}
