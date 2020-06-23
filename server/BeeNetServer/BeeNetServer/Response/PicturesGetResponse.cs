using BeeNetServer.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Response
{
    public class PicturesGetResponse
    {
        public List<PictureResponseDto> Pictures { get; set; }
        public string NextLink { get; set; }
    }
}
