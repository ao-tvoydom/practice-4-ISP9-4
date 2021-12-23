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
        }
        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}