using Infrastructure.Interfaces;

namespace DataProcessing.Interfaces
{
    public interface IDataProcessingService
    {
        void InsertReport(string filePath, IReportRepository reportRepository, ISourceReportFileConverter reportFileConverter);
    }
}