using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeNetServer.Data;
using BeeNetServer.Models;
using BeeNetServer.Parameters;
using System.Linq.Dynamic.Core;
using BeeNetServer.Dto;
using AutoMapper;

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
        public async Task<ActionResult<IEnumerable<Label>>> GetLabels([FromQuery]LabelsResourceParamters paramters)
        {
            var labelsQuery = _context.Labels.AsQueryable();
            if(!string.IsNullOrWhiteSpace(paramters.SearchKey))
            {
                labelsQuery.Where(l => l.Name.Contains(paramters.SearchKey));
            }
            labelsQuery.OrderBy(paramters.OrderBy);
            labelsQuery.Skip((paramters.PageNumber - 1) * paramters.PageSize)
                .Take(paramters.PageSize);
            return await labelsQuery.ToListAsync();
        }

        // GET: api/Labels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Label>> GetLabel(string name)
        {
            var label = await _context.Labels.FindAsync(name);

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }

        [HttpPut]
        public async Task<ActionResult<Label>> PutLabel(LabelDto labelDto)
        {               
            var label = _mapper.Map<Label>(labelDto);
            label.ModifiedTime = DateTime.Now;
            _context.Entry(label).State = EntityState.Modified;
            _context.Entry(label).Property(l => l.CreatedTime).IsModified = false;

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

        [HttpPost]
        public async Task<ActionResult<Label>> PostLabel(LabelDto labelDto)
        {
            var label = _mapper.Map<Label>(labelDto);
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
