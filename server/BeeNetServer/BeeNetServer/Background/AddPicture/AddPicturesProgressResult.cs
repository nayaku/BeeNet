using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background.AddPicture
{
    public class AddPicturesProgressResult
    {
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
