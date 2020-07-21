using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BeeNetServer.Background.PictureStore;
using BeeNetServer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BeeNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PictureStoreController : ControllerBase
    {
        private readonly BeeNetContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PictureStoreController(BeeNetContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        // GET: api/<PictureStoreController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            if (PictureStoreProgress.IsBusy)
                return BadRequest();
            PictureStoreProgress.CreateExportTask(_mapper, _configuration);
            return Accepted();
        }

        // POST api/<PictureStoreController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] IFormFile Store)
        {
            if (PictureStoreProgress.IsBusy)
                return BadRequest();
            PictureStoreProgress.CreateImportTask(_mapper, _configuration, Store);
            return Accepted();
        }
    }
}
