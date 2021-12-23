
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;


namespace Infrastructure.Excel
{
    internal static class ExcelHelper
    {

        
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

       public static void AddResultSheet(this XLWorkbook workbook)
        {
            var reportSheet = workbook.Worksheets.Add("ReportSheet");

            var pivotSheet = workbook.Worksheet(2);
            
            var pivotTable = pivotSheet.PivotTable("Pivot Table");
            
            var rowCount = pivotTable.SourceRange.RowCount();
            var columnCount = pivotTable.SourceRange.ColumnCount() 
                              * pivotTable.ColumnLabels.Count() 
                              * pivotTable.Values.Count();
            
            for (int i = 1; i < rowCount; i++)
            {

                for (int j = 1; j < columnCount; j++)
                {

                    var currentCellAddress = reportSheet.Cell(i, j).Address;
                    string formula = $"=IF('Сводная таблица'!{currentCellAddress}=\"\",\"\",'Сводная таблица'!{currentCellAddress})";
                    reportSheet.Cell(i, j).FormulaA1 = formula;

                }
            }
        }
       public static void Style(this IXLWorkbook workbook)
       {
           var ws = workbook.Worksheet(1);    
           
           
           ws.Columns().AdjustToContents(); 
           ws.Rows().AdjustToContents();
           
           var q = ws.LastRowUsed().RowNumber();
           var b = ws.LastColumnUsed().ColumnNumber();
           ws.Range(1,1,q,b).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
           ws.Range(1,1,q,b).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

           for (int i = 4; i <= ws.LastColumnUsed().ColumnNumber(); i += 3)
           {
               ws.Range(1,i,q,b).Style.Fill.BackgroundColor = XLColor.Yellow;
               
           }
           ws.Range(q,4,q,b).Style.Fill.BackgroundColor = XLColor.Alizarin;
           
       }
    }
}