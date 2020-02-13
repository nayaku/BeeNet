using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Models
{
    public class Label
    {
        [Key]
        public string Name { get; set; }
        public uint Color { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EditTime { get; set; }

        public List<PictureLabel> PictureLabels { get; set; }
    }
}
