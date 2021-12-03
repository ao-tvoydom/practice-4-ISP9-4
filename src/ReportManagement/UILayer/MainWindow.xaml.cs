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

        private void ImportFile_OnClick(object sender, RoutedEventArgs e)
        {
            

        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}