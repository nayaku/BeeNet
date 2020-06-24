using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Models
{
    public class WorkspacePicture
    {
        public Workspace Workspace { get; set; }
        public string WorkspaceName { get; set; }
        public Picture Picture { get; set; }
        public uint PictureId { get; set; }
    }
}
