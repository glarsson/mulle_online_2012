using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ComponentModel;
using System.Windows.Threading;
using Mulle.Client.NetworkServiceReference;
using System.Text.RegularExpressions;

namespace Mulle.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
  
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            GlobalProperties._netWorkServiceModel = new NetworkServiceCallbackHandler();
            GlobalProperties._client = new NetworkServiceClient(new InstanceContext(GlobalProperties._netWorkServiceModel));
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (GlobalProperties._client.Login(loginAlias.Text, loginPassword.Text) == true)
            {
                GlobalProperties.localAlias = loginAlias.Text;
                //new LoungeWindow().Show();
                LoungeWindow main = new LoungeWindow();
                App.Current.MainWindow = main;
                this.Close();
                main.Show();
                GlobalProperties.clientImageCache = new Dictionary<string, byte[]>();
                
            }
            else {
                ShowNotificationWindow("Login Error", "Wrong password?");
            }
        }

        private void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsvalidCredentials(registerEmail.Text, registerPassword.Text) == true)
            {
                RegisterStatus status = GlobalProperties._client.Register(registerEmail.Text, registerAlias.Text, registerPassword.Text);
                switch (status)
                {
                    case RegisterStatus.Success:
                        ShowNotificationWindow("Registration Success", "Everything went well, you may login");
                        registerAlias.Clear();
                        registerEmail.Clear();
                        registerPassword.Clear();
                        break;
                    case RegisterStatus.EmailInUse:
                        ShowNotificationWindow("Registration Error", "E-Mail already registered");
                        registerEmail.Clear();
                        break;
                    case RegisterStatus.AliasInUse:
                        registerAlias.Clear();
                        ShowNotificationWindow("Registration Error", "Alias already registered");
                        break;
                }
            }
            else
            {
                ShowNotificationWindow("Registration Error", "Invalid E-mail or blank password"); 
             }
        }

        public bool IsvalidCredentials(string email, string password)
        {
            var isValid = true;
            var emailPattern = @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$";
            if (!Regex.IsMatch(email, emailPattern) || string.IsNullOrWhiteSpace(password))
                isValid = false;
            return isValid;
        }
        public void ShowNotificationWindow(string header, string message)
        {
            Point locationFromScreen = this.Grid.PointToScreen(new Point(0, 0));
            PresentationSource source = PresentationSource.FromVisual(this);
            System.Windows.Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
            NotificationWindowOK nw = new NotificationWindowOK(header, message);
            nw.Owner = Window.GetWindow(this);
            nw.Top = locationFromScreen.Y + this.Height - 180;
            nw.Left = locationFromScreen.X + this.Width - 202;
            nw.Show();
        }
    }
}
