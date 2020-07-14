using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeNetServer.Data;
using BeeNetServer.Models;
using BeeNetServer.Background;
using AutoMapper;
using BeeNetServer.Parameters;
using AutoMapper.QueryableExtensions;
using System.Linq.Dynamic.Core;
using BeeNetServer.Response.Picture;
using BeeNetServer.Parameters.Picture;

namespace BeeNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly BeeNetContext _context;
        private readonly IMapper _mapper;

        public PicturesController(BeeNetContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Pictures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PictureGetResponse>>> GetPictures([FromQuery] PictureGetsParamter paramters)
        {
            var picturesQuery = _context.Pictures.AsQueryable();
            if (paramters.IsOrderByAddTimeDesc)
                picturesQuery.OrderByDescending(p => p.CreatedTime);
            else
                picturesQuery.OrderBy(p => p.CreatedTime);

            picturesQuery = picturesQuery.Skip((paramters.PageNumber - 1) * paramters.PageNumber)
                .Take(paramters.PageNumber);

            return await picturesQuery.ProjectTo<PictureGetResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // GET: api/Pictures/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PictureGetResponse>> GetPicture(uint id)
        {
            var picture = await _context.Pictures
                .ProjectTo<PictureGetResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == id);
            //var picture = await _context.Pictures.FindAsync(id);

            if (picture == null)
            {
                return NotFound();
            }
            // var pictureDto = _mapper.Map<PictureResponseDto>(picture);
            return picture;
        }

        // POST: api/Pictures
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Picture>> PostPicture(PicturePostParameters parameters)
        {
            if (picturePostParamters?.Count == 0)
            {
                return BadRequest();
            }
            var pictures = _mapper.Map<List<Picture>>(picturePostParamters);
            PicturesAddProgress.CreateTask(pictures);
            //_context.Pictures.AddRange(pictures);
            //await _context.SaveChangesAsync();
            return new AcceptedResult();
        }

        // DELETE: api/Pictures/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePicture(uint id)
        {
            var picture = await _context.Pictures.FindAsync(id);
            if (picture == null)
            {
                return NotFound();
            }

            _context.Pictures.Remove(picture);
            await _context.SaveChangesAsync();
            System.IO.File.Delete(picture.Path);
            return Ok();
        }
    }

}
