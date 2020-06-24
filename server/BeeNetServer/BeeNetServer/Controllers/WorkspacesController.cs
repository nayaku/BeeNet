using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeNetServer.Data;
using BeeNetServer.Models;

namespace BeeNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspacesController : ControllerBase
    {
        private readonly BeeNetContext _context;

        public WorkspacesController(BeeNetContext context)
        {
            _context = context;
        }

        // GET: api/Workspaces
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Workspace>>> GetWorkspaces()
        {
            return await _context.Workspaces.ToListAsync();
        }

        // GET: api/Workspaces/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Workspace>> GetWorkspace(string id)
        {
            var workspace = await _context.Workspaces.FindAsync(id);

            if (workspace == null)
            {
                return NotFound();
            }

            return workspace;
        }

        // PUT: api/Workspaces/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkspace(string id, Workspace workspace)
        {
            if (id != workspace.Name)
            {
                return BadRequest();
            }

            _context.Entry(workspace).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkspaceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Workspaces
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Workspace>> PostWorkspace(Workspace workspace)
        {
            _context.Workspaces.Add(workspace);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (WorkspaceExists(workspace.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetWorkspace", new { id = workspace.Name }, workspace);
        }

        // DELETE: api/Workspaces/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Workspace>> DeleteWorkspace(string id)
        {
            var workspace = await _context.Workspaces.FindAsync(id);
            if (workspace == null)
            {
                return NotFound();
            }

            _context.Workspaces.Remove(workspace);
            await _context.SaveChangesAsync();

            return workspace;
        }

        private bool WorkspaceExists(string id)
        {
            return _context.Workspaces.Any(e => e.Name == id);
        }
    }
}
