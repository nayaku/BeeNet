using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background
{
    public class PictureStoreOutputProgressIndicator
    {
        /// <summary>
        /// 当前进度数值
        /// </summary>
        public float CurrentValue { get; set; }
        /// <summary>
        /// 步骤信息
        /// </summary>
        public string Information { get; set; }
        public PictureStoreOutputProgressStatusEnum TaskProgressStatus { get; set; }
        public string ResultUrl { get; set; }
    }
    public enum PictureStoreOutputProgressStatusEnum
    {
        Running,
        Finish
    }
}
