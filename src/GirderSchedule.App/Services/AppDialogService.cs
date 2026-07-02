using GirderSchedule.App.Views.Common;
using System.Windows;

namespace GirderSchedule.App.Services
{
    public static class AppDialogService
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

        private static Window GetActiveWindow()
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