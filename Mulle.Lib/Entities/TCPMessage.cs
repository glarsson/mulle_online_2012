using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mulle.Lib.Entities
{
    public class TCPMessage : IEntity
    {
        public int Id { get; set; }
        public int TransmissionControl { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public int PlayerId { get; set; }
    }
}
