using ClosedXML.Excel;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.Excel.Models
{
    internal sealed class ExcelExportSet
    {
        public XLCellValue Id { get; set; }
        public XLCellValue Name { get; set; }
        public XLCellValue Width { get; set; }
        public XLCellValue Height { get; set; }
        public XLCellValue RebarSection { get; set; }
        public XLCellValue Cover { get; set; }
        public ScheduleItem? BaseItem { get; set; }
        public ScheduleItem? EndI { get; set; }
        public ScheduleItem? Center { get; set; }
        public ScheduleItem? EndJ { get; set; }
        public bool HasEndI
        {
            get { return EndI != null; }
        }

        public bool HasCenter
        {
            get { return Center != null; }
        }

        public bool HasEndJ
        {
            get { return EndJ != null; }
        }
    }
}