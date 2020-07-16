using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FreeImageAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BeeNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThumbController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public ThumbController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // GET api/<ThumbController>/5
        [HttpGet("{path}/{width}")]
        public async Task<IActionResult> Get(HttpRequest request, string path, int width)
        {
            FREE_IMAGE_FORMAT format;
            if (request.Headers["Accept"].Contains("image/webp"))
                format = FREE_IMAGE_FORMAT.FIF_WEBP;
            else
                format = FREE_IMAGE_FORMAT.FIF_JPEG;
            var formatString = format == FREE_IMAGE_FORMAT.FIF_JPEG ? "image/jpeg" : "image/webp";
            FreeImageBitmap freeImageBitmap, resultBitmap;
            // 尝试从缓冲里面读取
            if (_memoryCache.TryGetValue(path + "/" + width, out ThumbCache thumbCache) && thumbCache.Width >= width)
            {
                // 一致直接返回
                if (thumbCache.Width == width && format == thumbCache.Format)
                    return new FileContentResult(thumbCache.Bytes, formatString);
                // 如果缓存的大小大于需求大小
                else
                {
                    using var originStream = new MemoryStream(thumbCache.Bytes);
                    freeImageBitmap = new FreeImageBitmap(originStream);
                }
            }
            else
            {
                var newPath = "wwwroot/" + path;
                // 找不到图片
                if (!System.IO.File.Exists(newPath))
                    return NotFound();
                // 从文件中读取
                freeImageBitmap = FreeImageBitmap.FromFile(newPath);
            }

            using var memoryStream = new MemoryStream();
            // 缩放图片
            if (freeImageBitmap.Width!=width) {
                
                int height = (int)(1.0 * freeImageBitmap.Height * width / freeImageBitmap.Width);
                resultBitmap = freeImageBitmap.GetScaledInstance(width, height, FREE_IMAGE_FILTER.FILTER_BICUBIC);
                freeImageBitmap.Dispose();
            }
            else
            {
                resultBitmap = freeImageBitmap;
            }
            resultBitmap.Save(memoryStream, format);
            resultBitmap.Dispose();
            // 保存到缓冲
            thumbCache = new ThumbCache
            {
                Bytes = memoryStream.ToArray(),
                Format = format,
                Width = width
            };

            _memoryCache.Set(path + "/" + width, thumbCache);

            return new FileContentResult(thumbCache.Bytes, formatString);


        }
    }

    public class ThumbCache
    {
        public byte[] Bytes;
        public FREE_IMAGE_FORMAT Format;
        public int Width;
    }

}
