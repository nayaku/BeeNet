using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters.Label
{
    public class LabelGetsParamter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = UserSettingReader.UserSettings.RequestSettings.PageNum;
        public string Name { get; set; }
    }
}
