using BeeNetServer.CException;
using BeeNetServer.Data;
using BeeNetServer.Models;
using BeeNetServer.Parameters.Picture;
using BeeNetServer.Resources;
using BeeNetServer.Tool;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background
{
    /// <summary>
    /// 添加图片线程类
    /// </summary>
    public static class PicturesAddProgress
    {
        /// <summary>
        /// 进度指示器
        /// </summary>
        public static AddPicturesProgressIndicator TaskProgress { get; set; }

        /// <summary>
        /// 待添加的图片列表
        /// </summary>
        private static PicturePostParameters _picturePostParameters;
        /// <summary>
        /// 工作任务
        /// </summary>
        private static Task _workTask;
        private static IConfiguration _configuration;

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="pictures"></param>
        public static void CreateTask(PicturePostParameters parameters, IConfiguration configuration)
        {
            _picturePostParameters = parameters;
            _configuration = configuration;
            var len = _picturePostParameters.ImageFiles != null ? _picturePostParameters.ImageFiles.Count : _picturePostParameters.Paths.Count;
            TaskProgress = new AddPicturesProgressIndicator(len);
            TaskProgress.SetProgress(0, Resource.ReadyToRun);

            _workTask = new Task(Run, TaskCreationOptions.LongRunning);
            _workTask.Start();
        }

        /// <summary>
        /// 尝试添加图片
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="context">数据库上下文</param>
        /// <param name="Extension">扩展名</param>
        /// <param name="md5Set">md5集合</param>
        /// <returns>(图片，冲突ID)</returns>
        public static (Picture, uint) TryAddImage(Stream stream, BeeNetContext context, string Extension, HashSet<string> md5Set)
        {
            var picture = new Picture();
            using var bufferedStream = new BufferedStream(stream);
            picture.MD5 = HashUtil.GetMD5(bufferedStream);
            bufferedStream.Seek(0, SeekOrigin.Begin);
            // 判断MD5是否冲突
            if (md5Set.Contains(picture.MD5))
                return (null, 0);
            md5Set.Add(picture.MD5);
            var id = context.Pictures.Where(p => p.MD5 == picture.MD5).Take(1).Select(p => p.Id).FirstOrDefault();
            if (id != 0)
                return (null, id);
            // 获取尺寸
            (picture.Width, picture.Height) = HashUtil.GetSize(stream);
            bufferedStream.Seek(0, SeekOrigin.Begin);
            // 写入文件
            var imageFolder = _configuration["ServerSettings:PictureStorePath"];
            var childFolder = $"{DateTime.Today:yyyy_MM}";
            var childPath = Path.Join(imageFolder, childFolder);
            Directory.CreateDirectory(childPath);
            var imagePath = Path.Join(childPath, picture.MD5 + Extension);
            using var outputStream = File.OpenWrite(imagePath);
            bufferedStream.CopyTo(outputStream);
            // 添加到数据库
            picture.Path = childFolder + "/" + picture.MD5 + Extension;
            context.Pictures.Add(picture);
            return (picture, 0);
        }

        /// <summary>
        /// 运行
        /// </summary>
        public static void Run()
        {
            using var scope = Program.IHost.Services.CreateScope();
            var services = scope.ServiceProvider;
            using var context = scope.ServiceProvider.GetRequiredService<BeeNetContext>();
            // 是否为本地添加
            var isFromLocal = _picturePostParameters.Paths != null;
            var len = _picturePostParameters.ImageFiles != null ? _picturePostParameters.ImageFiles.Count : _picturePostParameters.Paths.Count;
            var pictures = new List<Picture>(len);
            var md5Set = new HashSet<string>(len);
            // 遍历列表
            for (var index = 0; index < len; index++)
            {
                string fileName, extension, path;
                IFormFile formFile;
                Stream stream;
                TaskProgress.PictureResults[index].Result = AddPicturesProgressResultStatusEnum.Processing;
                // 获取图片文件名等外部信息
                if (isFromLocal)
                {
                    path = _picturePostParameters.Paths[index];
                    fileName = Path.GetFileNameWithoutExtension(path);
                    extension = Path.GetExtension(path);
                    stream = File.OpenRead(fileName);

                }
                else
                {
                    formFile = _picturePostParameters.ImageFiles[index];
                    fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                    extension = Path.GetExtension(formFile.FileName);
                    stream = formFile.OpenReadStream();
                }
                // 开始处理文件内容
                TaskProgress.SetProgress(1.0f * (index + 1) / len * 0.99f, string.Format(Resource.AddingPicture, fileName));
                (var picture, var conflictId) = TryAddImage(stream, context, extension,md5Set);
                // 处理结果
                var progressResult = TaskProgress.PictureResults[index];
                if (picture == null)
                {
                    progressResult.Result = AddPicturesProgressResultStatusEnum.Conflict;
                    progressResult.ConflictPictureId = conflictId;
                }
                else
                {
                    progressResult.Result = AddPicturesProgressResultStatusEnum.Done;
                    progressResult.Id = picture.Id;
                    progressResult.Path = picture.Path;
                    progressResult.Height = picture.Height;
                    progressResult.Width = progressResult.Width;
                }
            }
            TaskProgress.SetProgress(0.99f, Resource.SavingDatabaseChange);
            context.SaveChanges();
            for(var index=0;index<len;index++)
            {
                if(pictures[index]!=null)
                    TaskProgress.PictureResults[index].Id = pictures[index].Id;
            }
            TaskProgress.SetProgress(1.0f, Resource.OperateFinish);
            TaskProgress.TaskProgressStatus = AddPicturesProgressStatus.Finished;
        }

    }

}
