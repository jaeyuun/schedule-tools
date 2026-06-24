using ClosedXML.Excel;
using GirderSchedule.Domain.Models;
using GirderSchedule.Excel.Constants;
using GirderSchedule.Excel.Layouts;
using GirderSchedule.Excel.Utils;

namespace GirderSchedule.Excel
{
    public sealed class ExcelExporter
    {
        private readonly ExcelValueMapper _mapper = new ExcelValueMapper();

        public void Export(ScheduleProject project, string filePath, ExcelExportOptions options)
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
                options = new ExcelExportOptions();
            }

            ExcelFileUtil.EnsureDirectory(filePath);

            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add(ExcelSheetNameUtil.GetSheetName(options));
                var lastRow = DrawSheet(sheet, project, options);

                ApplyPageSetup(sheet, lastRow);
                workbook.SaveAs(filePath);
            }
        }

        private int DrawSheet(IXLWorksheet sheet, ScheduleProject project, ExcelExportOptions options)
        {
            ExcelStyle.ApplySheetBase(sheet);
            DrawHeader(sheet);

            var row = ExcelLayout.FirstDataRow;
            row = DrawByFloor(sheet, project, row, options);

            if (row <= ExcelLayout.FirstDataRow)
            {
                DrawEmptyRow(sheet, row);
                row++;
            }

            return row - 1;
        }

        private void DrawHeader(IXLWorksheet sheet)
        {
            sheet.Range("A1:A2").Merge().Value = ExcelLabels.Id;
            sheet.Range("B1:B2").Merge().Value = ExcelLabels.Name;
            sheet.Range("C1:E1").Merge().Value = ExcelLabels.Section;
            sheet.Cell("C2").Value = ExcelLabels.SectionWidth;
            sheet.Cell("D2").Value = ExcelLabels.SectionSymbol;
            sheet.Cell("E2").Value = ExcelLabels.SectionHeight;
            sheet.Range("F1:F2").Merge().Value = ExcelLabels.RebarSectionHeader;
            sheet.Range("G1:G2").Merge().Value = ExcelLabels.Cover;
            sheet.Range("H1:K1").Merge().Value = ExcelLabels.Rebar;

            sheet.Range("L1:N2").Merge().Value = ExcelLabels.EndI;
            sheet.Range("O1:Q2").Merge().Value = ExcelLabels.Center;
            sheet.Range("R1:T2").Merge().Value = ExcelLabels.EndJ;

            sheet.Range("H2").Value = ExcelLabels.Type;
            sheet.Range("I2").Value = ExcelLabels.Diameter;
            sheet.Range("J2").Value = ExcelLabels.Layer;
            sheet.Range("K2").Value = ExcelLabels.No;

            ExcelStyle.ApplyHeader(sheet.Range("A1:T2"));
        }

        private int DrawByFloor(IXLWorksheet sheet, ScheduleProject project, int startRow, ExcelExportOptions options)
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
                    row += ExcelLayout.RowsPerSet;
                }
            }

            return row;
        }

        private void DrawFloorRow(IXLWorksheet sheet, int row, string floorName)
        {
            sheet.Range(row, 1, row, ExcelLayout.EndColumn).Merge().Value = "===" + floorName + "===";
            ExcelStyle.ApplyFloorRow(sheet.Range(row, 1, row, ExcelLayout.EndColumn));
            sheet.Row(row).Height = ExcelLayout.FloorNameRowHeight;
        }

        private void DrawEmptyRow(IXLWorksheet sheet, int row)
        {
            sheet.Range(row, 1, row, ExcelLayout.EndColumn).Merge().Value = ExcelLabels.EmptyMessage;
            ExcelStyle.ApplyFloorRow(sheet.Range(row, 1, row, ExcelLayout.EndColumn));
        }

        private void DrawSet(IXLWorksheet sheet, ScheduleSet set, int row, ExcelExportOptions options)
        {
            var endRow = ExcelLayout.GetSetEndRow(row);
            var item = _mapper.GetBaseItem(set);

            ApplySetBaseStyle(sheet, row, endRow);
            DrawSetInfo(sheet, set, item, row, endRow, options);
            DrawRebarInfo(sheet, item, row);
            DrawPositionValues(sheet, set, row);
        }

        private void ApplySetBaseStyle(IXLWorksheet sheet, int row, int endRow)
        {
            ExcelStyle.ApplyBody(sheet.Range(row, 1, endRow, ExcelLayout.EndColumn));

            for (var i = row; i <= endRow; i++)
            {
                sheet.Row(i).Height = ExcelLayout.DefaultRowHeight;
            }
        }

        private void DrawSetInfo(IXLWorksheet sheet, ScheduleSet set, ScheduleItem? item, int row, int endRow, ExcelExportOptions options)
        {
            sheet.Range(row, ExcelLayout.ColumnId, endRow, ExcelLayout.ColumnId).Merge().Value = _mapper.GetMemberId(set);
            sheet.Range(row, ExcelLayout.ColumnName, endRow, ExcelLayout.ColumnName).Merge().Value = _mapper.GetMemberName(set);
            sheet.Range(row, ExcelLayout.ColumnSectionWidth, endRow, ExcelLayout.ColumnSectionWidth).Merge().Value = _mapper.GetWidth(item);
            sheet.Range(row, ExcelLayout.ColumnSectionSymbol, endRow, ExcelLayout.ColumnSectionSymbol).Merge().Value = ExcelLabels.SectionSymbol;
            sheet.Range(row, ExcelLayout.ColumnSectionHeight, endRow, ExcelLayout.ColumnSectionHeight).Merge().Value = _mapper.GetHeight(item);
            sheet.Range(row, ExcelLayout.ColumnRebarSection, endRow, ExcelLayout.ColumnRebarSection).Merge().Value = ExcelLabels.RebarSection;
            sheet.Range(row, ExcelLayout.ColumnCover, endRow, ExcelLayout.ColumnCover).Merge().Value = options.DefaultCover;
        }

        private void DrawRebarInfo(IXLWorksheet sheet, ScheduleItem? item, int row)
        {
            sheet.Range(row, ExcelLayout.ColumnRebarType, row + 3, ExcelLayout.ColumnRebarType).Merge().Value = ExcelLabels.MainRebarType;
            sheet.Range(row, ExcelLayout.ColumnRebarDiameter, row + 3, ExcelLayout.ColumnRebarDiameter).Merge().Value = _mapper.GetMainDiameter(item);
            sheet.Range(row, ExcelLayout.ColumnLayerName, row + 1, ExcelLayout.ColumnLayerName).Merge().Value = ExcelLabels.TopLayerName;
            sheet.Range(row + 2, ExcelLayout.ColumnLayerName, row + 3, ExcelLayout.ColumnLayerName).Merge().Value = ExcelLabels.BottomLayerName;

            sheet.Cell(row, ExcelLayout.ColumnLayerNo).Value = 1;
            sheet.Cell(row + 1, ExcelLayout.ColumnLayerNo).Value = 2;
            sheet.Cell(row + 2, ExcelLayout.ColumnLayerNo).Value = 2;
            sheet.Cell(row + 3, ExcelLayout.ColumnLayerNo).Value = 1;

            sheet.Cell(row + 4, ExcelLayout.ColumnRebarType).Value = ExcelLabels.StirrupRebarType;
            sheet.Cell(row + 4, ExcelLayout.ColumnRebarDiameter).Value = _mapper.GetStirrupDiameter(item);

            ExcelStyle.ApplySubHeader(sheet.Range(row, ExcelLayout.ColumnRebarType, row + 3, ExcelLayout.ColumnRebarType));
            ExcelStyle.ApplySubHeader(sheet.Range(row, ExcelLayout.ColumnLayerName, row + 3, ExcelLayout.ColumnLayerName));
            ExcelStyle.ApplySubHeader(sheet.Range(row + 4, ExcelLayout.ColumnRebarType, row + 4, ExcelLayout.ColumnRebarType));
        }

        private void DrawPositionValues(IXLWorksheet sheet, ScheduleSet set, int row)
        {
            DrawPosition(sheet, set.Left, row, ExcelLayout.EndIStartColumn, ExcelLayout.EndIEndColumn);
            DrawPosition(sheet, set.Center, row, ExcelLayout.CenterStartColumn, ExcelLayout.CenterEndColumn);
            DrawPosition(sheet, set.Right, row, ExcelLayout.EndJStartColumn, ExcelLayout.EndJEndColumn);
        }

        private void DrawPosition(IXLWorksheet sheet, ScheduleItem? item, int row, int startColumn, int endColumn)
        {
            DrawMainValue(sheet, row, startColumn, endColumn, _mapper.GetTopFirstCount(item));
            DrawMainValue(sheet, row + 1, startColumn, endColumn, _mapper.GetTopSecondCount(item));
            DrawMainValue(sheet, row + 2, startColumn, endColumn, _mapper.GetBottomFirstCount(item));
            DrawMainValue(sheet, row + 3, startColumn, endColumn, _mapper.GetBottomSecondCount(item));
            DrawStirrupValue(sheet, row + 4, startColumn, item);
        }

        private void DrawMainValue(IXLWorksheet sheet, int row, int startColumn, int endColumn, XLCellValue value)
        {
            sheet.Range(row, startColumn, row, endColumn).Merge().Value = value;
        }

        private void DrawStirrupValue(IXLWorksheet sheet, int row, int startColumn, ScheduleItem? item)
        {
            sheet.Cell(row, startColumn).Value = _mapper.GetStirrupLegs(item);
            sheet.Cell(row, startColumn + 1).Value = ExcelLabels.StirrupAt;
            sheet.Cell(row, startColumn + 2).Value = _mapper.GetStirrupSpacing(item);
        }

        private void ApplyPageSetup(IXLWorksheet sheet, int lastRow)
        {
            sheet.SheetView.FreezeRows(2);
            sheet.Range(1, 1, lastRow, ExcelLayout.EndColumn).Style.Alignment.WrapText = false;
            sheet.Range(1, 1, lastRow, ExcelLayout.EndColumn).Style.Font.FontName = ExcelStyle.FontName;
            sheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            sheet.PageSetup.PagesWide = 1;
            sheet.PageSetup.PagesTall = 0;
            sheet.PageSetup.Margins.Top = 0.3;
            sheet.PageSetup.Margins.Bottom = 0.3;
            sheet.PageSetup.Margins.Left = 0.25;
            sheet.PageSetup.Margins.Right = 0.25;
        }

        private string GetFloorName(ScheduleFloor? floor)
        {
            if (floor == null || string.IsNullOrWhiteSpace(floor.Name))
            {
                return ExcelLabels.EmptyFloorName;
            }

            return floor.Name;
        }
    }
}