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
            string fileName = filePath;
            var workbook = new XLWorkbook(fileName);
            Workbook = workbook;
        }

        public static List<ReportData> ExcelToList()
        {
            
            List<ReportData> list = new List<ReportData>();
            foreach (var sheet in Workbook.Worksheets)
            {
                sheet.Row(1).Delete();
                foreach (var row in sheet.Rows())
                {
                    ReportData item = new ReportData();
                    if (row.Cell(3).Value.ToString() == String.Empty || row.Cell(13).Value.ToString() == "0")
                    {
                        continue;
                    }
                    item.NameBlockStatus = row.Cell(6).Value.ToString();
                    item.NameBrand = row.Cell(5).Value.ToString();
                    item.NameDepartment = row.Cell(1).Value.ToString();
                    item.Realization = Convert.ToDecimal(row.Cell(10).Value);
                    item.ProductDisposal = Convert.ToDecimal(row.Cell(11).Value);                   
                    item.ProductSurplus = Convert.ToDecimal(row.Cell(12).Value);                   
                    item.LastShipmentDate = Convert.ToDateTime(row.Cell(13).Value);
                    if (row.Cell(14).Value.ToString() == "0")
                    {
                        row.Cell(14).Value = null;
                    }
                    else
                    {
                        item.LastSaleDate = Convert.ToDateTime(row.Cell(14).Value);
                    }                    
                    item.SellingPrice = Convert.ToDecimal(row.Cell(9).Value);
                    item.NameUnit = row.Cell(8).Value.ToString();
                    item.CodeStatusProduct = Convert.ToInt32(row.Cell(7).Value);
                    item.NameSection = row.Cell(2).Value.ToString();
                    item.CodeProduct = Convert.ToInt64(row.Cell(3).Value);
                    item.NameProduct = row.Cell(4).Value.ToString();
                    item.ExpirationDate = Convert.ToInt32(row.Cell(15).Value);
                    list.Add(item);
                }
            }
            return list;

        }
    }
}
 