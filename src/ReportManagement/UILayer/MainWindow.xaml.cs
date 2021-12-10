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
        private string[] _pathToFile = null!; 
        
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
                Multiselect = true,
                DefaultExt = defExtension,
                Filter = "xls files (*.xls)|*.xls|xlsx files (*.xlsx)|*.xlsx",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            Nullable<bool> result = openFileDialog.ShowDialog();

                if (result == true) // выполняется при нажатии ОК на выборе файлов
                {
                    //openFileDialog.FileNames // массив с полными путями всех выбранных файлов
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