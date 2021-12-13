using ClosedXML;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Threading;
using Infrastructure.Model.ReportData;

namespace DataProcessing
{
    public static class ExcelConverter
    {
        public static XLWorkbook Worksheet;
        public static void GetFile(string filePath)
        {
           
            string fileName = filePath;
            var workbook = new XLWorkbook(fileName);
            WorkSheet = workbook.Worksheet(1);
           
        }

        public static List<ReportData> ExcelToList()
        {
            List<ReportData> list = new List<ReportData>();
            foreach (DataRow row in data.Rows)
            {
                ReportData item = new ReportData();
                item.NameBlockStatus = row[5].ToString();
                item.NameBrand  = row[4].ToString();
                item.NameDepartment  = row[0].ToString();
                item.Realization = Convert.ToDecimal(row[9].ToString());
                item.ProductDisposal = Convert.ToDecimal(row[10].ToString());
                item.ProductSurplus = Convert.ToDecimal(row[11].ToString());
                item.LastShipmentDate = Convert.ToDateTime(row[13].ToString());
                item.LastSaleDate = Convert.ToDateTime(row[14].ToString());
                item.SellingPrice = Convert.ToDecimal(row[8].ToString());
                item.NameUnit  = row[7].ToString();
                item.CodeStatusProduct  = Convert.ToInt32(row[6].ToString());
                item.NameSection = row[1].ToString();
                item.CodeProduct = Convert.ToInt64(row[2].ToString());
                item.NameProduct = row[3].ToString();
                item.ExpirationDate = Convert.ToInt64(row[15].ToString());
                list.Add(item);
            }
            return list;
            
        }
        
    }
}