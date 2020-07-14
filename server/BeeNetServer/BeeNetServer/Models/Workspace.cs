using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Models
{
    public class Workspace
    {
        [Key]
        public string Name { get; set; }
        public ushort Index { get; set; }

        public List<WorkspacePicture> WorkspacePictures { get; set; }
        public List<ScreenShot> ScreenShots { get; set; }
    }
}
