using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DataProcessing.Interfaces;
using DataProcessing.Services;
using DocumentFormat.OpenXml.Drawing.Charts;
using Infrastructure;
using Infrastructure.Excel;
using Infrastructure.Factory;
using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;


namespace UILayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
    
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDBConnectionFactory,DbConnectionFactory>();
            services.AddScoped<ISourceReportFileConverter, ExcelReportFileConverter>();
            services.AddScoped<IReportRepository,ReportRepository>();
            services.AddScoped<IDataProcessingService, DataProcessingService>();
            services.AddScoped<IServiceScopeFactory,ServiceScopeFactory>();

            services.AddSingleton<MainWindow>();
            
        }
    
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}