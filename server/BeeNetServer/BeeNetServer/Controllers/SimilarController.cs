using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BeeNetServer.Background.Similar;
using BeeNetServer.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BeeNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimilarController : ControllerBase
    {
        private readonly BeeNetContext _context;
        private readonly IMapper _mapper;

        public SimilarController(BeeNetContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<SimilarController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            if (SimilarPictureProgress.IsBusy)
                return BadRequest();
            SimilarPictureProgress.CreateTask(_mapper);
            return Accepted();
        }

        // GET api/<SimilarController>/5
        [HttpGet("Result")]
        public SimilarPictureProgressIndicator GetResult()
        {
            return SimilarPictureProgress.TaskProgress;
        }

    }
}
