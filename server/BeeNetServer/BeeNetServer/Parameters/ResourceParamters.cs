using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters
{
    public abstract class ResourceParamtersBase
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = UserSettingReader.UserSettings.RequestSettings.PageNum;
        public string OrderBy { get; set; }
    }
    public abstract class ResourceParamters : ResourceParamtersBase
    {
        public string SearchKey { get; set; }

    }
}
