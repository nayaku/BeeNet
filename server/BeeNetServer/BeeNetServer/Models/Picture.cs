using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Models
{
    public class Picture:PictureBase
    {
        public string MD5 { get; set; }
        public byte[] PriHash { get; set; }

        public List<PictureLabel> PictureLabels { get; set; }
    }
}
