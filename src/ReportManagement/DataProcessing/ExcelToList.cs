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
using DataProcessing.Interfaces;
using Infrastructure.Model;
using Microsoft.Office.Interop.Excel;
using DataProcessing.Interfaces;

namespace DataProcessing
{
    public class ConvertToList : IExcelConverter
    {
        public static XLWorkbook Workbook;

        List<ReportData> IExcelConverter.ExcelToList()
        {
           List<ReportData> list = new List<ReportData>();
            const int nameBlockStatusRow = 6;
            const int nameBrandRow = 5;
            const int nameDepartmentRow = 1;
            const int realizationRow = 10;
            const int productDisposalRow = 11;
            const int productSurplusRow = 12;
            const int lastShipmentDateRow = 13;
            const int lastSaleDateRow = 14;
            const int sellingPriceRow = 9;
            const int nameUnitRow = 8;
            const int codeStatusProductRow = 7;
            const int nameSectionRow = 2;
            const int codeProductRow = 3;
            const int nameProductRow = 4;
            const int expirationDateRow = 15;
            try
            {
                foreach (var sheet in Workbook.Worksheets)
                {
                    sheet.Row(1).Delete();
                    foreach (var row in sheet.Rows())
                    {
                        ReportData item = new ReportData();
                        if (row.Cell(codeProductRow).Value.ToString() == String.Empty ||
                            row.Cell(lastShipmentDateRow).Value.ToString() == "0")
                        {
                            continue;
                        }

                        item.NameBlockStatus = row.Cell(nameBlockStatusRow).Value.ToString();
                        item.NameBrand = row.Cell(nameBrandRow).Value.ToString();
                        item.NameDepartment = row.Cell(nameDepartmentRow).Value.ToString();
                        item.Realization = Convert.ToDecimal(row.Cell(realizationRow).Value);
                        item.ProductDisposal = Convert.ToDecimal(row.Cell(productDisposalRow).Value);
                        item.ProductSurplus = Convert.ToDecimal(row.Cell(productSurplusRow).Value);
                        item.LastShipmentDate =
                            DateOnly.FromDateTime(Convert.ToDateTime(row.Cell(lastShipmentDateRow).Value));
                        if (row.Cell(lastSaleDateRow).Value.ToString() == "0")
                        {
                            row.Cell(lastSaleDateRow).Value = null;
                        }
                        else
                        {
                            item.LastSaleDate =
                                DateOnly.FromDateTime(Convert.ToDateTime(row.Cell(lastSaleDateRow).Value));
                        }

                        item.SellingPrice = Convert.ToDecimal(row.Cell(sellingPriceRow).Value);
                        item.NameUnit = row.Cell(nameUnitRow).Value.ToString();
                        item.CodeStatusProduct = Convert.ToInt32(row.Cell(codeStatusProductRow).Value);
                        item.NameSection = row.Cell(nameSectionRow).Value.ToString();
                        item.CodeProduct = Convert.ToInt64(row.Cell(codeProductRow).Value);
                        item.NameProduct = row.Cell(nameProductRow).Value.ToString();
                        item.ExpirationDate = Convert.ToInt32(row.Cell(expirationDateRow).Value);
                        list.Add(item);
                    }
                }

                
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }

            return list;
        }

        void IExcelConverter.GetFile(string filePath)
        {
            GetFile(filePath);
        }

        public XLWorkbook ListToExcel()
        {
            return new XLWorkbook();
        }

        public static void GetFile(string filePath)
        {
            var fileName = filePath;
            var workbook = new XLWorkbook(fileName);
            Workbook = workbook;
        }

        
    }
}
