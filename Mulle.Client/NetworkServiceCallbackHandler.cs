using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Mulle.Client.NetworkServiceReference;
using System.ServiceModel;
using System.ComponentModel;
using System.Windows.Threading;



namespace Mulle.Client
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class NetworkServiceCallbackHandler : INetworkServiceCallback, INotifyPropertyChanged
    {
        private string _chatOutput;
        public string ChatOutput
        {
            get { return _chatOutput; }
            set
            {
                _chatOutput = value;
                OnPropertyChanged("ChatOutput");
            }
        }

        private string _gameChatOutput;
        public string GameChatOutput
        {
            get { return _gameChatOutput; }
            set
            {
                _gameChatOutput = value;
                OnPropertyChanged("GameChatOutput");
            }
        }


        private List<string> _connectedPlayersList;
        public List<string> connectedPlayersList
        {
            get { return _connectedPlayersList; }
            set
            {
                _connectedPlayersList = value;
                OnPropertyChanged("connectedPlayersList");
            }
        }

        private string _incomingGameRequest;
        public string incomingGameRequest
        {
            get { return _incomingGameRequest; }
            set
            {
                _incomingGameRequest = value;
                OnPropertyChanged("IncomingGameRequest");
            }
        }


        public void PushConnectedPlayers(List<string> players)
        {
            List<string> tempConnectedPlayersList = new List<string>();

            foreach (string p in players)
            {
                tempConnectedPlayersList.Add(p);
            }
            connectedPlayersList = tempConnectedPlayersList;
        }

        public void ServerToPlayerChat(string alias, string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<");
            builder.Append(alias);
            builder.Append("> ");
            builder.Append(message);
            builder.Append("\n");
            ChatOutput = builder.ToString();
        }

        public void ServerToPlayerStartGame(string requestorAlias, string requestedAlias, int gameId)
        {
            string tempString = String.Empty;
            if (requestorAlias != GlobalProperties.localAlias)
            {
                
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    GameWindow gw = new GameWindow(requestorAlias);
                    gw.ShowDialog();
                }));   
            }
            if (requestedAlias != GlobalProperties.localAlias)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    GameWindow gw = new GameWindow(requestedAlias);
                    gw.ShowDialog();
                }));   
            }
            GlobalProperties.gameId = gameId;
        }


        public void ServerToPlayerGameChat(string alias, string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<");
            builder.Append(alias);
            builder.Append("> ");
            builder.Append(message);
            builder.Append("\n");
            GameChatOutput = builder.ToString();
        }

        public void ServerToPlayerRequestGame(string requestorAlias)
        {
            incomingGameRequest = requestorAlias;
        }


        public void ServerToPlayerAnnouncement(string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("*** ");
            builder.Append(message);
            builder.Append(" ***");
            builder.Append("\n");
            ChatOutput = builder.ToString();
        }

        public void ServerToPlayerGameAnnouncement(string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("*** ");
            builder.Append(message);
            builder.Append(" ***");
            builder.Append("\n");
            GameChatOutput = builder.ToString();
        }


        public bool ClientHeartBeat()
        {
            return true;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
               
                PropertyChanged( this, new PropertyChangedEventArgs(propertyName));
        
            }
        }
    }
}
