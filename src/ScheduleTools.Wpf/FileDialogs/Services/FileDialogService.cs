using Microsoft.Win32;
using ScheduleTools.Wpf.FileDialogs.Contracts;
using ScheduleTools.Wpf.FileDialogs.Models;
using System.Windows;

namespace ScheduleTools.Wpf.FileDialogs.Services
{
    public sealed class FileDialogService : IFileDialogService
    {
        public string? OpenFile(OpenFileDialogOptions options, Window? owner = null)
        {
            ArgumentNullException.ThrowIfNull(options);

            var dialog = CreateOpenFileDialog(options, false);
            var result = ShowDialog(dialog, owner);

            return result == true ? dialog.FileName : null;
        }

        public IReadOnlyList<string> OpenFiles(OpenFileDialogOptions options, Window? owner = null)
        {
            ArgumentNullException.ThrowIfNull(options);

            var dialog = CreateOpenFileDialog(options, true);
            var result = ShowDialog(dialog, owner);

            return result == true ? dialog.FileNames : Array.Empty<string>();
        }

        public string? SaveFile(SaveFileDialogOptions options, Window? owner = null)
        {
            ArgumentNullException.ThrowIfNull(options);

            var dialog = new SaveFileDialog
            {
                Title = options.Title,
                Filter = options.Filter,
                DefaultExt = options.DefaultExtension,
                InitialDirectory = options.InitialDirectory,
                FileName = options.FileName,
                AddExtension = options.AddExtension,
                CheckPathExists = options.CheckPathExists,
                OverwritePrompt = options.OverwritePrompt
            };

            var result = ShowDialog(dialog, owner);

            return result == true ? dialog.FileName : null;
        }

        private static OpenFileDialog CreateOpenFileDialog(OpenFileDialogOptions options, bool multiselect)
        {
            return new OpenFileDialog
            {
                Title = options.Title,
                Filter = options.Filter,
                DefaultExt = options.DefaultExtension,
                InitialDirectory = options.InitialDirectory,
                FileName = options.FileName,
                CheckFileExists = options.CheckFileExists,
                CheckPathExists = options.CheckPathExists,
                Multiselect = multiselect || options.Multiselect
            };
        }

        private static bool? ShowDialog(CommonDialog dialog, Window? owner)
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