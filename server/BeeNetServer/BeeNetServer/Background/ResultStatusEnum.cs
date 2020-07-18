using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Background
{
    /// <summary>
    /// 结果的枚举
    /// </summary>
    public enum ResultStatusEnum
    {
        Pending =0,
        Processing,
        Done,
        DoNothing,
        Updated,
        Conflict
    }
}
