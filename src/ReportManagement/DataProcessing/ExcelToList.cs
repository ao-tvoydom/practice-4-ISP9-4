using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML;
using ClosedXML.Excel;
using System.IO;
using System.Net.Security;
using System.Threading;
using Infrastructure.Model.ReportData;
using Microsoft.Office.Interop.Excel;

namespace DataProcessing
{
    public static class ExcelToList
    {
        public static XLWorkbook Workbook;
        public static void GetFile(string filePath)
        {
            var fileName = filePath;
            var workbook = new XLWorkbook(fileName);
            Workbook = workbook;
        }

        public static List<ReportData> ExcelToList()
        {
            
            List<ReportData> list = new List<ReportData>();
            int NameBlockStatusRow = 6;
            int NameBrandRow = 5;
            int NameDepartmentRow = 1;
            int RealizationRow = 10;
            int ProductDisposalRow = 11;
            int ProductSurplusRow = 12;
            int LastShipmentDateRow = 13;
            int LastSaleDateRow = 14;
            int SellingPriceRow = 9;
            int NameUnitRow = 8;
            int CodeStatusProductRow = 7;
            int NameSectionRow = 2;
            int CodeProductRow = 3;
            int NameProductRow = 4;
            int ExpirationDateRow = 15;
            foreach (var sheet in Workbook.Worksheets)
            {
                sheet.Row(1).Delete();
                foreach (var row in sheet.Rows())
                {
                    ReportData item = new ReportData();
                    if (row.Cell(CodeProductRow).Value.ToString() == String.Empty || row.Cell(LastShipmentDateRow).Value.ToString() == "0")
                    {
                        continue;
                    }
                    item.NameBlockStatus = row.Cell(NameBlockStatusRow).Value.ToString();
                    item.NameBrand = row.Cell(NameBrandRow).Value.ToString();
                    item.NameDepartment = row.Cell(NameDepartmentRow).Value.ToString();
                    item.Realization = Convert.ToDecimal(row.Cell(RealizationRow).Value);
                    item.ProductDisposal = Convert.ToDecimal(row.Cell(ProductDisposalRow).Value);                   
                    item.ProductSurplus = Convert.ToDecimal(row.Cell(ProductSurplusRow).Value);                   
                    item.LastShipmentDate = Convert.ToDateTime(row.Cell(LastShipmentDateRow).Value);
                    if (row.Cell(LastSaleDateRow).Value.ToString() == "0")
                    {
                        row.Cell(LastSaleDateRow).Value = null;
                    }
                    else
                    {
                        item.LastSaleDate = Convert.ToDateTime(row.Cell(LastSaleDateRow).Value);
                    }                    
                    item.SellingPrice = Convert.ToDecimal(row.Cell(SellingPriceRow).Value);
                    item.NameUnit = row.Cell(NameUnitRow).Value.ToString();
                    item.CodeStatusProduct = Convert.ToInt32(row.Cell(CodeStatusProductRow).Value);
                    item.NameSection = row.Cell(NameSectionRow).Value.ToString();
                    item.CodeProduct = Convert.ToInt64(row.Cell(CodeProductRow).Value);
                    item.NameProduct = row.Cell(NameProductRow).Value.ToString();
                    item.ExpirationDate = Convert.ToInt32(row.Cell(ExpirationDateRow).Value);
                    list.Add(item);
                }
            }
            return list;

        }
    }
}
 