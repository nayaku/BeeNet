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
using BeeNetServer.CException;
using AutoMapper;
using BeeNetServer.Parameters;
using BeeNetServer.Dto;
using BeeNetServer.Response;
using AutoMapper.QueryableExtensions;
using System.Linq.Dynamic.Core;

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
        public async Task<ActionResult<IEnumerable<PictureResponseDto>>> GetPictures([FromQuery]PictureResourceParamters paramters)
        {
            var picturesQuery = _context.Pictures.Where(p=>p.Type==PictureType.Normal);
            if (!string.IsNullOrWhiteSpace(paramters.SearchKey))
            {
                picturesQuery = picturesQuery.Where(p => p.PictureLabels.Any(pl => pl.LabelName.Contains(paramters.SearchKey)));
            }
            if(!string.IsNullOrWhiteSpace(paramters.OrderBy))
            {
                picturesQuery = picturesQuery.OrderBy(paramters.OrderBy);
            }
            picturesQuery = picturesQuery.Skip((paramters.PageNumber - 1) * paramters.PageNumber)
                .Take(paramters.PageNumber);

            return await picturesQuery.ProjectTo<PictureResponseDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        // GET: api/Pictures/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PictureResponseDto>> GetPicture(uint id)
        {
            var picture = await _context.Pictures
                .ProjectTo<PictureResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            //var picture = await _context.Pictures.FindAsync(id);

            if (picture == null)
            {
                return NotFound();
            }
            var pictureDto =  _mapper.Map<PictureResponseDto>(picture);
            return pictureDto;
        }

        //// PUT: api/Pictures/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutPicture(uint id, Picture picture)
        //{
        //    if (id != picture.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(picture).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!PictureExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Pictures
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Picture>> PostPicture(List<PicturePostParamters> picturePostParamters)
        {
            if (picturePostParamters ?.Count == 0)
            {
                return BadRequest();
            }
            var pictures = _mapper.Map<List<Picture>>(picturePostParamters);
            PicturesAddProgress.Push(pictures);
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

            return Ok();
        }
    }

}
