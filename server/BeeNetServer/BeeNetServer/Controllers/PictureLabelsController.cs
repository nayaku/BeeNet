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
    public class PictureLabelsController : ControllerBase
    {
        private readonly BeeNetContext _context;

        public PictureLabelsController(BeeNetContext context)
        {
            _context = context;
        }

        // GET: api/PictureLabels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PictureLabel>>> GetPictureLabels()
        {
            return await _context.PictureLabels.ToListAsync();
        }

        // GET: api/PictureLabels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PictureLabel>> GetPictureLabel(uint id = 0, string labelName = null)
        {
            var pictureLabel = await _context.PictureLabels.FindAsync(id, labelName);

            if (pictureLabel == null)
            {
                return NotFound();
            }

            return pictureLabel;
        }

        // POST: api/PictureLabels
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PictureLabel>> PostPictureLabel(PictureLabel pictureLabel)
        {
            _context.PictureLabels.Add(pictureLabel);
            await _context.SaveChangesAsync();


            return CreatedAtAction("GetPictureLabel", new { id = pictureLabel.PictureId, labelName = pictureLabel.LabelName }, pictureLabel);
        }

        // DELETE: api/PictureLabels/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PictureLabel>> DeletePictureLabel(uint id)
        {
            var pictureLabel = await _context.PictureLabels.FindAsync(id);
            if (pictureLabel == null)
            {
                return NotFound();
            }

            _context.PictureLabels.Remove(pictureLabel);
            await _context.SaveChangesAsync();

            return pictureLabel;
        }

        private bool PictureLabelExists(uint id)
        {
            return _context.PictureLabels.Any(e => e.PictureId == id);
        }
    }
}
