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
        const int SectionNameColumn= 1;
        const int ProductCodeColumn = 2;
        const int ProductNameColumn = 3;
        const int BrandNameColumn = 4;
        const int BlockStatusNameColumn = 5;
        const int ProductStatusCode = 6;
        const int UnitNameColumn = 7;
        const int PriceColumn = 8;
        const int RealizationQuantityColumn = 9;
        const int DisposalColumn = 10;
        const int SurplusColumn = 11;
        const int LastShipmentDateColumn = 12;
        const int LastSaleDateColumn = 13;
        const int ExpirationDateColumn = 14;
        
        const int HeaderRow = 1;
        
        const int DayMarginColumn = 12;
        
        

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

                    if (sheet.Cell(sheet.FirstRowUsed().RowNumber(), sheet.FirstColumnUsed().ColumnNumber()).Value
                        .Equals("Подразделения"))
                    {
                        sheet.FirstColumnUsed().Delete();
                    }

                    if (sheet.Cell(HeaderRow, DayMarginColumn).Value.ToString()?.ToLower().Contains("запас в днях") ?? false)
                    {
                        sheet.Cell(HeaderRow,DayMarginColumn).WorksheetColumn().Delete();
                    }
                    
                    foreach (var row in sheet.Rows())
                    {
                        
                        if(!long.TryParse(row.Cell(ProductCodeColumn).Value.ToString(), out _))
                        {
                            continue;
                        }

                        var item = new ReportData
                        {
                            DepartmentName = string.Concat(sheet.Name[0].ToString().ToUpper(), sheet.Name.AsSpan(1)),
                            SectionName = row.Cell(SectionNameColumn).Value.ToString()!,
                            ProductName = row.Cell(ProductNameColumn).Value.ToString()!,
                            ProductCode = Convert.ToInt64(row.Cell(ProductCodeColumn).Value),
                            BrandName = row.Cell(BrandNameColumn).Value.ToString()!,
                            BlockStatusName = row.Cell(BlockStatusNameColumn).Value.ToString()!,
                            ProductStatusCode = int.Parse(row.Cell(ProductStatusCode).Value.ToString()!),
                            UnitName = row.Cell(UnitNameColumn).Value.ToString()!,
                            Price = Convert.ToDecimal(row.Cell(PriceColumn).Value),
                            RealizationQuantity = decimal.Parse(row.Cell(RealizationQuantityColumn).Value.ToString()!),
                            Disposal = Convert.ToDecimal(row.Cell(DisposalColumn).Value),
                            SurplusQuantity = Convert.ToDecimal(row.Cell(SurplusColumn).Value),
                            
                            LastShipmentDate = row.Cell(LastShipmentDateColumn).Value.ToString() == "0" 
                                ? new DateTime(2000,01,01) 
                                : DateTime.TryParse(row.Cell(LastShipmentDateColumn).Value.ToString(), out _) 
                                    ? DateTime.Parse(row.Cell(LastShipmentDateColumn).Value.ToString()!) 
                                    : DateTime.FromOADate(Convert.ToDouble(row.Cell(LastShipmentDateColumn).Value)),
                            
                            LastSaleDate = row.Cell(LastSaleDateColumn).Value.ToString() == "0" 
                                ? new DateTime(2000,01,01) 
                                : DateTime.TryParse(row.Cell(LastSaleDateColumn).Value.ToString(), out _) 
                                    ? DateTime.Parse(row.Cell(LastSaleDateColumn).Value.ToString()!) 
                                    : DateTime.FromOADate(Convert.ToDouble(row.Cell(LastSaleDateColumn).Value)),
                            ExpirationDate = Convert.ToInt32(row.Cell(ExpirationDateColumn).Value)
                        };
                        
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
                    p.Disposal,
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
                ws.Cell(headerRow + 1, headerColumn).Value = "Торг. реал-ция / Кол-во";
                ws.Cell(headerRow + 1, headerColumn + 1).Value = "Неторг. выбытие / Кол-во";
                ws.Cell(headerRow + 1, headerColumn + 2).Value = "Исх. остаток / Кол-во";
                
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
                            {x.RealizationQuantityTotal, x.Disposal, x.SurplusQuantityTotal});


                        ws.Cell(j + valueRow, valueColumn ).InsertData(values);
                    }

                }
                
                valueColumn += 3;
            }

            var formulaRow = ws.LastRowUsed().RowNumber() + 1;


            ws.Cell(formulaRow, 1).Value = "Общий итог";

            for (var i = 1; i <= departmentNameList.Count() * 3; i++)
            {
                var formulaColumn = i + 3;
                var firstCell = ws.Cell(3, formulaColumn).Address;
                var lastCell = ws.Cell(formulaRow - 1, formulaColumn).Address;
                string formula = $"=SUM({firstCell}:{lastCell})";
                ws.Cell(formulaRow, formulaColumn).FormulaA1 = formula;
            }
            ws.RecalculateAllFormulas();
            wb.Style();
            wb.SaveAs(path, true, true);
        }
    }
}