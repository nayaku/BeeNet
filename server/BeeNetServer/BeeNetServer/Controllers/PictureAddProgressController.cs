using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeNetServer.Background;
using BeeNetServer.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeeNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PictureAddProgressController : ControllerBase
    {
        [HttpGet]
        public async Task<PictureAddProgressGetResponse> GetPictureAddProgress()
        {
            return await Task<PictureAddProgressGetResponse>.Run(() => new PictureAddProgressGetResponse());
        }

    }
}
