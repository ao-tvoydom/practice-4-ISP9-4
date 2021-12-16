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
        
        //private readonly IDataProcessingService _dataProcessingService;
        private readonly IServiceProvider _dataProcessingService;
        
        private string[] _pathToFile = null!; 
        
        const string defExtension = "xls";
        
        public static MainWindow Window;
        public MainWindow(IServiceProvider dataProcessingService)
        {
            InitializeComponent();
            
            //_dataProcessingService = dataProcessingService;
            _dataProcessingService = dataProcessingService;
            
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Window = this;
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                MainWindow.Window.DragMove();
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

            bool? result = openFileDialog.ShowDialog();

                if (result == true) 
                {

                    using (var scope = _dataProcessingService.CreateScope())
                    {
                        var sp = scope.ServiceProvider;
                        var dataProcessingService = sp.GetRequiredService<DataProcessingService>();
                        dataProcessingService.ExportReportToDb(openFileDialog.FileName);
                    }
                    
                    
                    
                    
                    MessageBox.Show("Радость");
                }
                else
                {
                    MessageBox.Show("Файлы не выбраны");
                }
        }
        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}