using ClosedXML.Excel;

namespace GirderSchedule.Excel.Girder
{
    public static class Style
    {
        public static readonly XLColor HeaderFill = XLColor.FromHtml("#BFBFBF");
        public static readonly XLColor SubHeaderFill = XLColor.FromHtml("#EDEDED");
        public static readonly XLColor WhiteFill = XLColor.White;
        public static readonly XLColor BorderColor = XLColor.Black;

        public const string FontName = "맑은 고딕";
        public const int FontSize = 10;
        public const double FloorNameHeight = 20.0;
        public const double DefaultHeight = 13.5;

        public static void ApplySheetBase(IXLWorksheet sheet)
        {
            sheet.Style.Font.FontName = FontName;
            sheet.Style.Font.FontSize = FontSize;
            sheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            sheet.Columns().Width = 8.0;

            sheet.Column("A").Width = 8.0;
            sheet.Column("B").Width = 13.0;
            sheet.Column("C").Width = 6.0;
            sheet.Column("D").Width = 4.0;
            sheet.Column("E").Width = 6.0;
            sheet.Column("F").Width = 15.0;
            sheet.Column("G").Width = 13.0;
            sheet.Column("H").Width = 8.0;
            sheet.Column("I").Width = 8.0;
            sheet.Column("J").Width = 8.0;
            sheet.Column("K").Width = 6.0;
            sheet.Column("L").Width = 6.0;
            sheet.Column("M").Width = 4.0;
            sheet.Column("N").Width = 6.0;
            sheet.Column("O").Width = 6.0;
            sheet.Column("P").Width = 4.0;
            sheet.Column("Q").Width = 6.0;
            sheet.Column("R").Width = 6.0;
            sheet.Column("S").Width = 4.0;
            sheet.Column("T").Width = 6.0;

            sheet.Row(1).Height = DefaultHeight;
            sheet.Row(2).Height = DefaultHeight;
        }

        public static void ApplyHeader(IXLRange range)
        {
            range.Style.Fill.BackgroundColor = HeaderFill;
            range.Style.Font.Bold = true;
            range.Style.Font.FontColor = XLColor.Black;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ApplyBorder(range);
        }

        public static void ApplyFloorRow(IXLRange range)
        {
            range.Style.Fill.BackgroundColor = WhiteFill;
            range.Style.Font.Bold = true;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ApplyBorder(range);
        }

        public static void ApplyBody(IXLRange range)
        {
            range.Style.Fill.BackgroundColor = WhiteFill;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ApplyBorder(range);
        }

        public static void ApplySubHeader(IXLRange range)
        {
            range.Style.Fill.BackgroundColor = SubHeaderFill;
            range.Style.Font.Bold = true;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ApplyBorder(range);
        }

        public static void ApplyBorder(IXLRange range)
        {
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            range.Style.Border.TopBorderColor = BorderColor;
            range.Style.Border.BottomBorderColor = BorderColor;
            range.Style.Border.LeftBorderColor = BorderColor;
            range.Style.Border.RightBorderColor = BorderColor;
        }
    }
}