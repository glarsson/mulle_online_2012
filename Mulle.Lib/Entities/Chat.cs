using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mulle.Lib.Entities
{
    public class Chat : IEntity
    {
        public int Id { get; set; }
        public int DateTime { get; set; }
        public string Alias { get; set; }
        public string Message { get; set; }
    }
}
