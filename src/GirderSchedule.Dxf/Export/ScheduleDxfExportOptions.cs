namespace GirderSchedule.Dxf.Export
{
    public sealed class ScheduleDxfExportOptions
    {
        public bool IncludeLeft { get; set; }
        public bool IncludeCenter { get; set; }
        public bool IncludeRight { get; set; }

        public bool IsWidthOver { get; set; }
        public bool IsHeightOver { get; set; }

        public string TemplatePath { get; set; }

        public ScheduleDxfExportOptions()
        {
            IncludeLeft = true;
            IncludeCenter = true;
            IncludeRight = true;
            IsWidthOver = false;
            IsHeightOver = false;
            TemplatePath = string.Empty;
        }
    }
}