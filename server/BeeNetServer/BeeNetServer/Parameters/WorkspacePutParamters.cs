using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters
{
    public class WorkspacePutParamters
    {
        public string Name { get; set; }
        public ushort Index { get; set; }
        public string Context { get; set; }

        public List<uint> PictureId { get; set; }
    }
}
