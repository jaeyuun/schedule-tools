using ScheduleTools.Wpf.FileDialogs.Models;
using System.Windows;

namespace ScheduleTools.Wpf.FileDialogs.Contracts
{
    public interface IFolderDialogService
    {
        string? SelectFolder(FolderDialogOptions options, Window? owner = null);
        IReadOnlyList<string> SelectFolders(FolderDialogOptions options, Window? owner = null);
    }
}