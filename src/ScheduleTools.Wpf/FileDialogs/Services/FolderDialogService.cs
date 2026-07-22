using Microsoft.Win32;
using ScheduleTools.Wpf.FileDialogs.Contracts;
using ScheduleTools.Wpf.FileDialogs.Models;
using System.Windows;

namespace ScheduleTools.Wpf.FileDialogs.Services
{
    public sealed class FolderDialogService : IFolderDialogService
    {
        public string? SelectFolder(FolderDialogOptions options, Window? owner = null)
        {
            ArgumentNullException.ThrowIfNull(options);

            var dialog = CreateDialog(options, false);
            var result = ShowDialog(dialog, owner);

            return result == true ? dialog.FolderName : null;
        }

        public IReadOnlyList<string> SelectFolders(FolderDialogOptions options, Window? owner = null)
        {
            ArgumentNullException.ThrowIfNull(options);

            var dialog = CreateDialog(options, true);
            var result = ShowDialog(dialog, owner);

            return result == true ? dialog.FolderNames : Array.Empty<string>();
        }

        private static OpenFolderDialog CreateDialog(FolderDialogOptions options, bool multiselect)
        {
            return new OpenFolderDialog
            {
                Title = options.Title,
                InitialDirectory = options.InitialDirectory,
                Multiselect = multiselect || options.Multiselect
            };
        }

        private static bool? ShowDialog(OpenFolderDialog dialog, Window? owner)
        {
            var resolvedOwner = ResolveOwner(owner);

            return resolvedOwner == null
                ? dialog.ShowDialog()
                : dialog.ShowDialog(resolvedOwner);
        }

        private static Window? ResolveOwner(Window? owner)
        {
            if (owner != null && owner.IsVisible)
                return owner;

            var mainWindow = Application.Current?.MainWindow;

            return mainWindow != null && mainWindow.IsVisible
                ? mainWindow
                : null;
        }
    }
}