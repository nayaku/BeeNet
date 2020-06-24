using BeeNetServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background
{
    /// <summary>
    /// 封装图片实体
    /// </summary>
    public class PictureExtension
    {
        public Picture Picture { get; set; }
        public enum AddResultEnum { Waiting = 0, Done, Updated, DoNothing }
        public AddResultEnum AddResult { get; set; }
        public string ErrorMessage { get; set; }
        public List<Picture> ConflictPictures { get; set; }

        /// <summary>
        /// 设置错误
        /// </summary>
        /// <param name="errorMessage"></param>
        public void SetError(string errorMessage, List<Picture> conflictPictures = null)
        {
            SetAddResult(AddResultEnum.DoNothing, errorMessage, conflictPictures);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="errorMessage"></param>
        public void SetWarning(string errorMessage, List<Picture> conflictPictures = null)
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

        public void SetAddResult(AddResultEnum addResult, string errorMessage = "", List<Picture> conflictPictures = null)
        {
            AddResult = addResult;
            ErrorMessage = errorMessage;
            ConflictPictures = conflictPictures;
        }
    }
}
