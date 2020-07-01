using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeeNetServer.Data;
using BeeNetServer.Models;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using BeeNetServer.Parameters;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;

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

        // GET: api/ScreenShots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PictureBase>> GetScreenShot(uint id)
        {
            var screenShot = await  _context.ScreenShots
                .ProjectTo<PictureBase>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p=> p.Id == id);

            if (screenShot == null)
            {
                return NotFound();
            }

            return screenShot;
        }

        // POST: api/ScreenShots
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ScreenShot>> PostScreenShot(ScreenShotPostParamters paramters)
        {
            var screenShot = new ScreenShot
            {
                WorkspaceName = paramters.WorkspaceName
            };

            // 文件路径
            var dirPathString = UserSettingReader.UserSettings.PictureSettings.ScreenShotPath;
            while (true)
            {
                var filePathString = Path.Combine(dirPathString, Path.GetTempPath() + paramters.Ext);
                if (!System.IO.File.Exists(filePathString))
                {
                    screenShot.Path = filePathString;
                    break;
                }
            }
            
            // 读取文件尺寸
            using (var memoryStream = new MemoryStream(paramters.Data))
            {
                using var bitmap = new Bitmap(memoryStream);
                screenShot.Height = bitmap.Height;
                screenShot.Width = bitmap.Width;
            }

            // 写入文件
            using (var outputStream = System.IO.File.Create(screenShot.Path))
            {
                await outputStream.WriteAsync(paramters.Data,0,paramters.Data.Length);
            }
                
            _context.ScreenShots.Add(screenShot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScreenShot", new { id = screenShot.Id }, screenShot);
        }

        // DELETE: api/ScreenShots/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ScreenShot>> DeleteScreenShot(uint id)
        {
            var screenShot = await _context.ScreenShots.FindAsync(id);
            if (screenShot == null)
            {
                return NotFound();
            }

            _context.ScreenShots.Remove(screenShot);
            await _context.SaveChangesAsync();
            System.IO.File.Delete(screenShot.Path);
            return screenShot;
        }
    }
}
