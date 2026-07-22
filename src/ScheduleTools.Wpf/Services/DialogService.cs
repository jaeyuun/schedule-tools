using ScheduleTools.Wpf.Dialogs;
using System.Windows;

namespace ScheduleTools.Wpf.Services
{
    public static class DialogService
    {
        public static void ShowNotice(string title, string content)
        {
            var dialog = new NoticeDialog(title, content)
            {
                Owner = GetActiveWindow()
            };

            dialog.ShowDialog();
        }

        public static MessageBoxResult ShowConfirm(string title, string content)
        {
            var dialog = new ConfirmDialog(title, content)
            {
                Owner = GetActiveWindow()
            };

            dialog.ShowDialog();
            return dialog.Result;
        }

        public static MessageBoxResult ShowSaveConfirm(string title, string content)
        {
            var dialog = new SaveConfirmDialog(title, content)
            {
                Owner = GetActiveWindow()
            };

            dialog.ShowDialog();
            return dialog.Result;
        }

        private static Window? GetActiveWindow()
        {
            if (Application.Current == null)
            {
                return null;
            }

            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsActive)
                {
                    return window;
                }
            }

            return Application.Current.MainWindow;
        }
    }
}
