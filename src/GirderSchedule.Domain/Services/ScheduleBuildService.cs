using GirderSchedule.Domain.Girder.Models;

namespace GirderSchedule.Domain.Girder.Services
{
    public sealed class ScheduleBuildService
    {
        public ScheduleSheet CreateSample()
        {
            var sheet = new ScheduleSheet();
            var row = new ScheduleRow();

            var set = new ScheduleSet();
            set.MemberName = "1G1";
            set.Left = CreateItem("LEFT");
            set.Center = CreateItem("CENTER");
            set.Right = CreateItem("RIGHT");

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
            item.Section.SlabLeft = 100;
            item.Section.SlabRight = 100;
            item.Section.SlabThickness = 150;

            item.TopRebar.Diameter = 19;
            item.TopRebar.FirstLayer.Count = 4;
            item.TopRebar.SecondLayer.Count = 4;

            item.BottomRebar.Diameter = 19;
            item.BottomRebar.FirstLayer.Count = 4;
            item.BottomRebar.SecondLayer.Count = 2;

            item.Stirrup.Legs = 2;
            item.Stirrup.Diameter = 10;
            item.Stirrup.Spacing = 250;

            item.SkinRebarText = "-";

            return item;
        }
    }
}