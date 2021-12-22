using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Infrastructure.Interfaces;
using Infrastructure.Model;

namespace Infrastructure.Excel
{

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
                    p.SurplusQuantity,
                            };
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Report");
            ws.Cell(2, 1).InsertData(excelData);
            
            wb.AddPivotTableSheet();
            
            wb.SaveAs(path);
        }
        public static void Coloring(string path, IReadOnlyCollection<ReportData> data)
        {
           var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Inserting Data");          
            //ws.Column("J").Sort(XLSortOrder.Descending);
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