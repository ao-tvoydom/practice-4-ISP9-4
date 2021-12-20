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
        const int BlockStatusRow = 6;
        const int BrandRow = 5;
        const int DepartmentRow = 1;
        const int RealizationRow = 10;
        const int ProductDisposalRow = 11;
        const int ProductSurplusRow = 12;
        const int LastShipmentDateRow = 13;
        const int LastSaleDateRow = 14;
        const int SellingPriceRow = 9;
        const int UnitRow = 8;
        const int CodeStatusProductRow = 7;
        const int SectionRow = 2;
        const int CodeProductRow = 3;
        const int ProductRow = 4;
        const int ExpirationDateRow = 15;

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
                        
                        if (row.Cell(CodeProductRow).Value.ToString() == String.Empty)
                        {
                            continue;
                        }

                        item.NameBlockStatus = row.Cell(BlockStatusRow).Value.ToString()!;
                        item.NameBrand = row.Cell(BrandRow).Value!.ToString()!;
                        item.NameDepartment = row.Cell(DepartmentRow).Value!.ToString()!;
                        item.Realization = Convert.ToDecimal(row.Cell(RealizationRow).Value);
                        item.ProductDisposal = Convert.ToDecimal(row.Cell(ProductDisposalRow).Value);
                        item.ProductSurplus = Convert.ToDecimal(row.Cell(ProductSurplusRow).Value);
                        
                        if (row.Cell(LastSaleDateRow).Value.ToString() == "0")
                        {
                            item.LastSaleDate = new DateOnly(2000, 01, 01);
                        }
                        else
                        {
                            item.LastSaleDate =
                                DateOnly.FromDateTime(Convert.ToDateTime(row.Cell(LastSaleDateRow).Value));
                        }
                        
                        if (row.Cell(LastShipmentDateRow).Value.ToString() == "0")
                        {
                            item.LastShipmentDate = new DateOnly(2000, 01, 01);
                        }
                        else
                        {
                            item.LastShipmentDate =
                                DateOnly.FromDateTime(Convert.ToDateTime(row.Cell(LastShipmentDateRow).Value));
                        }

                        item.SellingPrice = Convert.ToDecimal(row.Cell(SellingPriceRow).Value);
                        item.NameUnit = row.Cell(UnitRow).Value.ToString()!;
                        item.CodeStatusProduct = Convert.ToInt32(row.Cell(CodeStatusProductRow).Value);
                        item.NameSection = row.Cell(SectionRow).Value.ToString()!;
                        item.CodeProduct = Convert.ToInt64(row.Cell(CodeProductRow).Value);
                        item.NameProduct = row.Cell(ProductRow).Value.ToString()!;
                        item.ExpirationDate = Convert.ToInt32(row.Cell(ExpirationDateRow).Value);
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
                select new { p.NameDepartment, 
                    p.NameSection, 
                    p.CodeProduct,
                    p.NameProduct,
                    p.NameBrand,
                    p.NameBlockStatus,
                    p.CodeStatusProduct,
                    p.NameUnit,
                    p.SellingPrice,
                    p.Realization,
                    p.ProductDisposal,
                    p.ProductSurplus,
                    p.LastShipmentDate,
                    p.LastSaleDate,
                    p.ExpirationDate };
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Inserting Data");

            ws.Cell(1, 1).Value = "Подразделения";
            ws.Cell(1, 2).Value = "Наименование секции";
            ws.Cell(1, 3).Value = "Код товара";
            ws.Cell(1, 4).Value = "Наименование товара";
            ws.Cell(1, 5).Value = "Бренд";
            ws.Cell(1, 6).Value = "КИП";
            ws.Cell(1, 7).Value = "Статус товара";
            ws.Cell(1, 8).Value = "ЕИ";
            ws.Cell(1, 9).Value = "Продаж. цена";
            ws.Cell(1, 10).Value = "Торг. реал-ция / Кол-во";
            ws.Cell(1, 11).Value = "Неторг. выбытие / Кол-во";
            ws.Cell(1, 12).Value = "Исх. остаток / Кол-во";
            ws.Cell(1, 13).Value = "[Последняя поставка]";
            ws.Cell(1, 14).Value = "Дата последней продажи";
            ws.Cell(1, 15).Value = "[Срок годности в днях]";
            ws.Cell(2, 1).InsertData(excelData);
            wb.SaveAs(path);
        }
    }
}