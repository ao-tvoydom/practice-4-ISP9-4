using System.Windows;
using System.Windows.Input;

namespace UILayer
{
    public partial class ResultWindow : Window
    {
        public static ResultWindow Window;
        public ResultWindow()
        {
            InitializeComponent();
        }
        
        private void ResultWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Window = this;
        }
        
        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                ResultWindow.Window.DragMove();
            }
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