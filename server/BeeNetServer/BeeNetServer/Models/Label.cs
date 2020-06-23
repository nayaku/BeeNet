using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Models
{

    public class Label
    {
        private int _num;
        [Key]
        public string Name { get; set; }
        public uint Color { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime EditTime { get; set; }

        public int Num => _num;
        public List<PictureLabel> PictureLabels { get; set; }
    }
}
