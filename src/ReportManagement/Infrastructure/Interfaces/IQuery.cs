using System.Collections.Generic;
using Infrastructure.Model;

namespace Infrastructure.Interfaces
{
    public interface IQuery
    {
         List<ReportData> GetReportData();

         ReportData GetReportData(int reportId);

         void InsertReportData(List<ReportData> list);
         
         void InsertBlockStatus(ReportData reportData);

         void InsertBrand(ReportData reportData);
         
         void InsertDepartment(ReportData reportData);
         
         void InsertDepartmentProduct(ReportData reportData);
         
         void InsertOrder(ReportData reportData);
         
         void InsertProduct(ReportData reportData);
         
         void InsertSection(ReportData reportData);
         
         void InsertStatusProduct(ReportData reportData);
         
         void InsertUnit(ReportData reportData);
    }
}