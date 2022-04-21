using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Mulle.Lib.Entities;
using Mulle.Lib.Repositories;
using System.Timers;

namespace Mulle.AzureServiceHost
{
    public partial class NetworkService : INetworkService
    {
        public void UpdateConnectedPlayersOnClient()
        {
            List<string> tempconnectedPlayers = new List<string>();
            
            foreach (KeyValuePair<PlayerContract, INetworkServiceCallback> p in _connectionCatalog)
            {
                tempconnectedPlayers.Add(p.Key.Alias);
            }
            foreach (INetworkServiceCallback cb in _connectionCatalog.Values)
            {
                if (IsChannelAlive(cb) == true)
                {
                    cb.PushConnectedPlayers(tempconnectedPlayers);
                }
            }
        }

        public void ServerToPlayerChat(string alias, string message)
        {
            foreach (INetworkServiceCallback cb in _connectionCatalog.Values)
            {
                if (IsChannelAlive(cb) == true)
                {
                    cb.ServerToPlayerChat(alias, message);
                }
            }
        }

        public void ServerToPlayerGameChat(string alias, string message)
        {
            var requestorContract = new PlayerContract();
            var requestedContract = new PlayerContract();

            foreach (PlayerContract p in _connectionCatalog.Keys)
            {
                if (p.Alias == alias)
                {
                    requestorContract = p;
                }
            }
            int tempInt = 0;
            foreach (KeyValuePair<PlayerContract, int> item in _gameCatalog)
            {
                if (requestorContract == item.Key)
                {
                    tempInt = item.Value;
                }
            }
            foreach (KeyValuePair<PlayerContract, int> item in _gameCatalog)
            {
                if (tempInt == item.Value)
                {
                    requestedContract = item.Key;
                }
            }

            foreach (KeyValuePair<PlayerContract, INetworkServiceCallback> item in _connectionCatalog)
            {
                if ((item.Key.Alias == requestorContract.Alias) || (item.Key.Alias == requestedContract.Alias))
                {
                    if (IsChannelAlive(item.Value) == true)
                    {
                        item.Value.ServerToPlayerGameChat(alias, message);
                    }
                }
            }
        }

        public void ServerToPlayerStartGame(string requestorAlias, string requestedAlias)
        {
            var requestorContract = new PlayerContract();
            var requestedContract = new PlayerContract();
            int gameId = 0;

            foreach (PlayerContract p in _connectionCatalog.Keys)
            {
                if (p.Alias == requestorAlias)
                {
                    requestorContract = p;
                }
            }
            foreach (KeyValuePair<PlayerContract, int> item in _gameCatalog)
            {
                if (requestorContract == item.Key)
                {
                    gameId = item.Value;
                }
            }
            foreach (KeyValuePair<PlayerContract, int> item in _gameCatalog)
            {
                if (gameId == item.Value)
                {
                    requestedContract = item.Key;
                }
            }

            foreach (KeyValuePair<PlayerContract, INetworkServiceCallback> item in _connectionCatalog)
            {
                if ((item.Key.Alias == requestorContract.Alias) || (item.Key.Alias == requestedContract.Alias))
                {
                    if (IsChannelAlive(item.Value) == true)
                    {
                        item.Value.ServerToPlayerStartGame(requestorContract.Alias, requestedContract.Alias, gameId);
                    }
                }
            }
        }

        public void ServerToPlayerAnnouncement(string message)
        {
            foreach (INetworkServiceCallback cb in _connectionCatalog.Values)
            {
                if (IsChannelAlive(cb) == true)
                {
                    cb.ServerToPlayerAnnouncement(message);
                }
            }
        }

        public void ServerToPlayerRequestGame(string requestedAlias, string requestorAlias)
        {
            foreach (KeyValuePair<PlayerContract, INetworkServiceCallback> item in _connectionCatalog)
            {
                if (item.Key.Alias == requestedAlias)
                {
                    if (IsChannelAlive(item.Value) == true)
                    {
                        item.Value.ServerToPlayerRequestGame(requestorAlias);
                    }
                }
            }
        }
    }
}


