using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background.PictureStore
{
    public class PictureStoreJson
    {
        public List<PictureStorePicture> Pictures { get; set; }
        public List<PictureStoreLabel> Labels { get; set; }
        public string Version { get; set; }
    }
}
