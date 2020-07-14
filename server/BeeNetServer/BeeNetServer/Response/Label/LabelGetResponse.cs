using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Response.Label
{
    public class LabelGetResponse
    {
        public string Name { get; set; }
        public uint Color { get; set; }
        public int Num { get; set; }
    }
}
