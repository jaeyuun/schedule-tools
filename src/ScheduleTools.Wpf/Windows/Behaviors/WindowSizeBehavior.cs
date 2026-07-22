using System;
using System.Windows;

namespace ScheduleTools.Wpf.Windows.Behaviors
{
    public static class WindowSizeBehavior
    {
        public static readonly DependencyProperty FitToWorkAreaProperty = DependencyProperty.RegisterAttached("FitToWorkArea", typeof(bool), typeof(WindowSizeBehavior), new PropertyMetadata(false, OnFitToWorkAreaChanged));
        public static readonly DependencyProperty WorkAreaMarginProperty = DependencyProperty.RegisterAttached("WorkAreaMargin", typeof(Thickness), typeof(WindowSizeBehavior), new PropertyMetadata(new Thickness(16)));
        public static readonly DependencyProperty CenterOnFirstLoadProperty = DependencyProperty.RegisterAttached("CenterOnFirstLoad", typeof(bool), typeof(WindowSizeBehavior), new PropertyMetadata(true));
        private static readonly DependencyProperty IsInitializedProperty = DependencyProperty.RegisterAttached("IsInitialized", typeof(bool), typeof(WindowSizeBehavior), new PropertyMetadata(false));

        public static void SetFitToWorkArea(DependencyObject element, bool value)
        {
            element.SetValue(FitToWorkAreaProperty, value);
        }

        public static bool GetFitToWorkArea(DependencyObject element)
        {
            return (bool)element.GetValue(FitToWorkAreaProperty);
        }

        public static void SetWorkAreaMargin(DependencyObject element, Thickness value)
        {
            element.SetValue(WorkAreaMarginProperty, value);
        }

        public static Thickness GetWorkAreaMargin(DependencyObject element)
        {
            return (Thickness)element.GetValue(WorkAreaMarginProperty);
        }

        public static void SetCenterOnFirstLoad(DependencyObject element, bool value)
        {
            element.SetValue(CenterOnFirstLoadProperty, value);
        }

        public static bool GetCenterOnFirstLoad(DependencyObject element)
        {
            return (bool)element.GetValue(CenterOnFirstLoadProperty);
        }

        private static void SetIsInitialized(DependencyObject element, bool value)
        {
            element.SetValue(IsInitializedProperty, value);
        }

        private static bool GetIsInitialized(DependencyObject element)
        {
            return (bool)element.GetValue(IsInitializedProperty);
        }

        private static void OnFitToWorkAreaChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not Window window)
                return;

            window.Loaded -= Window_Loaded;

            if (e.NewValue is true)
                window.Loaded += Window_Loaded;
        }

        private static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Window window)
                return;

            FitWindowToWorkArea(window);
        }

        private static void FitWindowToWorkArea(Window window)
        {
            var workArea = SystemParameters.WorkArea;
            var margin = GetWorkAreaMargin(window);

            var availableWidth = Math.Max(0, workArea.Width - margin.Left - margin.Right);
            var availableHeight = Math.Max(0, workArea.Height - margin.Top - margin.Bottom);

            if (availableWidth <= 0 || availableHeight <= 0)
                return;

            if (window.MaxWidth > availableWidth)
                window.MaxWidth = availableWidth;

            if (window.MaxHeight > availableHeight)
                window.MaxHeight = availableHeight;

            if (window.Width > availableWidth)
                window.Width = availableWidth;

            if (window.Height > availableHeight)
                window.Height = availableHeight;

            if (window.MinWidth > availableWidth)
                window.MinWidth = availableWidth;

            if (window.MinHeight > availableHeight)
                window.MinHeight = availableHeight;

            if (!GetIsInitialized(window) && GetCenterOnFirstLoad(window))
            {
                window.Left = workArea.Left + margin.Left + (availableWidth - window.Width) / 2;
                window.Top = workArea.Top + margin.Top + (availableHeight - window.Height) / 2;
            }
            else
            {
                EnsureWindowInsideWorkArea(window, workArea, margin);
            }

            SetIsInitialized(window, true);
        }

        private static void EnsureWindowInsideWorkArea(Window window, Rect workArea, Thickness margin)
        {
            var minimumLeft = workArea.Left + margin.Left;
            var minimumTop = workArea.Top + margin.Top;
            var maximumLeft = workArea.Right - margin.Right - window.Width;
            var maximumTop = workArea.Bottom - margin.Bottom - window.Height;

            if (double.IsNaN(window.Left))
                window.Left = minimumLeft;

            if (double.IsNaN(window.Top))
                window.Top = minimumTop;

            window.Left = Math.Clamp(window.Left, minimumLeft, Math.Max(minimumLeft, maximumLeft));
            window.Top = Math.Clamp(window.Top, minimumTop, Math.Max(minimumTop, maximumTop));
        }
    }
}