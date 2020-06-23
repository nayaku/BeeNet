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
            var pictures = _context.Pictures.AsQueryable();
            if (!string.IsNullOrWhiteSpace(paramters.SearchKey))
            {
                pictures = pictures.Where(p => p.PictureLabels.Any(pl => pl.LabelName.Contains(paramters.SearchKey)));
            }
            if(!string.IsNullOrWhiteSpace(paramters.OrderBy))
            {
                pictures = pictures.OrderBy(paramters.OrderBy);
            }
            pictures = pictures.Skip((paramters.PageNumber - 1) * paramters.PageNumber)
                .Take(paramters.PageNumber);

            return await pictures.ProjectTo<PictureResponseDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("Progress")]
        public Tuple<TaskProgressIndicator, List<PictureExtension>> GetAddProgress()
        {
            return new Tuple<TaskProgressIndicator, List<PictureExtension>>(PicturesAddProgress.TaskProgress, PicturesAddProgress.PictureExtensions);
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
        public async Task<ActionResult<Picture>> PostPicture(List<Picture> pictures)
        {
            if (pictures == null || pictures.Count == 0)
            {
                return BadRequest();
            }

            PicturesAddProgress.Push(pictures);
            //_context.Pictures.AddRange(pictures);
            //await _context.SaveChangesAsync();
            return new AcceptedResult();
        }

        [HttpPost("Force")]
        public async Task<ActionResult<Picture>> ForceAddPicture(Picture picture)
        {
            try
            {
                picture = await PicturesAddProgress.ForceAddPicture(picture);
                return picture;
            }
            catch (SimpleException e)
            {
                return BadRequest(e.Message);
            }
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
    }

}
