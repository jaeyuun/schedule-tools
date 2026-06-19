namespace GirderSchedule.Excel.Girder
{
    public sealed class ExportOptions
    {
        public string SheetName { get; set; }
        public int DefaultCover { get; set; }
        public bool ContinueAcrossFloors { get; set; }

        public ExportOptions()
        {
            SheetName = "Girder Schedule";
            DefaultCover = 40;
            ContinueAcrossFloors = true;
        }
    }
}