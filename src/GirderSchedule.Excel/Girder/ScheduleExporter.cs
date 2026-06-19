using ClosedXML.Excel;
using GirderSchedule.Domain.Girder.Models;
using GirderSchedule.Domain.Models;

namespace GirderSchedule.Excel.Girder
{
    public sealed class ScheduleExporter
    {
        public void Export(ScheduleProject project, string filePath, ExportOptions options)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Excel 저장 경로가 비어 있습니다.", nameof(filePath));
            }

            if (options == null)
            {
                options = new ExportOptions();
            }

            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var workbook = new XLWorkbook())
            {
                var sheetName = string.IsNullOrWhiteSpace(options.SheetName) ? "Girder Schedule" : options.SheetName;
                var sheet = workbook.Worksheets.Add(sheetName);

                Style.ApplySheetBase(sheet);
                DrawHeader(sheet);

                var row = Layout.FirstDataRow;

                row = DrawByFloor(sheet, project, row, options);

                if (row <= Layout.FirstDataRow)
                {
                    DrawEmptyRow(sheet, row);
                    row++;
                }

                sheet.SheetView.FreezeRows(2);
                sheet.Range(1, 1, row - 1, Layout.EndColumn).Style.Alignment.WrapText = false;
                sheet.Range(1, 1, row - 1, Layout.EndColumn).Style.Font.FontName = Style.FontName;
                sheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                sheet.PageSetup.PagesWide = 1;
                sheet.PageSetup.PagesTall = 0;
                sheet.PageSetup.Margins.Top = 0.3;
                sheet.PageSetup.Margins.Bottom = 0.3;
                sheet.PageSetup.Margins.Left = 0.25;
                sheet.PageSetup.Margins.Right = 0.25;

                workbook.SaveAs(filePath);
            }
        }

        private void DrawHeader(IXLWorksheet sheet)
        {
            sheet.Range("A1:A2").Merge().Value = "ID";
            sheet.Range("B1:B2").Merge().Value = "NAME";
            sheet.Range("C1:E1").Merge().Value = "SECTION";
            sheet.Cell("C2").Value = "( B )";
            sheet.Cell("D2").Value = "X";
            sheet.Cell("E2").Value = "( H )";
            sheet.Range("F1:F2").Merge().Value = "REBAR SECTION";
            sheet.Range("G1:G2").Merge().Value = "피복(mm)";
            sheet.Range("H1:K1").Merge().Value = "REBAR";

            sheet.Range("L1:N2").Merge().Value = "END(I)";
            sheet.Range("O1:Q2").Merge().Value = "CENTER";
            sheet.Range("R1:T2").Merge().Value = "END(J)";

            sheet.Range("H2").Value = "TYPE";
            sheet.Range("I2").Value = "DIA";
            sheet.Range("J2").Value = "LAYER";
            sheet.Range("K2").Value = "NO";

            Style.ApplyHeader(sheet.Range("A1:T2"));
        }

        private int DrawByFloor(IXLWorksheet sheet, ScheduleProject project, int startRow, ExportOptions options)
        {
            var row = startRow;

            for (var i = 0; i < project.Floors.Count; i++)
            {
                var floor = project.Floors[i];

                if (floor == null || floor.Sets == null || floor.Sets.Count == 0)
                {
                    continue;
                }

                DrawFloorRow(sheet, row, GetFloorName(floor));
                row++;

                for (var j = 0; j < floor.Sets.Count; j++)
                {
                    var set = floor.Sets[j];

                    if (set == null)
                    {
                        continue;
                    }

                    DrawSet(sheet, set, row, options);
                    row += Layout.RowsPerSet;
                }
            }

            return row;
        }

        private void DrawFloorRow(IXLWorksheet sheet, int row, string floorName)
        {
            sheet.Range(row, 1, row, Layout.EndColumn).Merge().Value = "===" + floorName + "===";
            Style.ApplyFloorRow(sheet.Range(row, 1, row, Layout.EndColumn));
            sheet.Row(row).Height = Style.FloorNameHeight;
        }

        private void DrawEmptyRow(IXLWorksheet sheet, int row)
        {
            sheet.Range(row, 1, row, Layout.EndColumn).Merge().Value = "내보낼 부재가 없습니다.";
            Style.ApplyFloorRow(sheet.Range(row, 1, row, Layout.EndColumn));
        }

        private void DrawSet(IXLWorksheet sheet, ScheduleSet set, int row, ExportOptions options)
        {
            var endRow = Layout.GetSetEndRow(row);
            var item = GetBaseItem(set);

            Style.ApplyBody(sheet.Range(row, 1, endRow, Layout.EndColumn));

            sheet.Row(row).Height = Style.DefaultHeight;
            sheet.Row(row + 1).Height = Style.DefaultHeight;
            sheet.Row(row + 2).Height = Style.DefaultHeight;
            sheet.Row(row + 3).Height = Style.DefaultHeight;
            sheet.Row(row + 4).Height = Style.DefaultHeight;

            sheet.Range(row, Layout.ColumnId, endRow, Layout.ColumnId).Merge().Value = GetMemberId(set);
            sheet.Range(row, Layout.ColumnName, endRow, Layout.ColumnName).Merge().Value = GetMemberName(set);
            sheet.Range(row, Layout.ColumnSectionWidth, endRow, Layout.ColumnSectionWidth).Merge().Value = GetWidth(item);
            sheet.Range(row, Layout.ColumnSectionSymbol, endRow, Layout.ColumnSectionSymbol).Merge().Value = "X";
            sheet.Range(row, Layout.ColumnSectionHeight, endRow, Layout.ColumnSectionHeight).Merge().Value = GetHeight(item);
            sheet.Range(row, Layout.ColumnRebarSection, endRow, Layout.ColumnRebarSection).Merge().Value = "ALL";
            sheet.Range(row, Layout.ColumnCover, endRow, Layout.ColumnCover).Merge().Value = options.DefaultCover;

            sheet.Range(row, Layout.ColumnRebarType, row + 3, Layout.ColumnRebarType).Merge().Value = "MAIN";
            sheet.Range(row, Layout.ColumnRebarDiameter, row + 3, Layout.ColumnRebarDiameter).Merge().Value = GetMainDiameter(item);
            sheet.Range(row, Layout.ColumnLayerName, row + 1, Layout.ColumnLayerName).Merge().Value = "TOP";
            sheet.Range(row + 2, Layout.ColumnLayerName, row + 3, Layout.ColumnLayerName).Merge().Value = "BOT";

            sheet.Cell(row, Layout.ColumnLayerNo).Value = 1;
            sheet.Cell(row + 1, Layout.ColumnLayerNo).Value = 2;
            sheet.Cell(row + 2, Layout.ColumnLayerNo).Value = 2;
            sheet.Cell(row + 3, Layout.ColumnLayerNo).Value = 1;

            sheet.Cell(row + 4, Layout.ColumnRebarType).Value = "STR";
            sheet.Cell(row + 4, Layout.ColumnRebarDiameter).Value = GetStirrupDiameter(item);

            Style.ApplySubHeader(sheet.Range(row, Layout.ColumnRebarType, row + 3, Layout.ColumnRebarType));
            Style.ApplySubHeader(sheet.Range(row, Layout.ColumnLayerName, row + 3, Layout.ColumnLayerName));
            Style.ApplySubHeader(sheet.Range(row + 4, Layout.ColumnRebarType, row + 4, Layout.ColumnRebarType));

            DrawPosition(sheet, set.Left, row, Layout.EndIStartColumn, Layout.EndIEndColumn);
            DrawPosition(sheet, set.Center, row, Layout.CenterStartColumn, Layout.CenterEndColumn);
            DrawPosition(sheet, set.Right, row, Layout.EndJStartColumn, Layout.EndJEndColumn);
        }

        private void DrawPosition(IXLWorksheet sheet, ScheduleItem item, int row, int startColumn, int endColumn)
        {
            DrawMainValue(sheet, row, startColumn, endColumn, GetTopFirstCount(item));
            DrawMainValue(sheet, row + 1, startColumn, endColumn, GetTopSecondCount(item));
            DrawMainValue(sheet, row + 2, startColumn, endColumn, GetBottomFirstCount(item));
            DrawMainValue(sheet, row + 3, startColumn, endColumn, GetBottomSecondCount(item));
            DrawStirrupValue(sheet, row + 4, startColumn, endColumn, item);
        }

        private void DrawMainValue(IXLWorksheet sheet, int row, int startColumn, int endColumn, int value)
        {
            sheet.Range(row, startColumn, row, endColumn).Merge().Value = value;
        }

        private void DrawStirrupValue(IXLWorksheet sheet, int row, int startColumn, int endColumn, ScheduleItem item)
        {
            var legs = GetStirrupLegs(item);
            var spacing = GetStirrupSpacing(item);

            sheet.Cell(row, startColumn).Value = legs;
            sheet.Cell(row, startColumn + 1).Value = "@";
            sheet.Cell(row, startColumn + 2).Value = spacing;
        }

        private string GetFloorName(ScheduleFloor floor)
        {
            if (floor == null || string.IsNullOrWhiteSpace(floor.Name))
            {
                return "층 이름 없음";
            }

            return floor.Name;
        }

        private string GetMemberName(ScheduleSet set)
        {
            if (set == null || string.IsNullOrWhiteSpace(set.MemberName))
            {
                return "부재명 없음";
            }

            return set.MemberName;
        }

        private ScheduleItem GetBaseItem(ScheduleSet set)
        {
            if (set == null)
            {
                return null;
            }

            if (set.Left != null && set.Left.Section != null)
            {
                return set.Left;
            }

            if (set.Center != null && set.Center.Section != null)
            {
                return set.Center;
            }

            if (set.Right != null && set.Right.Section != null)
            {
                return set.Right;
            }

            return null;
        }

        private string GetMemberId(ScheduleSet set)
        {
            return string.Empty;
        }

        private double GetWidth(ScheduleItem item)
        {
            return item == null || item.Section == null ? 0.0 : item.Section.Width;
        }

        private double GetHeight(ScheduleItem item)
        {
            return item == null || item.Section == null ? 0.0 : item.Section.Height;
        }

        private string GetMainDiameter(ScheduleItem item)
        {
            var top = item == null ? null : item.TopRebar;
            var bottom = item == null ? null : item.BottomRebar;

            if (top != null && top.Diameter > 0)
            {
                return "D" + FormatNumber(top.Diameter);
            }

            if (bottom != null && bottom.Diameter > 0)
            {
                return "D" + FormatNumber(bottom.Diameter);
            }

            return string.Empty;
        }

        private string GetStirrupDiameter(ScheduleItem item)
        {
            if (item == null || item.Stirrup == null || item.Stirrup.Diameter <= 0)
            {
                return string.Empty;
            }

            return "D" + FormatNumber(item.Stirrup.Diameter);
        }

        private int GetTopFirstCount(ScheduleItem item)
        {
            return item == null || item.TopRebar == null || item.TopRebar.FirstLayer == null ? 0 : item.TopRebar.FirstLayer.Count;
        }

        private int GetTopSecondCount(ScheduleItem item)
        {
            return item == null || item.TopRebar == null || item.TopRebar.SecondLayer == null ? 0 : item.TopRebar.SecondLayer.Count;
        }

        private int GetBottomFirstCount(ScheduleItem item)
        {
            return item == null || item.BottomRebar == null || item.BottomRebar.FirstLayer == null ? 0 : item.BottomRebar.FirstLayer.Count;
        }

        private int GetBottomSecondCount(ScheduleItem item)
        {
            return item == null || item.BottomRebar == null || item.BottomRebar.SecondLayer == null ? 0 : item.BottomRebar.SecondLayer.Count;
        }

        private int GetStirrupLegs(ScheduleItem item)
        {
            return item == null || item.Stirrup == null ? 0 : item.Stirrup.Legs;
        }

        private double GetStirrupSpacing(ScheduleItem item)
        {
            return item == null || item.Stirrup == null ? 0.0 : item.Stirrup.Spacing;
        }

        private string FormatNumber(double value)
        {
            return value.ToString("0");
        }
    }
}