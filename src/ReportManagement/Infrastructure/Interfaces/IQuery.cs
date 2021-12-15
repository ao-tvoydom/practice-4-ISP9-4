using System;
using System.Collections.Generic;
using Infrastructure.Model;

namespace Infrastructure.Interfaces
{
    internal interface IReportRepository : IDisposable
    {
        IReadOnlyCollection<ReportData> GetAll();
        ReportData GetById(int reportId);
        void Add(IReadOnlyCollection<ReportData> reportDataList);
    }
}