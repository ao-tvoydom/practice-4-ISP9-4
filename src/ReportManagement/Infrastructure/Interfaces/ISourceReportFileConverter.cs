using System.Collections.Generic;
using Infrastructure.Model;

namespace Infrastructure.Interfaces
{

    public interface ISourceReportFileConverter
    {
        IReadOnlyCollection<ReportData> ConvertFrom(string path);
        void ConvertTo(string path, IReadOnlyCollection<ReportData> data);
    }
}