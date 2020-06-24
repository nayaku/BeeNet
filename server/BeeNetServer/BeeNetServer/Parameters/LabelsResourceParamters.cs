using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters
{
    public class LabelsResourceParamters:ResourceParamters
    {
        public LabelsResourceParamters()
        {
            OrderBy = "Num desc";
        }
    }
}
