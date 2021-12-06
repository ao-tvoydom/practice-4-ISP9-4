using System;
using System.Windows;
using System.IO;
using Microsoft.Win32;

namespace UILayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Create_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
        
        private void Import_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = @"c:\";
            openFileDialog.Multiselect = true;
            const string defExtension = "xls";
            openFileDialog.DefaultExt = defExtension;
            openFileDialog.Filter = "xls files (*.xls)|*.xls|xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                //openFileDialog.FileNames // массив с полными путями всех выбранных файлов
                // выполняется при нажатии ОК на выборе файлов
            }
            else
            {
                MessageBox.Show("Файлы не выбраны");
            }
        }

    }
}