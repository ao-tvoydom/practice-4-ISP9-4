using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace UILayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int fileOpen = 0;
        
        const string defExtension = "xls";
        
        public static MainWindow Window;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Window = this;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        
        private string[] _pathToFile = null!; 
        
        const string defExtension = "xlsx";
        
        public MainWindow( IServiceScopeFactory serviceScopeFactory)
        {
            InitializeComponent();
            
            _serviceScopeFactory = serviceScopeFactory;

        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {

                MainWindow.Window.DragMove();

                this.DragMove();

            }
        }
        
        private void CreateReport_OnClick(object sender, RoutedEventArgs e)
        {

            if (fileOpen != 1)
            {
                MessageBox.Show("Выберите файлы");
            }
            else
            {
                ResultWindow resultWindow = new ResultWindow();
                resultWindow.Show();
                this.Close();
            }

            
            var saveFileDialog = new SaveFileDialog
            {
                FileName = "ExcelReport.xlsx",
                DefaultExt = defExtension,
                AddExtension = true,
                Filter = "xls files (*.xls)|*.xls|xlsx files (*.xlsx)|*.xlsx",
                InitialDirectory = @"c:\"
                
            };

            var result = saveFileDialog.ShowDialog();
   
            if (result != true) return;

            var savePath = saveFileDialog.FileName;
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var dataProcessingService = sp.GetRequiredService<IDataProcessingService>();
                dataProcessingService.ImportReportToExcel(savePath!);
            }
            
            MessageBox.Show("Отчёт создан","Успех",MessageBoxButton.OK,MessageBoxImage.Information);
            
            

        }
        
        private void ImportFile_OnClick(object sender, RoutedEventArgs e)
        {

            try
            {
                
                var openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = @"c:\",
                    Multiselect = true,
                    DefaultExt = defExtension,
                    Filter = "xls files (*.xls)|*.xls|xlsx files (*.xlsx)|*.xlsx",
                    FilterIndex = 2,
                    RestoreDirectory = true
                };

                bool? result = openFileDialog.ShowDialog();

                if (result == true) // выполняется при нажатии ОК на выборе файлов
                {
                    fileOpen = 1;
                    //openFileDialog.FileNames // массив с полными путями всех выбранных файлов
                }
                else
                {
                    MessageBox.Show("Файлы не выбраны!");
                }
                
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }

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