using BeeNetServer.Data;
using BeeNetServer.Models;
using BeeNetServer.Resources;
using BeeNetServer.Tool;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
        public static List<PictureExtension> PictureExtensions { get; set; }
        public static TaskProgressIndicator TaskProgress { get; set; } = new TaskProgressIndicator();
        public static Task WorkTask { get; set; }

        public static void Push(List<Picture> pictures)
        {
            TaskProgress.TaskProgressStatus = TaskProgressStatus.Running;
            TaskProgress.SetProgress(0,"准备执行。");
            PictureExtensions = new List<PictureExtension>(pictures.Count);
            foreach (var picture in pictures)
            {
                picture.Type = PictureType.Normal;
                PictureExtensions.Add(new PictureExtension { Picture = picture });
            }
            WorkTask = new Task(Run, TaskCreationOptions.LongRunning);
            WorkTask.Start();
        }

        /// <summary>
        /// 运行
        /// </summary>
        public static void Run()
        {
            // 从本地添加
            using var scope = Program.IHost.Services.CreateScope();
            var services = scope.ServiceProvider;
            using var context = scope.ServiceProvider.GetRequiredService<BeeNetContext>();

            Dictionary<string, Picture> md5Dict = new Dictionary<string, Picture>();
            for(int idx=0;idx<PictureExtensions.Count;idx++)
            {
                var pictureExtension = PictureExtensions[idx];
                var picture = pictureExtension.Picture;
                TaskProgress.SetProgress((float)idx / PictureExtensions.Count, string.Format(Resource.AddingPicture, Path.GetFileName(picture.Path)));
                HashUtil.ComplementPicture(picture);
                if (File.Exists(picture.Path))
                {
                    //var md5 = picture.MD5 = HashUtil.GetMD5ByHashAlgorithm(picture.Path);
                    var md5 = picture.MD5;
                    // 队列中是否已经包含相同图片
                    if (!md5Dict.ContainsKey(md5))
                    {
                        var conflictPicture = context.Pictures.FirstOrDefault(p => p.MD5 == md5);
                        if (conflictPicture == null)
                        {
                            // 启动相似性判断
                            if (UserSettingReader.UserSettings.PictureSettings.UseSimilarJudge)
                            {
                                //picture.PriHash = HashUtil.PriHash(picture.Path);
                                var conflictPictures = new List<Picture>();
                                var resPictures = context.Pictures;
                                foreach (var resPicture in resPictures)
                                {
                                    if (HashUtil.IsSimilar(picture.PriHash, resPicture.PriHash))
                                    {
                                        conflictPictures.Add(resPicture);
                                    }
                                }
                                foreach (var resPicture in PictureExtensions)
                                {
                                    if (resPicture.Picture != picture)
                                    {
                                        if (HashUtil.IsSimilar(picture.PriHash, resPicture.Picture.PriHash))
                                        {
                                            conflictPictures.Add(resPicture.Picture);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (conflictPictures.Count == 0)
                                {
                                    // 通过判断
                                    md5Dict.Add(md5, picture);
                                    AddPicture(pictureExtension);
                                    context.Pictures.Add(picture);

                                }
                                else
                                {
                                    pictureExtension.SetError(Resource.SimilarPicture, conflictPictures.ToArray());
                                }
                            }
                            else
                            {
                                picture.PriHash = null;
                                // 通过判断
                                md5Dict.Add(md5, picture);
                                AddPicture(pictureExtension);
                                context.Pictures.Add(picture);
                            }

                        }
                        else
                        {
                            pictureExtension.SetError(Resource.SamePictureInGallery, new Picture[] { conflictPicture });
                        }

                    }
                    else
                    {
                        pictureExtension.SetError(Resource.SamePictureInQueue, new Picture[] { md5Dict[md5] });
                    }
                }
                else
                {
                    pictureExtension.SetError(Resource.FileNotExist);
                }

            }
            TaskProgress.SetProgress(1.0f, Resource.SavingDatabaseChange);
            context.SaveChanges();
            TaskProgress.SetProgress(1.0f, Resource.OperateFinish);
            TaskProgress.TaskProgressStatus = TaskProgressStatus.Finished;
        }

        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="pictureExtension"></param>
        /// <param name="md5"></param>
        private static void AddPicture(PictureExtension pictureExtension)
        {
            var picture = pictureExtension.Picture;
            var newFileName = picture.MD5 + Path.GetExtension(picture.Path);
            var newFilePath = Path.Combine(UserSettingReader.UserSettings.PictureSettings.PictureStorePath, newFileName);
            File.Copy(picture.Path, newFilePath, true);
            picture.Path = newFilePath;
        }

        public static Picture ForceAddPicture(Picture picture)
        {
            return null;
            if(TaskProgress.TaskProgressStatus == TaskProgressStatus.Finished)
            {
                var res = PictureExtensions.SingleOrDefault(p => p.Picture.Path == picture.Path);
                //if(res)
            }
            else
            {
                //throw new Ex
            }
        }
    }


    /// <summary>
    /// 封装图片实体
    /// </summary>
    public class PictureExtension
    {
        public Picture Picture { get; set; }
        public enum AddResultEnum { Waiting = 0, Done, Updated, DoNothing }
        public AddResultEnum AddResult { get; set; }
        public string ErrorMessage { get; set; }
        public Picture[] ConflictPictures { get; set; }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="errorMessage"></param>
        public void SetError(string errorMessage, Picture[] conflictPictures = null)
        {
            SetAddResult(AddResultEnum.DoNothing, errorMessage, conflictPictures);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="errorMessage"></param>
        public void SetWarning(string errorMessage, Picture[] conflictPictures = null)
        {
            SetAddResult(AddResultEnum.Updated, errorMessage, conflictPictures);
        }

        /// <summary>
        /// 成功
        /// </summary>
        public void Succeed()
        {
            SetAddResult(AddResultEnum.Done);
        }

        public void SetAddResult(AddResultEnum addResult, string errorMessage = "", Picture[] conflictPictures = null)
        {
            AddResult = addResult;
            ErrorMessage = errorMessage;
            ConflictPictures = conflictPictures;
        }
    }


    /// <summary>
    /// 进度状态指示
    /// </summary>
    public class TaskProgressIndicator
    {
        /// <summary>
        /// 当前进度数值
        /// </summary>
        public float CurrentValue { get; set; }
        /// <summary>
        /// 步骤信息
        /// </summary>
        public string Information { get; set; }
        /// <summary>
        /// 当前步骤状态
        /// </summary>
        public TaskProgressStatus TaskProgressStatus { get; set; }
        /// <summary>
        /// 是否繁忙
        /// </summary>
        /// <returns></returns>
        public bool IsBusy => !(TaskProgressStatus == TaskProgressStatus.Running);
        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        public void SetProgress(float value,string text)
        {
            CurrentValue = value;
            Information = text;
        }
    }
    public enum TaskProgressStatus
    {
        Empty = 0,
        Running,
        Finished,
    }
}
