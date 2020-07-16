using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background
{
    public class PictureStorePicture
    {
        public uint Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Path { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public List<String> Labels { get; set; }
    }
}
