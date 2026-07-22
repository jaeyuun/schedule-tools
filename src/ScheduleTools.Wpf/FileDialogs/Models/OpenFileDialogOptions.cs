namespace ScheduleTools.Wpf.FileDialogs.Models
{
    public sealed class OpenFileDialogOptions
    {
        public string Title { get; init; } = string.Empty;
        public string Filter { get; init; } = "모든 파일 (*.*)|*.*";
        public string DefaultExtension { get; init; } = string.Empty;
        public string InitialDirectory { get; init; } = string.Empty;
        public string FileName { get; init; } = string.Empty;
        public bool CheckFileExists { get; init; } = true;
        public bool CheckPathExists { get; init; } = true;
        public bool Multiselect { get; init; }
    }
}