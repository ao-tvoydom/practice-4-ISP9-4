
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;


namespace Infrastructure.Excel
{
    internal static class ExcelHelper
    {

        //TODO: Columns; lastCellRow
        public static void  AddPivotTableSheet(this XLWorkbook workbook)
        {

            var ptSheet = workbook.Worksheets.Add("Сводная таблица");

            var DataRange  = workbook.Worksheet(1).RangeUsed();
            
            var pivotTable = ptSheet.PivotTables.Add("Pivot Table", ptSheet.Cell(1, 1), DataRange);
            pivotTable.Layout = XLPivotLayout.Tabular;
            pivotTable.ClassicPivotTableLayout = true;

            pivotTable.RowLabels.Add(@"Код товара");
            pivotTable.RowLabels.Add(@"Наименование товара");
            pivotTable.RowLabels.Add(@"Бренд");
      
            pivotTable.ColumnLabels.Add(@"Подразделения");

            pivotTable.Values.Add("Сумма по полю Торг. реал-ция / Кол-во");
            pivotTable.Values.Add("Сумма по полю Торг. реал-ция / Сумма продажи");
            pivotTable.Values.Add("Сумма по полю Исх. остаток / Кол-во");
        }

       //public static void AddResultSheet(this XLWorkbook workbook)
        //{
            //var report = workbook.Worksheets.Add("ReportSheet");

            //var tableList = workbook.Worksheet(1).Cell(4, 3).CachedValue;
            
            
            
            
            
            
            
            //foreach (var row in tableList.Rows())
            //{
                //foreach (var cell in row.Cells())
                //{
                //    report.Cell(cell.Address).Value = cell.Value;
                //}
            //}
        //}
    }
}