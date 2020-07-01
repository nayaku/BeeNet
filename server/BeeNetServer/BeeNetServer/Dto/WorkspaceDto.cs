﻿using BeeNetServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Dto
{
    public class WorkspaceDto
    {
        public string Name { get; set; }
        public ushort Index { get; set; }
        public string Context { get; set; }

        public List<PictureBase> Pictures { get; set; }
        public List<PictureBase> ScreenShots { get; set; }
    }
}
