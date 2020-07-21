using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Response.ScreenShot
{
    public class ScreenShotGetResponse
    {
        public uint Id { get; set; }
        public string Path { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
