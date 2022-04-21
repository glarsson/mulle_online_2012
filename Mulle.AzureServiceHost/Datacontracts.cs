using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Mulle.AzureServiceHost
{
    [DataContract]
    public class PlayerContract
    {
        [DataMember]
        public string Alias { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Rank { get; set; }
        [DataMember]
        public int Win { get; set; }
        [DataMember]
        public int Loss { get; set; }
        [DataMember]
        public int Reputation { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public int GameId { get; set; }
    }

    // does not work, not recognized on client. wtf?
    [DataContract]
    public class ClientNotification
    {
        [DataMember]
        public string NotificationHeader { get; set; }
        [DataMember]
        public string NotificationMessage { get; set; }
        [DataMember]
        public int Mode { get; set; }
    }


}



