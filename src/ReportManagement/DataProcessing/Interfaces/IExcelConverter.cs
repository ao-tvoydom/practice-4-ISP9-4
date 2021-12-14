using System.Collections.Generic;
using ClosedXML.Excel;
using Infrastructure.Model;

namespace DataProcessing.Interfaces
{
    public interface IExcelConverter
    {
        public List<ReportData> ExcelToList();
        public void GetFile(string filePath);
        public XLWorkbook ListToExcel();
    }
}