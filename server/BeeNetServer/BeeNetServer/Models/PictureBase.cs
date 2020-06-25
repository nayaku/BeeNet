using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Models
{
    public class PictureBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedTime { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModifiedTime { get; set; }
        public string Path { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public List<PictureLabel> PictureLabels { get; set; }
        public string MD5 { get; set; }
        public byte[] PriHash { get; set; }
    }
}
