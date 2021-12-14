using System.Collections.Generic;
using System.Data.SqlClient;
using Infrastructure.Model;

namespace Infrastructure.Interfaces
{
    public interface IQuery
    {
         List<ReportData> GetReportData();

         ReportData GetReportData(int reportId);

         void InsertReportData(List<ReportData> reportDataList);
         
         void InsertBlockStatus(ReportData reportData, SqlConnection sqlConnection);

         void InsertBrand(ReportData reportData, SqlConnection sqlConnection);
         
         void InsertDepartment(ReportData reportData, SqlConnection sqlConnection);
         
         int InsertDepartmentProduct(ReportData reportData, SqlConnection sqlConnection);
         
         void InsertOrder(ReportData reportData, int departmentProductId, SqlConnection sqlConnection);
         
         void InsertProduct(ReportData reportData, SqlConnection sqlConnection);
         
         void InsertSection(ReportData reportData, SqlConnection sqlConnection);
         
         void InsertStatusProduct(ReportData reportData, SqlConnection sqlConnection);
         
         void InsertUnit(ReportData reportData, SqlConnection sqlConnection);
         
    }
}