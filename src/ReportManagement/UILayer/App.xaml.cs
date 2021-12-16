using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DocumentFormat.OpenXml.Drawing.Charts;
using Infrastructure;
using Infrastructure.Excel;
using Infrastructure.Factory;
using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddTransient<IReportRepository>(x => new ReportRepository(ConfigurationManager.ConnectionStrings["DatabaseEntities"].ConnectionString) );
            services.AddSingleton<ISourceReportFileConverter>(new ExcelReportFileConverter());
            services.AddSingleton<MainWindow>();
        }
    
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}