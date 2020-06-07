using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Models
{
    public class PictureLabel
    {
        public uint PictureId { get; set; }
        public Picture Picture { get; set; }

        public string LabelName { get; set; }
        public Label Label { get; set; }
    }
}
