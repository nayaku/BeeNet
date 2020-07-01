using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Models
{
    public class ScreenShot : PictureBase
    {
        public string WorkspaceName { get; set; }
        public Workspace Workspace { get; set; }
    }
}
