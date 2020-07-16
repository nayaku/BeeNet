using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background
{
    public class AddPicturesProgressResult
    {
        public uint Id { get; set; }
        public string Path { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public AddPicturesProgressResultStatusEnum Result { get; set; }
        public uint ConflictPictureId { get; set; }
    }
    public enum AddPicturesProgressResultStatusEnum
    {
        Pending,
        Processing,
        Done,
        Conflict
    }
}
