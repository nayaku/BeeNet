using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeNetServer.Data;
using BeeNetServer.Models;
using BeeNetServer.Background;

namespace BeeNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly BeeNetContext _context;
        private readonly string[] _supportTypes = { ".BMP", ".GIF", ".EXIF", ".JPEG", ".JPG", ".PNG", ".TIFF" };

        public PicturesController(BeeNetContext context)
        {
            _context = context;
        }

        // GET: api/Pictures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Picture>>> GetPictures()
        {
            return await _context.Pictures.ToListAsync();
        }
        
        [HttpGet("SupportType")]
        public string[] GetSupportType()
        {
            return _supportTypes;
        }

        [HttpGet("AddProgress")]
        public Tuple<TaskProgressIndicator, List<PictureExtension>> GetAddProgress()
        {
            return new Tuple<TaskProgressIndicator,List<PictureExtension>>(PicturesAddProgress.TaskProgress, PicturesAddProgress.PictureExtensions );
        }

        // GET: api/Pictures/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Picture>> GetPicture(uint id)
        {
            var picture = await _context.Pictures.FindAsync(id);

            if (picture == null)
            {
                return NotFound();
            }

            return picture;
        }

        // PUT: api/Pictures/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPicture(uint id, Picture picture)
        {
            if (id != picture.Id)
            {
                return BadRequest();
            }

            _context.Entry(picture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PictureExists(id))
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

        // POST: api/Pictures
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Picture >> PostPicture(List<Picture> pictures)
        {
            if(pictures == null ||pictures.Count==0)
            {
                return BadRequest();
            }

            PicturesAddProgress.Push(pictures);
            //_context.Pictures.AddRange(pictures);
            //await _context.SaveChangesAsync();
            return new AcceptedResult();
        }

        // DELETE: api/Pictures/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Picture>> DeletePicture(uint id)
        {
            var picture = await _context.Pictures.FindAsync(id);
            if (picture == null)
            {
                return NotFound();
            }

            _context.Pictures.Remove(picture);
            await _context.SaveChangesAsync();

            return picture;
        }

        private bool PictureExists(uint id)
        {
            return _context.Pictures.Any(e => e.Id == id);
        }
    }
    
}
