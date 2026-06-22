using GirderSchedule.Dxf.Styles;

namespace GirderSchedule.Dxf.Export
{
    public sealed class DxfExportOptions
    {
        public bool IncludeLeft { get; set; }
        public bool IncludeCenter { get; set; }
        public bool IncludeRight { get; set; }
        public bool IsWidthOver { get; set; }
        public bool IsHeightOver { get; set; }
        public string TemplatePath { get; set; }
        public int FormColumnCount { get; set; }
        public DxfLayerNameSet LayerNames { get; set; }

        public DxfExportOptions()
        {
            IncludeLeft = true;
            IncludeCenter = true;
            IncludeRight = true;
            IsWidthOver = false;
            IsHeightOver = false;
            TemplatePath = string.Empty;
            FormColumnCount = 3;
            LayerNames = new DxfLayerNameSet();
        }
    }
}