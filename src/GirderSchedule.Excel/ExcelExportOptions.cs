namespace GirderSchedule.Excel
{
    public sealed class ExcelExportOptions
    {
        public string SheetName { get; set; }
        public int DefaultCover { get; set; }
        public bool ContinueAcrossFloors { get; set; }

        public ExcelExportOptions()
        {
            SheetName = "Girder Schedule";
            DefaultCover = 40;
            ContinueAcrossFloors = true;
        }
    }
}
