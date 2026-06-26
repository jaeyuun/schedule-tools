using System.Windows.Media;

namespace GirderSchedule.App.Rendering
{
    public sealed class RenderBrushSet
    {
        public Brush Line { get; private set; }
        public Brush Thin { get; private set; }
        public Brush Text { get; private set; }
        public Brush Rebar { get; private set; }
        public Brush Disabled { get; private set; }

        public RenderBrushSet()
        {
            Line = CreateBrush(25, 25, 25);
            Thin = CreateBrush(120, 120, 120);
            Text = CreateBrush(25, 25, 25);
            Rebar = CreateBrush(70, 70, 70);
            Disabled = CreateBrush(170, 170, 170);
        }

        private static Brush CreateBrush(byte r, byte g, byte b)
        {
            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return brush;
        }
    }
}
