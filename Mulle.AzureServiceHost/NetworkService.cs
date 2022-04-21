using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using Mulle.Lib.Entities;
using Mulle.Lib.Repositories;
using Mulle.Lib.Cards;
using System.Timers;

namespace Mulle.AzureServiceHost
{
    public enum RegisterStatus
    {
        Success,
        EmailInUse,
        AliasInUse
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class NetworkService : INetworkService
    {
        Dictionary<PlayerContract, INetworkServiceCallback> _connectionCatalog;
        Dictionary<PlayerContract, int> _gameCatalog;
        Dictionary<string, Deck> _deckCatalog;

        int gameIncrementor;

        public NetworkService()
        {
            _connectionCatalog = new Dictionary<PlayerContract, INetworkServiceCallback>();
            _gameCatalog = new Dictionary<PlayerContract, int>();
            _deckCatalog = new Dictionary<string, Deck>();

            gameIncrementor = 0;
        }







        public List<Card> RequestMainTableCards(string alias)
        {
            List<Card> tempCardList = new List<Card>();
            foreach (KeyValuePair<string, Deck> item in _deckCatalog)
            {
                if (item.Key == alias)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        tempCardList.Add(item.Value.Deal());
                    }
                }
            }
            return tempCardList;
        }

        public List<Card> RequestPlayerHand(string alias)
        {
            List<Card> tempCardList = new List<Card>();
            foreach (KeyValuePair<string, Deck> item in _deckCatalog)
            {
                if (item.Key == alias)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        tempCardList.Add(item.Value.Deal());
                    }
                }
            }
            return tempCardList;
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public void PlayerToServerRequestGame(string requestedAlias, string requestorAlias)
        {
            ServerToPlayerRequestGame(requestedAlias, requestorAlias);
        }

        public void PlayerToServerConfirmGame(string requestorAlias, string requestedAlias)
        {
            var requestorContract = new PlayerContract();
            var requestedContract = new PlayerContract();

            foreach (PlayerContract p in _connectionCatalog.Keys)
            {
                if (p.Alias == requestorAlias)
                {
                    requestorContract = p;
                }
                if (p.Alias == requestedAlias)
                {
                    requestedContract = p;
                }
            }
            _gameCatalog.Add(requestorContract, gameIncrementor);
            _gameCatalog.Add(requestedContract, gameIncrementor);
            ServerToPlayerAnnouncement(requestorAlias + " and " + requestedAlias + " have agreed to play a game of mulle");
            ServerToPlayerStartGame(requestorAlias, requestedAlias);
            Deck deck = new Deck();
            _deckCatalog.Add(requestorAlias, deck);
            _deckCatalog.Add(requestedAlias, deck);

            gameIncrementor++;
        }



        public byte[] DownloadProfilePicture(string alias)
        {
            byte[] profilePicByteStream;
            using (var playerRepository = new PlayerRepository())
            {
                try
                {
                    Player p = playerRepository.Items.First(i => i.Alias == alias);
                    profilePicByteStream = p.ProfilePicture;
                }
                finally
                {

                }
                return profilePicByteStream;
            }
        }

        public bool UploadProfilePicture(string alias, byte[] profilePicByteStream)
        {
            using (var playerRepository = new PlayerRepository())
            {
                Player p = playerRepository.Items.First(i => i.Alias == alias);
                p.ProfilePicture = profilePicByteStream;
                playerRepository.Save();
                return true;
            }
        }


        public PlayerContract GetPlayerDetails(string alias)
        {
            PlayerContract tempPlayerContract = new PlayerContract();

            foreach (PlayerContract p in _connectionCatalog.Keys)
            {
                if (alias == p.Alias)
                {
                    tempPlayerContract = p;
                }
            }
            return tempPlayerContract;
        }


        public List<PlayerContract> GetAllConnectedPlayers()
        {
            List<PlayerContract> tempPlayerList = new List<PlayerContract>();
            foreach (PlayerContract p in _connectionCatalog.Keys)
            {
                tempPlayerList.Add(p);
            }
            return tempPlayerList;
        }

        public void PlayerToServerChat(string alias, string message)
        {
            ServerToPlayerChat(alias, message);
        }

        public void PlayerToServerGameChat(string alias, string message)
        {
            ServerToPlayerGameChat(alias, message);

        }


        public bool Login(string alias, string password)
        {
            foreach (PlayerContract c in _connectionCatalog.Keys)
            {
                if (c.Alias == alias)
                {
                    return false;
                }
            }
            using (var playerRepository = new PlayerRepository())
            {
                PlayerContract tempPlayerContract = new PlayerContract();
                var player = playerRepository.Items.FirstOrDefault(p => p.Alias == alias && p.Password == password);
                if (player != null)
                {
                    //_connectionCatalog.Add(player.Id, OperationContext.Current.GetCallbackChannel<INetworkServiceCallback>());
                    tempPlayerContract.Alias = player.Alias;
                    tempPlayerContract.Id = player.Id;
                    tempPlayerContract.Email = player.Email;
                    tempPlayerContract.Loss = player.Loss;
                    tempPlayerContract.Win = player.Win;
                    tempPlayerContract.Reputation = player.Reputation;
                    tempPlayerContract.Rank = player.Rank;

                    _connectionCatalog.Add(tempPlayerContract, OperationContext.Current.GetCallbackChannel<INetworkServiceCallback>());
                    OperationContext.Current.Channel.Faulted += new EventHandler(Channel_Faulted);
                    OperationContext.Current.Channel.Closed += new EventHandler(Channel_Faulted);

                    UpdateConnectedPlayersOnClient();
                    ServerToPlayerAnnouncement(player.Alias + " has logged in");
                    return true;
                }
                else
                {
                    return false;
                }
            }            
        }

        public void Logout(string alias, int reason)
        {
            PlayerContract tempPlayerContract = new PlayerContract();
            foreach (KeyValuePair<PlayerContract, INetworkServiceCallback> c in _connectionCatalog)
            {
                if (alias == c.Key.Alias)
                {
                     tempPlayerContract = c.Key;
                     //((ICommunicationObject)c.Value).Close();
                }
            }
            switch (reason)
            {
                case 0:
                    ServerToPlayerAnnouncement(tempPlayerContract.Alias + " has quit (application exit)");
                    break;
                case 11:
                    ServerToPlayerAnnouncement(tempPlayerContract.Alias + " has lost connection (dead channel)");
                    break;
                case 12:
                    ServerToPlayerAnnouncement(tempPlayerContract.Alias + " has lost connection (channel faulted)");
                    break;
                case 13:
                    ServerToPlayerAnnouncement(tempPlayerContract.Alias + " has lost connection (dead channel)");
                    break;
            }
            
            _connectionCatalog.Remove(tempPlayerContract);
            _gameCatalog.Remove(tempPlayerContract);
            UpdateConnectedPlayersOnClient();
        }        

        public RegisterStatus Register(string email, string alias, string password)
        {
            RegisterStatus status;
            using (var playerRepository = new PlayerRepository())
            {
                var playerAlias = playerRepository.Items.FirstOrDefault(p => p.Alias == alias);
                var playerEmail = playerRepository.Items.FirstOrDefault(p => p.Email == email);

                if (playerAlias != null)
                {
                    status = RegisterStatus.AliasInUse;
                }
                else if (playerEmail != null)
                {
                    status = RegisterStatus.EmailInUse;
                }
                else
                {
                    var newPlayer = new Player { Email = email, Alias = alias, Password = password, Rank = 0, Loss = 0, Win = 0, Reputation = 0 };
                    playerRepository.Add(newPlayer);
                    playerRepository.Save();
                    status = RegisterStatus.Success;
                }
            }
            return status;
        }

        public bool IsChannelAlive(INetworkServiceCallback cb)
        {
            PlayerContract tempPlayerContract = new PlayerContract();
            bool state = false;

            if (((ICommunicationObject)cb).State == CommunicationState.Opened)
            {
                state = true;
            }
            else if (((ICommunicationObject)cb).State != CommunicationState.Opening && ((ICommunicationObject)cb).State != CommunicationState.Created)
            {
                state = false;
                foreach (KeyValuePair<PlayerContract, INetworkServiceCallback> item in _connectionCatalog)
                {
                    if (cb == ((INetworkServiceCallback)item.Value))
                    {
                        tempPlayerContract = item.Key;
                    }
                }
                //Logout(tempPlayerContract.Alias, 11);
            }

            return state;
        }

        void Channel_Faulted(object sender, EventArgs e)
        {
            var tempPlayerContract = new PlayerContract();
            foreach (KeyValuePair<PlayerContract, INetworkServiceCallback> item in _connectionCatalog)
            {
                if (e == ((INetworkServiceCallback)item.Value) )
                {
                    tempPlayerContract = item.Key;
                }
            }
            Logout(tempPlayerContract.Alias, 12);
        }

    
    
    // GAME MECHANICS

        //public Card RequestInitialCards(string alias)
        //{
            //CreateNewDeck(alias);
        //}
    
        //Card CreateNewDeck()


    }
}
