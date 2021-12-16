using DataProcessing.Interfaces;
using Infrastructure.Interfaces;

namespace DataProcessing.Services
{
    public class DataProcessingService : IDataProcessingService
    {
        public void InsertReport(string filePath, IReportRepository reportRepository, ISourceReportFileConverter reportFileConverter)
        {
            var convertedForm = reportFileConverter.ConvertFrom(filePath);
            reportRepository.Add(convertedForm);
        }
    }
}