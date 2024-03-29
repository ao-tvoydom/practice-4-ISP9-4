﻿using System.Windows;
using System.Windows.Input;
using System.Net.Mail;
using System.Net;

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
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress addressFrom = new MailAddress("fill_SendingEmailAddress", "fill_Name");
            // кому отправляем
            MailAddress addressTo = new MailAddress("fill_RecievingEmailAddress");
            // создаем объект сообщения
            MailMessage message = new MailMessage(addressFrom, addressTo);
            // тема письма
            message.Subject = "fill_Subject";
            // текст письма
            message.Body = "<h2>Текст-письма-в-виде-html</h2>";
            // письмо представляет код html (если нужно)
            message.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // логин и пароль отправителя
            smtp.Credentials = new NetworkCredential("fill_SendingEmailAddress", "fill_SendingEmailPassword");
            smtp.EnableSsl = true;
            smtp.Send(message);
            
            //обязательная подтверждающая обратная связь
            //(при надобности добавить информацию из письма в сообщение)
            MessageBox.Show("Письмо отправленно");
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