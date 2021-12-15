using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
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
                        var t = row.Cell(CodeProductRow).Value.ToString();
                        if (row.Cell(CodeProductRow).Value.ToString() == String.Empty ||
                            row.Cell(LastShipmentDateRow).Value.ToString() == "0")
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
            throw new System.NotImplementedException();
        }
    }
}