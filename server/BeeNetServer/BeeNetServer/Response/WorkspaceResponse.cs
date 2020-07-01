using BeeNetServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Response
{
    public class WorkspaceResponse
    {
        public List<PictureBase> Pictures { get; set; }
        public List<PictureBase> ScreenShots { get; set; }
    }
}
