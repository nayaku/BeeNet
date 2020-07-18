using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background.PictureStore
{
    public class PictureStorePicture
    {
        public DateTime CreatedTime { get; set; }
        public string Path { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string MD5 { get; set; }
        public byte[] PriHash { get; set; }
        public List<String> Labels { get; set; }
    }
}
