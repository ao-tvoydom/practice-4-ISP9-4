using System;
using System.Windows;
using System.Windows.Input;
using DocumentFormat.OpenXml.CustomProperties;
using Infrastructure;
using Infrastructure.Excel;
using System.Configuration;
using DataProcessing.Interfaces;
using DataProcessing.Services;
using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace UILayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;
        
        private string[] _pathToFile = null!; 
        
        const string defExtension = "xls";
        
        public MainWindow( IServiceScopeFactory serviceScopeFactory)
        {
            InitializeComponent();
            
            _serviceScopeFactory = serviceScopeFactory;

        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        
        private void CreateReport_OnClick(object sender, RoutedEventArgs e)
        {
            if (_pathToFile == null)
            {
                MessageBox.Show("Выберите файлы");
            }
            else
            {
                ResultWindow resultWindow = new ResultWindow();
                resultWindow.Show();
                this.Close();
            }
        }
        
        private void ImportFile_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"c:\",
                Multiselect = false,
                DefaultExt = defExtension,
                Filter = "xls files (*.xls)|*.xls|xlsx files (*.xlsx)|*.xlsx",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            var result = openFileDialog.ShowDialog();

            if (result != true) return;
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var dataProcessingService = sp.GetRequiredService<IDataProcessingService>();
                dataProcessingService.ExportReportToDb(openFileDialog.FileName);
            }

            MessageBox.Show("Данные успешно загружены в базу данных","Успех",MessageBoxButton.OK,MessageBoxImage.Information);


        }
        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}