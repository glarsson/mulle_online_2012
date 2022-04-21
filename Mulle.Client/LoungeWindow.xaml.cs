using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ComponentModel;
using System.Windows.Threading;
using Mulle.Client.NetworkServiceReference;

namespace Mulle.Client
{
    /// <summary>
    /// Interaction logic for LoungeWindow.xaml
    /// </summary>
    public partial class LoungeWindow : Window, INotifyPropertyChanged
    {
        private StringBuilder chatOutputLog = new StringBuilder();
        
        private List<string> _connectedPlayersList = new List<string>();
        public List<string> _ConnectedPlayersList
        {
            get { return _connectedPlayersList; }
            set
            {
                if (_connectedPlayersList != value)
                {
                    _connectedPlayersList = value;
                    OnPropertyChanged("_ConnectedPlayersList");
                }
            }
        }

        public LoungeWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            GlobalProperties._netWorkServiceModel.PropertyChanged += new PropertyChangedEventHandler(_netWorkServiceModel_PropertyChanged);
            string localPlayerJoinedText = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + "*** You have joined the chat ***\n";
            chatOutputLog.Append(localPlayerJoinedText);
            chatOutput.Text = localPlayerJoinedText;
            loggedInAsLabel.Content = "Logged in as: " + GlobalProperties.localAlias;
            UpdateConnectedPlayersLocally();

        }

       private void ChatEnterKeyHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GlobalProperties._client.PlayerToServerChat(GlobalProperties.localAlias, chatInput.Text);
                chatInput.Text = String.Empty;
            }
        }
       public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                var handler = PropertyChanged;
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        

        void _netWorkServiceModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            var chatOutput = ((NetworkServiceCallbackHandler)sender).ChatOutput;
            var connectedPlayersList = ((NetworkServiceCallbackHandler)sender).connectedPlayersList;
            var incomingGameRequest = ((NetworkServiceCallbackHandler)sender).incomingGameRequest;
            
            if (e.PropertyName == "ChatOutput")
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() => { UpdateChatWindow(chatOutput); })
                );
            }
            if (e.PropertyName == "connectedPlayersList")
            {
                UpdateConnectedPlayersLocally();
            }
            if (e.PropertyName == "IncomingGameRequest")
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() => { IncomingGameRequest(incomingGameRequest); })
                );
            }
        }

        void UpdateChatWindow(string chatUpdateString)
        {
            StringBuilder timestamp = new StringBuilder();
            timestamp.Append("[");
            timestamp.Append(DateTime.Now.ToString("HH:mm:ss"));
            timestamp.Append("] ");
            chatOutputLog.Append(timestamp);
            chatOutputLog.Append(chatUpdateString);
            chatOutput.Text = String.Empty;
            chatOutput.Text = chatOutputLog.ToString();
            //chatOutput.Focus();
            chatOutput.CaretIndex = chatOutput.Text.Length;
            chatOutput.ScrollToEnd();
            System.Windows.Threading.Dispatcher.Run();
        }

        void IncomingGameRequest(string requestorAlias)
        {
            if (ShowNotificationWindowYesNo("Game Request", requestorAlias + " wants to play a game, do you accept?") == true)
            {
                GlobalProperties._client.PlayerToServerConfirmGame(requestorAlias, GlobalProperties.localAlias);
            }
            else
            {

            }
            System.Windows.Threading.Dispatcher.Run();
        }

        List<PlayerContract> GetConnectedPlayersFullContract()
        {
            return GlobalProperties._client.GetAllConnectedPlayers();
        }

        List<string> GetConnectedPlayersAliasOnly()
        {
            List<PlayerContract> tempPlayerContracts = new List<PlayerContract>();
            tempPlayerContracts = GlobalProperties._client.GetAllConnectedPlayers();
            List<string> tempPlayerList = new List<string>();
            foreach (PlayerContract p in tempPlayerContracts)
            {
                tempPlayerList.Add(p.Alias);
            }
            return tempPlayerList;

        }

        void UpdateConnectedPlayersLocally()
        {
            List<string> tempConnectedPlayers = new List<string>();
            tempConnectedPlayers = GetConnectedPlayersAliasOnly();
            _ConnectedPlayersList = tempConnectedPlayers;
            //System.Windows.Threading.Dispatcher.Run();
        }
        

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //base.OnClosing(e);
            try
            {
                GlobalProperties._client.Logout(GlobalProperties.localAlias, 0);
            }
            catch (Exception)
            {
            }
            Application.Current.Shutdown();
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            EditProfile editProfileWindow = new EditProfile();
            editProfileWindow.Show();
        }

        private void PlayerMenu_ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            if (connectedPlayersListBox.SelectedIndex == -1)
            {
                return;
            }
            string tempListBoxItem = (string)connectedPlayersListBox.SelectedItem;
            ViewProfile viewProfileWindow = new ViewProfile(tempListBoxItem);
            //String curItem = (connectedPlayersListBox.SelectedValue as ListBoxItem).Content.ToString();
            viewProfileWindow.Show();

        }

        private void PlayerMenu_RequestGame_Click(object sender, RoutedEventArgs e)
        {
            if (connectedPlayersListBox.SelectedIndex == -1)
            {
                return;
            }
            string tempListBoxItem = (string)connectedPlayersListBox.SelectedItem;
            //MenuItem menuitem = e.Source as MenuItem;
            //if (menuitem != null)
            if (tempListBoxItem != GlobalProperties.localAlias)
            {
                GlobalProperties._client.PlayerToServerRequestGame(tempListBoxItem, GlobalProperties.localAlias);
            }
        }
        private void PlayerMenu_RequestChat_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = e.Source as MenuItem;
            if (menuitem != null)
            {
                //MessageBox.Show(menuitem.Header.ToString(), "Name");
            }

        }
        private void PlayerMenu_RequestFriend_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = e.Source as MenuItem;
            if (menuitem != null)
            {
                //MessageBox.Show(menuitem.Header.ToString(), "Name");
            }

        }
        private void PlayerMenu_Ignore_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = e.Source as MenuItem;
            if (menuitem != null)
            {

            }
        }

        private void connectedPlayersListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            connectedPlayersListBox.UnselectAll();
        }

        public bool? ShowNotificationWindowYesNo(string header, string message)
        {
            Point locationFromScreen = this.Grid.PointToScreen(new Point(0, 0));
            PresentationSource source = PresentationSource.FromVisual(this);
            System.Windows.Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);
            NotificationWindowYesNo nw = new NotificationWindowYesNo(header, message);
            nw.Owner = Window.GetWindow(this);
            nw.Top = locationFromScreen.Y + this.Height - 180;
            nw.Left = locationFromScreen.X + this.Width - 202;
            nw.ShowDialog();
            return (nw.DialogResult);
        }    
    
    
    
    }
}
