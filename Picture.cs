using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Models
{
    public class Picture
    {
        public Guid Id { get; set; }
        public DateTime AddTime { get; set; }
        public DateTime EditTime { get; set; }
        public string Path { get; set; }
        public string MD5 { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public PictureType Type { get; set; }
    }
    public enum PictureType
    {
        Normal,
        Screenshot
    }
}
