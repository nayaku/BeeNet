using AutoMapper;
using BeeNetServer.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background
{
    public static class PictureStoreProgress
    {
        private static IMapper _mapper;
        private static IConfiguration _configuration;
        public static void CreateOuputJob(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }
        private static void OutputRun()
        {
            using var scope = Program.IHost.Services.CreateScope();
            var services = scope.ServiceProvider;
            using var context = scope.ServiceProvider.GetRequiredService<BeeNetContext>();

            // 创建压缩包
            var zipStream = File.OpenWrite(_configuration["ServerSettings:PictureStoreOutputPath"]);
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create);
            var context.Pictures.ProjectTo<PictureStorePicture>().
        }
    }
}
