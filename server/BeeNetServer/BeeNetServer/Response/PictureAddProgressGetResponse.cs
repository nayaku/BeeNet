using BeeNetServer.Background;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Response
{
    public class PictureAddProgressGetResponse
    {
        public List<PictureExtension> Pictures { get; set; }
        public TaskProgressIndicator Progress { get; set; }
        public PictureAddProgressGetResponse()
        {
            Pictures = PicturesAddProgress.PictureExtensions;
            Progress = PicturesAddProgress.TaskProgress;
        }
        public PictureAddProgressGetResponse(List<PictureExtension> pictures, TaskProgressIndicator progress)
        {
            Pictures = pictures;
            Progress = progress;
        }
    }
}
