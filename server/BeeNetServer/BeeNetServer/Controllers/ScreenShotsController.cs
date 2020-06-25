using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeNetServer.Data;
using BeeNetServer.Models;
using BeeNetServer.Dto;
using BeeNetServer.Parameters;
using System.Linq.Dynamic.Core;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using System.IO;

namespace BeeNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenShotsController : ControllerBase
    {
        private readonly BeeNetContext _context;
        private readonly IMapper _mapper;

        public ScreenShotsController(BeeNetContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ScreenShot
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScreenShotResponseDto>>> GetPictures([FromQuery]ScreenShotsResourceParamters paramters)
        {
            return await _context.Pictures.Where(p => p.Type == PictureType.Screenshot)
                .Skip((paramters.PageNumber - 1) * paramters.PageSize)
                .Take(paramters.PageSize)
                .OrderBy(paramters.OrderBy)
                .ProjectTo<ScreenShotResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // GET: api/ScreenShot/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScreenShotResponseDto>> GetPicture(uint id)
        {
            var picture = await _context.PictureLabels
                .Where(p=>p.PictureId == id)
                .ProjectTo<ScreenShotResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            if (picture == null)
            {
                return NotFound();
            }

            return picture;
        }

        private async Task SaveScreenShot()
        {

        }

        // POST: api/ScreenShot
        [HttpPost]
        public async Task<ActionResult<Picture>> PostPicture(ScreenShotPostParamters postParamters)
        {
            var picture = _mapper.Map<Picture>
            HashUtil.ComplementPicture(picture);
            var picture = pictureExtension.Picture;
            var newFileName = picture.MD5 + Path.GetExtension(picture.Path);
            var newFilePath = Path.Combine(UserSettingReader.UserSettings.PictureSettings.PictureStorePath, newFileName);
            File.Copy(picture.Path, newFilePath, true);
            picture.Path = newFilePath;


            _context.Pictures.Add(picture);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPicture", new { id = picture.Id }, picture);
        }

        // DELETE: api/ScreenShot/5
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
