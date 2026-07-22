using ScheduleTools.Wpf.Windows.Contracts;
using System;
using System.Windows;

namespace ScheduleTools.Wpf.Windows.Services
{
    public sealed class WindowService : IWindowService
    {
        public Window? GetMainWindow()
        {
            return Application.Current?.MainWindow;
        }

        public bool? ShowDialog(Window window, Window? owner = null)
        {
            ArgumentNullException.ThrowIfNull(window);

            SetOwner(window, owner);
            return window.ShowDialog();
        }

        public void Show(Window window, Window? owner = null)
        {
            ArgumentNullException.ThrowIfNull(window);

            SetOwner(window, owner);
            window.Show();
        }

        public void Close(Window? window)
        {
            window?.Close();
        }

        public void SwitchMainWindow(Window nextWindow)
        {
            SwitchMainWindow(GetMainWindow(), nextWindow);
        }

        public void SwitchMainWindow(Window? currentWindow, Window nextWindow)
        {
            ArgumentNullException.ThrowIfNull(nextWindow);

            var application = Application.Current ?? throw new InvalidOperationException("현재 WPF Application을 찾을 수 없습니다.");

            application.MainWindow = nextWindow;
            nextWindow.Show();

            if (currentWindow != null && !ReferenceEquals(currentWindow, nextWindow))
                currentWindow.Close();
        }

        private static void SetOwner(Window window, Window? owner)
        {
            var resolvedOwner = owner ?? ResolveDefaultOwner(window);

            if (resolvedOwner == null)
                return;

            if (ReferenceEquals(window, resolvedOwner))
                return;

            window.Owner = resolvedOwner;
        }

        private static Window? ResolveDefaultOwner(Window targetWindow)
        {
            var mainWindow = Application.Current?.MainWindow;

            if (mainWindow == null)
                return null;

            if (ReferenceEquals(mainWindow, targetWindow))
                return null;

            if (!mainWindow.IsVisible)
                return null;

            return mainWindow;
        }
    }
}