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
    static public class ExcelConverter
    {
        public static XLWorkbook Worksheet;
        static public void GetFile(string filePath)
        {
           
            string fileName = filePath;
            var workbook = new XLWorkbook(fileName);
            WorkSheet = workbook.Worksheet(1);
           
        }

        static public void SetFile()
        {
            List<ReportData> ReportList = new List<ReportData>();
            for (int i = 0; i < 10; i++)
            {
                ReportData reportData = new ReportData();
                var row = WorkSheet.Row(i);
                for (int j = 0; j < 10; j++)
                {
                    var cell = row.Cell(j);
                    switch (j)
                    {
                        case 1:
                            reportData.NameBlockStatus = cell.Value.ToString();                    
                            break;
                        case 2:
                            reportData.NameBrand = cell.Value.ToString();
                            break;
                        case 3:
                            reportData.NameDepartment = cell.Value.ToString();
                            break;
                        case 4:
                            reportData.Realization = Convert.ToDecimal(cell.Value.ToString());
                            break;
                        case 5:
                            reportData.ProductDisposal = Convert.ToDecimal(cell.Value.ToString());
                            break;
                        case 6:
                            reportData.ProductSurplus = Convert.ToDecimal(cell.Value.ToString());
                            break;
                        case 7:
                            reportData.LastShipmentDate = Convert.ToDateTime(cell.Value.ToString());
                            break;
                        case 8:
                            reportData.LastSaleDate = Convert.ToDateTime(cell.Value.ToString());
                            break;
                        case 9:
                            reportData.Id = Convert.ToInt32(cell.Value.ToString());
                            break;
                        case 10:
                            reportData.SellingPrice = Convert.ToDecimal(cell.Value.ToString());
                            break;
                        case 11:
                            reportData.NameUnit = cell.Value.ToString();
                            break;
                        case 12:
                            reportData.CodeStatusProduct = Convert.ToInt32(cell.Value.ToString());
                            break;
                        case 13:
                            reportData.NameSection = cell.Value.ToString();
                            break;
                        case 14:
                            reportData.CodeProduct = Convert.ToInt64(cell.Value.ToString());
                            break;
                        case 15:
                            reportData.NameProduct = cell.Value.ToString();
                            break;
                        case 16:
                            reportData.ExpirationDate = Convert.ToInt32(cell.Value.ToString());
                            break;
                    }
                    ReportList.Add(reportData);
                }

            }
            return ReportList;
        }
        }
    }
}