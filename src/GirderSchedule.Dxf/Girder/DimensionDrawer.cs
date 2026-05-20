using GirderSchedule.Domain.Girder.Layout;
using GirderSchedule.Domain.Girder.Models;
using netDxf;

namespace GirderSchedule.Dxf.Girder
{
    public sealed class DimensionDrawer
    {
        private readonly DxfDocument _document;

        public DimensionDrawer(DxfDocument document)
        {
            _document = document;
        }

        public void Draw(CellBox box, ScheduleItem item)
        {
        }
    }
}