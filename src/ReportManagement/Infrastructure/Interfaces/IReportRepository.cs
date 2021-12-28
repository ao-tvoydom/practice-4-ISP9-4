using System;
using System.Collections.Generic;
using Infrastructure.Model;

namespace Infrastructure.Interfaces
{
    public interface IReportRepository : IDisposable
    {
        IReadOnlyCollection<ReportData> GetAll();
        
        IReadOnlyCollection<PivotData> GetPivotData();

        ReportData GetById(int reportId);
        void Add(IReadOnlyCollection<ReportData> reportDataList);
    }
}