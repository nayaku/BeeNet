using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters
{

    public class PictureResourceParamters
    {
        public string SearchKey { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; }
        public string OrderBy { get; set; } = "AddTime desc";

    }
}
