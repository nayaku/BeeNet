using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SimilarController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SimilarController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SimilarController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SimilarController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
