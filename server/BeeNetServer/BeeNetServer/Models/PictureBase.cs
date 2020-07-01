using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string Path { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
