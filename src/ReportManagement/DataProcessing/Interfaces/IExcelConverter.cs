using System.Collections.Generic;
using ClosedXML.Excel;
using Infrastructure.Model;

namespace DataProcessing.Interfaces
{
    public interface IExcelConverter
    {
        List<ReportData> ExcelToList();
        XLWorkbook ListToExcel();
    }
}