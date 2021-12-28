using Infrastructure.Interfaces;

namespace DataProcessing.Interfaces
{
    public interface IDataProcessingService
    {
        void ExportReportToDb(string filePath);

        void ImportReportToExcel(string path);

    }
}