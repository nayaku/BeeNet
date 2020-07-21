using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeeNetServer.Data;
using BeeNetServer.Models;
using BeeNetServer.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BeeNetServer.Background.PictureStore
{
    public static class PictureStoreProgress
    {
        public static PictureStoreExportProgressIndicator ExportTaskProgress { get; set; }
        public static PictureStoreImportProgressIndicator ImportTaskProgress { get; set; }

        private static IMapper _mapper;
        private static IConfiguration _configuration;
        private static IFormFile _formFile;
        /// <summary>
        /// 创建导出工作任务
        /// </summary>
        private static Task _workTask;

        public static bool IsBusy => 
            (ExportTaskProgress != null && ExportTaskProgress.TaskProgressStatus == TaskProgressEnum.Running) ||
            (ImportTaskProgress != null && ImportTaskProgress.TaskProgressStatus == TaskProgressEnum.Running);
        public static void CreateExportTask(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
            ExportTaskProgress = new PictureStoreExportProgressIndicator();

            ExportTaskProgress.SetProgress(0, Resource.ReadyToRun);

            _workTask = new Task(Export, TaskCreationOptions.LongRunning);
            _workTask.Start();
        }

        /// <summary>
        /// 创建导入任务
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="configuration"></param>
        public static void CreateImportTask(IMapper mapper, IConfiguration configuration, IFormFile formFile)
        {
            _mapper = mapper;
            _configuration = configuration;
            _formFile = formFile;

            ImportTaskProgress = new PictureStoreImportProgressIndicator();

            ImportTaskProgress.SetProgress(0, Resource.ReadyToRun);

            _workTask = new Task(Import, TaskCreationOptions.LongRunning);
            _workTask.Start();
        }

        /// <summary>
        /// 导出任务
        /// </summary>
        private static void Export()
        {
            using var scope = Program.IHost.Services.CreateScope();
            var services = scope.ServiceProvider;
            using var context = scope.ServiceProvider.GetRequiredService<BeeNetContext>();


            // 创建压缩包
            ExportTaskProgress.SetProgress(0.05f, Resource.CreatingZipFile);
            var zipStream = File.OpenWrite(_configuration["ServerSettings:PictureStoreOutputPath"]);
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create);

            // 导出数据库
            ExportTaskProgress.SetProgress(0.1f, Resource.ExportingDatabase);
            var entry = archive.CreateEntry("output.json", CompressionLevel.Optimal);
            using var entryStream = entry.Open();
            using var jsonWriter = new Utf8JsonWriter(entryStream);

            var pictureStoreJson = new PictureStoreJson
            {
                Pictures = context.Pictures.ProjectTo<PictureStorePicture>(_mapper.ConfigurationProvider).ToList(),
                Labels = context.Labels.ProjectTo<PictureStoreLabel>(_mapper.ConfigurationProvider).ToList(),
                Version = "1.0.0"
            };
            ExportTaskProgress.SetProgress(0.15f, Resource.ExportingDatabase);
            JsonSerializer.Serialize(jsonWriter, pictureStoreJson, typeof(PictureStoreJson));

            // 导出图片文件
            for (var i = 0; i < pictureStoreJson.Pictures.Count; i++)
            {
                var picture = pictureStoreJson.Pictures[i];
                ExportTaskProgress.SetProgress(0.8f * i / pictureStoreJson.Pictures.Count + 0.2f,
                    string.Format(Resource.ExportingPicture, Path.GetFileNameWithoutExtension(picture.Path)));
                var path = "wwwroot/" + picture.Path;
                archive.CreateEntryFromFile(path, picture.Path);
            }

            // 完成前处理
            ExportTaskProgress.ResultUrl = Path.GetRelativePath("wwwroot", _configuration["ServerSettings:PictureStoreOutputPath"]);
            ExportTaskProgress.SetProgress(1.0f, Resource.OperateFinish);
            ExportTaskProgress.TaskProgressStatus = TaskProgressEnum.Finish;
        }

        /// <summary>
        /// 导入任务
        /// </summary>
        private static void Import()
        {
            using var scope = Program.IHost.Services.CreateScope();
            var services = scope.ServiceProvider;
            using var context = scope.ServiceProvider.GetRequiredService<BeeNetContext>();

            // 读取压缩包
            ImportTaskProgress.SetProgress(0.05f, Resource.ReadingZipFile);
            using var archive = new ZipArchive(_formFile.OpenReadStream(), ZipArchiveMode.Read);

            // 读取数据库
            ImportTaskProgress.SetProgress(0.1f, Resource.ReadingDatabaseFile);
            var entry = archive.GetEntry("output.json");
            var entryStream = entry.Open();
            Span<byte> data = new byte[entry.Length];
            entryStream.Read(data);
            entryStream.Dispose();
            var jsonReader = new Utf8JsonReader(data);

            PictureStoreJson pictureStoreJson = (PictureStoreJson)JsonSerializer.Deserialize(ref jsonReader, typeof(PictureStoreJson));

            ImportTaskProgress.LabelResults = Enumerable.Repeat(new ResultStatusEnum(), pictureStoreJson.Labels.Count).ToList();
            ImportTaskProgress.PictureResults = Enumerable.Repeat(new PictureStoreImportProgressPictureResult(), pictureStoreJson.Pictures.Count).ToList();

            // 写入标签数据
            for (var i = 0; i < pictureStoreJson.Labels.Count; i++)
            {
                var storeLabel = pictureStoreJson.Labels[i];
                ImportTaskProgress.SetProgress(0.38f * i / pictureStoreJson.Labels.Count + 0.2f, string.Format(Resource.ImportingLabel, storeLabel.Name));
                ImportTaskProgress.LabelResults[i] = ResultStatusEnum.Processing;
                var label = context.Labels.Find(storeLabel.Name);
                // 标签存在，则更新
                if (label != null)
                {
                    if (label.Color != storeLabel.Color)
                    {
                        ImportTaskProgress.LabelResults[i] = ResultStatusEnum.Updated;
                        label.Color = storeLabel.Color;
                    }
                    else
                    {
                        ImportTaskProgress.LabelResults[i] = ResultStatusEnum.DoNothing;
                    }
                }
                else
                {
                    label = new Label
                    {
                        Name = storeLabel.Name,
                        Color = storeLabel.Color
                    };
                    context.Labels.Add(label);
                    ImportTaskProgress.LabelResults[i] = ResultStatusEnum.Done;
                }
            }
            ImportTaskProgress.SetProgress(0.40f, Resource.SavingLabelDatabaseChange);
            context.SaveChanges();

            // 写入图片数据
            for (var i = 0; i < pictureStoreJson.Pictures.Count; i++)
            {
                var storePicture = pictureStoreJson.Pictures[i];
                ImportTaskProgress.SetProgress(0.45f * i / pictureStoreJson.Pictures.Count + 0.5f,
                    string.Format(Resource.ImportingPicture, i));
                var pictureResult = ImportTaskProgress.PictureResults[i];
                pictureResult.Result = ResultStatusEnum.Processing;
                var conflictPictureId = context.Pictures.Where(p => p.MD5 == storePicture.MD5).Select(p => p.Id).FirstOrDefault();

                if (conflictPictureId != 0)
                {
                    pictureResult.Result = ResultStatusEnum.Conflict;
                    pictureResult.ConflictPictureId = conflictPictureId;
                }
                else
                {
                    // 解压图片文件
                    entry = archive.GetEntry(storePicture.Path);
                    var imagePath = "wwwroot/" + storePicture.Path;
                    Directory.CreateDirectory(imagePath);
                    entry.ExtractToFile(imagePath);
                    var picture = _mapper.Map<Picture>(storePicture);
                    picture.PictureLabels = storePicture.Labels.Select(ln => new PictureLabel
                    {
                        Picture = picture,
                        LabelName = ln
                    }).ToList();

                    context.Pictures.Add(picture);
                    pictureResult.Result = ResultStatusEnum.Done;
                }
            }
            ImportTaskProgress.SetProgress(0.99f, Resource.SavingPictureDatabaseChange);
            context.SaveChanges();

            // 完成
            ImportTaskProgress.TaskProgressStatus = TaskProgressEnum.Finish;
            ImportTaskProgress.SetProgress(1.0f, Resource.OperateFinish);
        }

    }
}

