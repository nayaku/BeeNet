﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Parameters
{

    public class PictureResourceParamters : ResourceParamters
    {
        public PictureResourceParamters()
        {
            OrderBy = "CreatedTime desc";
        }

    }
}
