using System.Windows;

namespace UILayer
{
    public partial class ResultWindow : Window
    {
        public ResultWindow()
        {
            InitializeComponent();
        }

        private void SendByEmail_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            
        }
        
        private void UploadExcelFile_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
        
        private void ExitBut_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}