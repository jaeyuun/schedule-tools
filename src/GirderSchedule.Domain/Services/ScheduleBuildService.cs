using GirderSchedule.Domain.Models;

namespace GirderSchedule.Domain.Services
{
    public sealed class ScheduleBuildService
    {
        public ScheduleSheet CreateSample()
        {
            var sheet = new ScheduleSheet();
            var row = new ScheduleRow();

            var set = new ScheduleSet();
            set.MemberName = "1G1";
            set.Left = CreateItem("END");
            set.Center = CreateItem("CEN");
            set.Right = CreateItem("END");

            row.Sets.Add(set);
            sheet.Rows.Add(row);

            return sheet;
        }

        private ScheduleItem CreateItem(string position)
        {
            var item = new ScheduleItem();

            item.Name = "1G1";
            item.Position = position;

            item.Section.Width = 400;
            item.Section.Height = 600;

            item.TopRebar.Diameter = 0;
            item.TopRebar.FirstLayer.Count = 0;
            item.TopRebar.SecondLayer.Count = 0;

            item.BottomRebar.Diameter = 0;
            item.BottomRebar.FirstLayer.Count = 0;
            item.BottomRebar.SecondLayer.Count = 0;

            item.Stirrup.Legs = 2;
            item.Stirrup.Diameter = 0;
            item.Stirrup.Spacing = 0;

            item.SkinRebar.Diameter = 0;
            item.SkinRebar.Spacing = 0;

            return item;
        }
    }
}