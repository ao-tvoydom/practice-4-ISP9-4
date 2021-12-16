using System;
using DataProcessing.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataProcessing.Services
{
    public class DataProcessingService : IDataProcessingService, IServiceProvider
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

 
        public IServiceProvider ServiceProvider { get; }

        public object? GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}