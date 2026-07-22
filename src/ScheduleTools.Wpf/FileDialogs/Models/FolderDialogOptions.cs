namespace ScheduleTools.Wpf.FileDialogs.Models
{
    public sealed class FolderDialogOptions
    {
        public string Title { get; init; } = string.Empty;
        public string InitialDirectory { get; init; } = string.Empty;
        public bool Multiselect { get; init; }
    }
}