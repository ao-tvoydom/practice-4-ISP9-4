
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;


namespace Infrastructure.Excel
{
    internal static class ExcelHelper
    {

        //TODO: Columns; lastCellRow
        public static XLWorkbook AddPivotTableSheet(this XLWorkbook workbook)
        {

            var ptSheet = workbook.Worksheets.Add("Сводная таблица");


            var DataRange = workbook.Worksheet(1).Range(1,1,997,15);
            
            var pivotTable = ptSheet.PivotTables.Add("Pivot Table", ptSheet.Cell(1, 1), DataRange);
            
            pivotTable.RowLabels.Add(@"Статус товара");
            pivotTable.RowLabels.Add(@"Наименование секции");
            pivotTable.RowLabels.Add(@"Бренд");
            pivotTable.RowLabels.Add(@"Наименование товара");
      
            pivotTable.ColumnLabels.Add(@"КИП");

            pivotTable.Values.Add(@"Торг. реал-ция / Кол-во");
            

            return workbook;
        }
        
        
    }
}