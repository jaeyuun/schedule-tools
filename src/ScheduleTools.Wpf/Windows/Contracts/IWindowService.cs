using System.Windows;

namespace ScheduleTools.Wpf.Windows.Contracts
{
    public interface IWindowService
    {
        Window? GetMainWindow();
        bool? ShowDialog(Window window, Window? owner = null);
        void Show(Window window, Window? owner = null);
        void Close(Window? window);
        void SwitchMainWindow(Window nextWindow);
        void SwitchMainWindow(Window? currentWindow, Window nextWindow);
    }
}