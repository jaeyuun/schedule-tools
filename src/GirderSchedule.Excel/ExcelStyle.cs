using ClosedXML.Excel;
using GirderSchedule.Excel.Layouts;

namespace GirderSchedule.Excel
{
    public static class ExcelStyle
    {
        public static readonly XLColor HeaderFill = XLColor.FromHtml("#BFBFBF");
        public static readonly XLColor SubHeaderFill = XLColor.FromHtml("#EDEDED");
        public static readonly XLColor WhiteFill = XLColor.White;
        public static readonly XLColor BorderColor = XLColor.Black;

        public const string FontName = "맑은 고딕";
        public const int FontSize = 10;

        public static void ApplySheetBase(IXLWorksheet sheet)
        {
            ApplyDefaultFont(sheet);
            ApplyDefaultAlignment(sheet);
            ApplyDefaultColumnWidth(sheet);
            ApplyColumnWidths(sheet);
            ApplyHeaderRowHeights(sheet);
        }

        public static void ApplyHeader(IXLRange range)
        {
            range.Style.Fill.BackgroundColor = HeaderFill;
            range.Style.Font.Bold = true;
            range.Style.Font.FontColor = XLColor.Black;
            ApplyCenterAlignment(range);
            ApplyBorder(range);
        }

        public static void ApplyFloorRow(IXLRange range)
        {
            range.Style.Fill.BackgroundColor = WhiteFill;
            range.Style.Font.Bold = true;
            ApplyCenterAlignment(range);
            ApplyBorder(range);
        }

        public static void ApplyBody(IXLRange range)
        {
            range.Style.Fill.BackgroundColor = WhiteFill;
            ApplyCenterAlignment(range);
            ApplyBorder(range);
        }

        public static void ApplySubHeader(IXLRange range)
        {
            range.Style.Fill.BackgroundColor = SubHeaderFill;
            range.Style.Font.Bold = true;
            ApplyCenterAlignment(range);
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
        
        private static void ApplyDefaultFont(IXLWorksheet sheet)
        {
            sheet.Style.Font.FontName = FontName;
            sheet.Style.Font.FontSize = FontSize;
        }

        private static void ApplyDefaultAlignment(IXLWorksheet sheet)
        {
            sheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private static void ApplyCenterAlignment(IXLRange range)
        {
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private static void ApplyDefaultColumnWidth(IXLWorksheet sheet)
        {
            sheet.Columns().Width = ExcelLayout.DefaultColumnWidth;
        }

        private static void ApplyColumnWidths(IXLWorksheet sheet)
        {
            for (var i = 0; i < ExcelLayout.ColumnWidths.Length; i++)
            {
                var column = ExcelLayout.ColumnWidths[i];
                sheet.Column(column.ColumnName).Width = column.Width;
            }
        }

        private static void ApplyHeaderRowHeights(IXLWorksheet sheet)
        {
            sheet.Row(1).Height = ExcelLayout.HeaderRowHeight;
            sheet.Row(2).Height = ExcelLayout.HeaderRowHeight;
        }
    }
}