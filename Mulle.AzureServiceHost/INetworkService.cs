using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Mulle.Lib.Entities;
using Mulle.Lib.Cards;

namespace Mulle.AzureServiceHost
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(INetworkServiceCallback))]
    public interface INetworkService
    {
        [OperationContract]
        bool Login(string alias, string password);
        [OperationContract]
        void Logout(string alias, int reason);
        [OperationContract]
        RegisterStatus Register(string alias, string email, string password);
        [OperationContract]
        void PlayerToServerChat(string alias, string message);
        [OperationContract]
        void PlayerToServerGameChat(string alias, string message);
        [OperationContract]
        PlayerContract GetPlayerDetails(string alias);
        [OperationContract]
        List<PlayerContract> GetAllConnectedPlayers();
        [OperationContract]
        bool UploadProfilePicture(string alias, byte[] profilePicByteStream);
        [OperationContract]
        byte[] DownloadProfilePicture(string alias);
        [OperationContract]
        void PlayerToServerRequestGame(string requestedAlias, string requestorAlias);
        [OperationContract]
        void PlayerToServerConfirmGame(string requestorAlias, string requestedAlias);
        // Game related
        [OperationContract]
        List<Card> RequestMainTableCards(string alias);
        [OperationContract]
        List<Card> RequestPlayerHand(string alias);
    }
    public interface INetworkServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ServerToPlayerChat(string alias, string message);
        [OperationContract(IsOneWay = true)]
        void ServerToPlayerGameChat(string alias, string message);
        [OperationContract(IsOneWay = true)]
        void ServerToPlayerAnnouncement(string message);
        [OperationContract(IsOneWay = true)]
        void PushConnectedPlayers(List<string> players);
        [OperationContract(IsOneWay = true)]
        void ServerToPlayerRequestGame(string requestorAlias);
        [OperationContract(IsOneWay = true)]
        void ServerToPlayerStartGame(string firstPlayer, string secondPlayer, int gameId);
    }
}
