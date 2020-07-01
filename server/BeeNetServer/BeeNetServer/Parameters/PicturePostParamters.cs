using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters
{
    public class PicturePostParamters
    {
        [Required]
        public uint Id { get; set; }
        [Required]
        public string Path { get; set; }
    }
}
