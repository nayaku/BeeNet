using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters
{
    public class ScreenShotsResourceParamters:ResourceParamtersBase
    {
        public ScreenShotsResourceParamters()
        {
            OrderBy = "CreatedTime desc";
        }
    }
}
