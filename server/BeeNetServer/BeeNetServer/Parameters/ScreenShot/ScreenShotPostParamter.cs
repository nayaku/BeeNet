using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters.ScreenShot
{
    public class ScreenShotPostParamter
    {
        public IFormFile File { get; set; }
        public string WorkspaceName { get; set; }
    }
}
