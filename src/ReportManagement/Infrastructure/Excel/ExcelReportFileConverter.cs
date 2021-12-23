using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Infrastructure.Interfaces;
using Infrastructure.Model;

namespace Infrastructure.Excel
{

    //TODO: TotalSum, styling,
    public class ExcelReportFileConverter : ISourceReportFileConverter
    {
        const int DepartmentNameRow = 1;
        const int ProductCodeRow = 2;
        const int ProductNameRow = 3;
        const int BrandNameRow = 4;
        const int RealizationQuantityRow = 5;
        const int RealizationSumRow = 6;
        const int SurplusQuantityRow = 7;   

        public IReadOnlyCollection<ReportData> ConvertFrom(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new InvalidOperationException("File in provided path is not exists");

            var list = new List<ReportData>();
            try
            {
                var workbook = new XLWorkbook(path);

                foreach (var sheet in workbook.Worksheets)
                {
                    sheet.Row(1).Delete();
                    foreach (var row in sheet.Rows())
                    {
                        var item = new ReportData();

                        if (row.Cell(ProductCodeRow).Value.ToString() == String.Empty)
                        {
                            continue;
                        }

                        item.DepartmentName = row.Cell(DepartmentNameRow).Value.ToString();
                        item.ProductCode = Convert.ToInt64(row.Cell(ProductCodeRow).Value.ToString());                        
                        item.ProductName = row.Cell(ProductNameRow).Value.ToString();
                        item.BrandName = row.Cell(BrandNameRow).Value.ToString();
                        item.RealizationQuantity = Convert.ToDecimal(row.Cell(RealizationQuantityRow).Value);                        
                        item.RealizationSum = Convert.ToDecimal(row.Cell(RealizationSumRow).Value);
                        item.SurplusQuantity = Convert.ToDecimal(row.Cell(SurplusQuantityRow).Value.ToString());                       
                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return list;
        }

        public void ConvertTo(string path, IReadOnlyCollection<ReportData> data)
        {
           var excelData = from p in data 
                select new { p.DepartmentName, 
                    p.ProductCode,
                    p.ProductName,
                    p.BrandName,
                    p.RealizationQuantity,
                    p.RealizationSum,
                    p.SurplusQuantity};
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Report");


            ws.Cell(1, 1).Value = "Подразделения";
            ws.Cell(1, 2).Value = "Код товара";
            ws.Cell(1, 3).Value = "Наименование товара";
            ws.Cell(1, 4).Value = "Бренд";
            ws.Cell(1, 5).Value = "Сумма по полю Торг. реал-ция / Кол-во";
            ws.Cell(1, 6).Value = "Сумма по полю Торг. реал-ция / Сумма продажи";
            ws.Cell(1, 7).Value = "Сумма по полю Исх. остаток / Кол-во";
            ws.Cell(2, 1).InsertData(excelData);
            
            wb.SaveAs(path);

        }

        public void ConvertTo(string path, IReadOnlyCollection<PivotData> data)
        {
            
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Report");

            var departmentNameList = data.Select(x => x.DepartmentName).Distinct().ToList();

            //Department headers

            const int headerRow = 1;
            var headerColumn = 4;
            foreach (var departmentName in departmentNameList)
            {
                
                ws.Range(headerRow, headerColumn, headerRow, headerColumn + 2)
                    .Row(headerRow)
                    .Merge()
                    .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                
                ws.Cell(headerRow, headerColumn).Value = departmentName;
                ws.Cell(headerRow + 1, headerColumn).Value = "Кол-во";
                ws.Cell(headerRow + 1, headerColumn + 1).Value = "Продажи";
                ws.Cell(headerRow + 1, headerColumn + 2).Value = "Остаток";
                
                headerColumn += 3;

            }
            
            //Product headers
            
            ws.Cell(2, 1).Value = "Код продукта";
            ws.Cell(2, 2).Value = "Наименование товара";
            ws.Cell(2, 3).Value = "Бренд";
            
            //PivotTable Rows

            var rowData = data
                .DistinctBy(x => x.ProductCode)
                .Select(x=> new {x.ProductCode, x.ProductName, x.BrandName})
                .OrderBy(x=> x.ProductCode).ToList();

            ws.Cell(3, 1).InsertData(rowData);
            
            //PivotTable Values

            const int valueRow = 2; 
            var valueColumn = 4;


            for (var i = 1; i <= departmentNameList.Count(); i++)
            {
                for (var j = 1; j <= rowData.Count(); j++)
                {

                    string currentDepartmentName = departmentNameList[i-1];
                    var currentProductCode = rowData[j-1].ProductCode;
                    var currentData = data.Where(x => x.DepartmentName == currentDepartmentName
                                                      && x.ProductCode == currentProductCode);

                    if (currentData.Any())
                    {
                        var values = currentData.Select(x => new
                            {x.RealizationQuantityTotal, x.RealizationSumTotal, x.SurplusQuantityTotal});


                        ws.Cell(j + valueRow, valueColumn ).InsertData(values);
                    }

                }
                
                valueColumn += 3;
                
            }
                
   
            wb.SaveAs(path);

        }
        
        public static void Coloring(string path, IReadOnlyCollection<ReportData> data)
        {
           var wb = new XLWorkbook();
           var ws = wb.Worksheets.Add("Inserting Data");          
           ws.Column("J").Sort(XLSortOrder.Descending);
           ws.Range("d2", "f1230").Style.Fill.BackgroundColor = XLColor.Yellow;
           ws.Range("j2", "l1230").Style.Fill.BackgroundColor = XLColor.Yellow;
           ws.Range("p2", "r1230").Style.Fill.BackgroundColor = XLColor.Yellow;
           ws.Range("d1231", "r1231").Style.Fill.BackgroundColor = XLColor.Red;
           ws.Range("d1232", "f1233").Style.Fill.BackgroundColor = XLColor.Yellow;
           ws.Range("j1232", "l1233").Style.Fill.BackgroundColor = XLColor.Yellow;
           ws.Range("p1232", "r1233").Style.Fill.BackgroundColor = XLColor.Yellow;
        }
    }
}