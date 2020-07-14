using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters.Picture
{
    public class PicturePostParameters
    {
        public List<string> Paths { get; set; }

        public List<IFormFile> ImageFiles { get; set; }
    }
}
