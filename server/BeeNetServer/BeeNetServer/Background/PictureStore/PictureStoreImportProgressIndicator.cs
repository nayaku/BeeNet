using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background.PictureStore
{
    public class PictureStoreImportProgressIndicator
    {
        /// <summary>
        /// 当前进度数值
        /// </summary>
        public float CurrentValue { get; set; }
        /// <summary>
        /// 步骤信息
        /// </summary>
        public string Information { get; set; }
        public TaskProgressEnum TaskProgressStatus { get; set; }

        public List<ResultStatusEnum> LabelResults { get; set; }
        public List<PictureStoreImportProgressPictureResult> PictureResults { get; set; }

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
    }

    public class PictureStoreImportProgressPictureResult
    {
        public ResultStatusEnum Result { get; set; }
        public uint ConflictPictureId { get; set; }
    }
}
