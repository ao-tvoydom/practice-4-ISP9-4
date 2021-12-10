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
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = @"c:\";
            openFileDialog.Multiselect = true;
            const string defExtension = "xls";
            openFileDialog.DefaultExt = defExtension;
            openFileDialog.Filter = "xls files (*.xls)|*.xls|xlsx files (*.xlsx)|*.xlsx";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                _pathToFile = openFileDialog.FileNames;
                //openFileDialog.FileNames // массив с полными путями всех выбранных файлов
                // выполняется при нажатии ОК на выборе файлов
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