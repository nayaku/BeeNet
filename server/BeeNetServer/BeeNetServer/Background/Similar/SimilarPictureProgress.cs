using AutoMapper;
using BeeNetServer.Data;
using BeeNetServer.Resources;
using BeeNetServer.Tool;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background.Similar
{
    public class SimilarPictureProgress
    {
        public static SimilarPictureProgressIndicator TaskProgress { get; set; }

        private static IMapper _mapper;

        private static Task _workTask;

        public static void CreateTask(IMapper mapper)
        {
            _mapper = mapper;
            TaskProgress = new SimilarPictureProgressIndicator();

            TaskProgress.SetProgress(0, Resource.ReadyToRun);

            _workTask = new Task(GetSimilar, TaskCreationOptions.LongRunning);
            _workTask.Start();
        }

        private static void GetSimilar()
        {
            using var scope = Program.IHost.Services.CreateScope();
            var services = scope.ServiceProvider;
            using var context = scope.ServiceProvider.GetRequiredService<BeeNetContext>();

            // 补充所有图片的priHash数值
            var pictures = context.Pictures.Where(p => p.PriHash == null);
            var i = 0;
            var num = pictures.Count();
            foreach (var picture in pictures)
            {
                TaskProgress.SetProgress(0.3f * i / num, String.Format(Resource.ComputingPictureEigenvalue, i, num);
                var path = "wwwroot/" + picture.Path;
                using var pictureStream = File.OpenRead(path);
                picture.PriHash = HashUtil.GetPriHash(pictureStream);
            }

            // 保存到数据库
            TaskProgress.SetProgress(0.3f, Resource.SavingPictureEigenvalueToDatabase);
            context.SaveChanges();

            object TaskProgressLock = new object();
            object TaskProgressGroupLock = new object();
            var picturePriHashList = context.Pictures.Select(p => new Tuple<byte[], uint>(p.PriHash, p.Id)).ToList();
            i = 0;
            num = picturePriHashList.Count;
            var parallelResult = Parallel.ForEach(picturePriHashList, picturePriHash =>
             {
                 var similarList = new List<uint>();
                 lock (TaskProgressLock)
                 {
                     TaskProgress.SetProgress(0.7f * i / num + 0.3f, String.Format(Resource.ProcessingPicture, i, num));
                     i++;
                 }

                 foreach (var tPrihash in picturePriHashList)
                 {
                     if (HashUtil.IsSimilar(picturePriHash.Item1, tPrihash.Item1))
                     {
                         similarList.Append(tPrihash.Item2);
                     }
                 }

                 if (similarList.Count > 0)
                 {
                     lock (TaskProgressGroupLock)
                     {
                         TaskProgress.Groups.Append(similarList);
                     }
                 }

             });

            // 完成任务
            TaskProgress.SetProgress(1.0f, Resource.OperateFinish);
            TaskProgress.TaskProgressStatus = TaskProgressEnum.Finish;


        }
    }
}
