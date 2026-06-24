namespace GirderSchedule.Domain.Layouts
{
    public sealed class ScheduleLayout
    {
        public double SheetWidth { get; private set; }
        public double SheetHeight { get; private set; }

        public double FormX { get; private set; }
        public double FirstRowY { get; private set; }

        public double RowHeight { get; private set; }
        public double FormWidth { get; private set; }

        public double LabelColumnWidth { get; private set; }
        public double ItemColumnWidth { get; private set; }

        public int ItemColumnCount { get; private set; }

        public ScheduleLayout()
        {
            SheetWidth = 27300.0;
            SheetHeight = 19305.0;

            FormX = 3009.8381;
            FirstRowY = 12152.5003;

            RowHeight = 5000.0;
            FormWidth = 22710.0;

            LabelColumnWidth = 1200.0;
            ItemColumnWidth = 2390.0;

            ItemColumnCount = 9;
        }

        public LayoutBox GetRowBox(int rowIndex)
        {
            return new LayoutBox(FormX, FirstRowY - RowHeight * rowIndex, FormWidth, RowHeight);
        }

        public LayoutBox GetItemBox(int rowIndex, int itemIndex)
        {
            var row = GetRowBox(rowIndex);
            var x = row.X + LabelColumnWidth + ItemColumnWidth * itemIndex;

            return new LayoutBox(x, row.Y, ItemColumnWidth, RowHeight);
        }

        public LayoutBox GetSectionBox(int rowIndex, int itemIndex)
        {
            var item = GetItemBox(rowIndex, itemIndex);
            return new LayoutBox(item.X, item.Y + 1200.0, item.Width, 3200.0);
        }

        public LayoutBox GetTopRebarTextBox(int rowIndex, int itemIndex)
        {
            var item = GetItemBox(rowIndex, itemIndex);
            return new LayoutBox(item.X, item.Y + 900.0, item.Width, 300.0);
        }

        public LayoutBox GetBottomRebarTextBox(int rowIndex, int itemIndex)
        {
            var item = GetItemBox(rowIndex, itemIndex);
            return new LayoutBox(item.X, item.Y + 600.0, item.Width, 300.0);
        }

        public LayoutBox GetStirrupTextBox(int rowIndex, int itemIndex)
        {
            var item = GetItemBox(rowIndex, itemIndex);
            return new LayoutBox(item.X, item.Y + 300.0, item.Width, 300.0);
        }

        public LayoutBox GetSkinRebarTextBox(int rowIndex, int itemIndex)
        {
            var item = GetItemBox(rowIndex, itemIndex);
            return new LayoutBox(item.X, item.Y, item.Width, 300.0);
        }
    }
}