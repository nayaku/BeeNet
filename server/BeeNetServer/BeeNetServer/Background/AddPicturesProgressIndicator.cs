using AutoMapper.Configuration.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background
{

    /// <summary>
    /// 进度状态指示
    /// </summary>
    public class AddPicturesProgressIndicator
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
        public AddPicturesProgressStatus TaskProgressStatus { get; set; }
        public List<AddPicturesProgressResult> PictureResults { get; set; }
        /// <summary>
        /// 是否繁忙
        /// </summary>
        /// <returns></returns>
        public bool IsBusy => TaskProgressStatus == AddPicturesProgressStatus.Running;
        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        public void SetProgress(float value, string text)
        {
            CurrentValue = value;
            Information = text;
        }
        public AddPicturesProgressIndicator(int capacity)
        {
            PictureResults = Enumerable.Repeat(new AddPicturesProgressResult(), capacity).ToList();
        }
    }
    public enum AddPicturesProgressStatus
    {
        Running,
        Finished,
    }

}
