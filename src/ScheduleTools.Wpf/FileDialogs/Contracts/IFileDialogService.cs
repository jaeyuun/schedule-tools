using ScheduleTools.Wpf.FileDialogs.Models;
using System.Windows;

namespace ScheduleTools.Wpf.FileDialogs.Contracts
{
    public interface IFileDialogService
    {
        string? OpenFile(OpenFileDialogOptions options, Window? owner = null);
        IReadOnlyList<string> OpenFiles(OpenFileDialogOptions options, Window? owner = null);
        string? SaveFile(SaveFileDialogOptions options, Window? owner = null);
    }
}