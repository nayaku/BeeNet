using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters
{

    public class PictureGetsParamter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = UserSettingReader.UserSettings.RequestSettings.PageNum;
        public bool IsOrderByAddTimeDesc { get; set; } = true;
        public List<string> LimitLabels { get; set; }

    }
}
