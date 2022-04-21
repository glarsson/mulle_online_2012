using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mulle.Lib.Entities
{
    public class Player : IEntity
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public int Win { get; set; }
        public int Loss { get; set; }
        public int Reputation { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Alias { get; set; }
        //Navigation properties
        //public virtual ICollection<TCPMessage> Messages { get; set; }
    }
}
