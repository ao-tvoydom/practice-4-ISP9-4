using System.Collections.Generic;
using Infrastructure.Model;

namespace Infrastructure.Interfaces
{
    public interface IQuery
    {
         List<ReportData> GetReportData();

         ReportData GetReportData(int reportId);

    }
}