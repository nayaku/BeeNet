using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Models
{
    public class DatabaseInformation
    {
        public uint Id { get; set; }
        public string Version { get; set; }
        public string Extension { get; set; }
    }
}
