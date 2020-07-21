using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeNetServer.Data;
using BeeNetServer.Models;
using AutoMapper;
using BeeNetServer.Parameters.Label;
using BeeNetServer.Response.Label;
using AutoMapper.QueryableExtensions;

namespace BeeNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        private readonly BeeNetContext _context;
        private readonly IMapper _mapper;

        public LabelsController(BeeNetContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Labels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LabelGetResponse>>> GetLabels([FromQuery] LabelGetsParamter paramters)
        {

            var labelsQuery = _context.Labels.AsQueryable();
            if (!string.IsNullOrWhiteSpace(paramters.Name))
            {
                labelsQuery.Where(l => l.Name.Contains(paramters.Name));
            }
            labelsQuery = labelsQuery.OrderByDescending(l => l.Num)
                .Skip((paramters.PageNumber - 1) * paramters.PageSize)
                .Take(paramters.PageSize);
            return await labelsQuery.ProjectTo<LabelGetResponse>(_mapper.ConfigurationProvider).ToListAsync();

        }

        // GET: api/Labels/5
        [HttpGet("{name}")]
        public async Task<ActionResult<LabelGetResponse>> GetLabel(string name)
        {
            var label = await _context.Labels.FindAsync(name);

            if (label == null)
            {
                return NotFound();
            }
            var labelResponse = _mapper.Map<LabelGetResponse>(label);
            return labelResponse;
        }

        [HttpPost]
        public async Task<ActionResult<Label>> PostLabel([FromBody] LabelPostPutParamter paramter)
        {
            var label = _mapper.Map<Label>(paramter);
            _context.Labels.Add(label);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LabelExists(label.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLabel", new { id = label.Name }, label);
        }

        [HttpPut]
        public async Task<ActionResult<Label>> PutLabel([FromBody] LabelPostPutParamter paramter)
        {
            var label = _mapper.Map<Label>(paramter);
            _context.Entry(label).State = EntityState.Modified;
            _context.Entry(label).Property(l => l.Num).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LabelExists(label.Name))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetLabel", new { id = label.Name }, label);
        }

        // DELETE: api/Labels/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLabel(string name)
        {
            var label = await _context.Labels.FindAsync(name);
            if (label == null)
            {
                return NotFound();
            }

            _context.Labels.Remove(label);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool LabelExists(string name)
        {
            return _context.Labels.Any(e => e.Name == name);
        }
    }
}
