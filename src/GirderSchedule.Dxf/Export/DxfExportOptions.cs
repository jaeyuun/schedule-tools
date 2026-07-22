using GirderSchedule.Dxf.Styles;

using ScheduleTools.Dxf.Export;

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
        public DxfStyleNameSet StyleNames { get; set; }

        public DxfExportOptions()
        {
            IncludeLeft = true;
            IncludeCenter = true;
            IncludeRight = true;
            TemplatePath = string.Empty;
            FormColumnCount = DxfExportConstants.DefaultFormColumnCount;
            LayerNames = new DxfLayerNameSet();
            StyleNames = new DxfStyleNameSet();
        }
    }
}
