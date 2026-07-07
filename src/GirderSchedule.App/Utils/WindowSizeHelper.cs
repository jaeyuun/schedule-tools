using System;
using System.Windows;

namespace GirderSchedule.App.Utils
{
    public static class WindowSizeHelper
    {
        private const double DefaultWorkAreaMargin = 40.0;

        public static void FitToWorkArea(Window window)
        {
            FitToWorkArea(window, DefaultWorkAreaMargin);
        }

        public static void FitToWorkArea(Window window, double margin)
        {
            if (window == null)
            {
                return;
            }

            var workArea = SystemParameters.WorkArea;
            var maxWidth = Math.Max(window.MinWidth, workArea.Width - margin);
            var maxHeight = Math.Max(window.MinHeight, workArea.Height - margin);

            window.MaxWidth = maxWidth;
            window.MaxHeight = maxHeight;

            if (window.Width > maxWidth)
            {
                window.Width = maxWidth;
            }

            if (window.Height > maxHeight)
            {
                window.Height = maxHeight;
            }

            if (window.Left + window.Width > workArea.Right)
            {
                window.Left = Math.Max(workArea.Left, workArea.Right - window.Width - margin / 2.0);
            }

            if (window.Top + window.Height > workArea.Bottom)
            {
                window.Top = Math.Max(workArea.Top, workArea.Bottom - window.Height - margin / 2.0);
            }

            if (window.Left < workArea.Left)
            {
                window.Left = workArea.Left;
            }

            if (window.Top < workArea.Top)
            {
                window.Top = workArea.Top;
            }
        }
    }
}