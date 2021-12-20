using System;
using DataProcessing.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataProcessing.Services
{
    public class DataProcessingService : IDataProcessingService
    {

        private readonly ISourceReportFileConverter _reportFileConverter;
        private readonly IReportRepository _reportRepository;
        
        public DataProcessingService(IReportRepository reportRepository, ISourceReportFileConverter reportFileConverter)
        {
            _reportFileConverter = reportFileConverter;
            _reportRepository = reportRepository;
        }
        
        public void ExportReportToDb(string filePath)
        {
            var convertedForm = _reportFileConverter.ConvertFrom(filePath);
            _reportRepository.Add(convertedForm);
        }

        public void ImportReportToExcel(string path)
        {
            var report = _reportRepository.GetAll();
            _reportFileConverter.ConvertTo(path,report);
        }
    }
}