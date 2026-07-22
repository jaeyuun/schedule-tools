namespace ScheduleTools.Dxf.Export
{
    public sealed class DxfExportSetting
    {
        public bool ContinueAcrossFloors { get; set; }
        public int FormColumnCount { get; set; }

        public DxfExportSetting()
        {
            ContinueAcrossFloors = true;
            FormColumnCount = DxfExportConstants.DefaultFormColumnCount;
        }
    }
}
