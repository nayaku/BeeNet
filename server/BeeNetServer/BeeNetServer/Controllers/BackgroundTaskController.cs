using System.Threading.Tasks;
using BeeNetServer.Background.AddPicture;
using BeeNetServer.Background.PictureStore;
using Microsoft.AspNetCore.Mvc;
namespace BeeNetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundTaskController : ControllerBase
    {
        [HttpGet("Add")]
        public AddPicturesProgressIndicator GetAddProgress()
        {
            return PicturesAddProgress.TaskProgress;
        }

        [HttpGet("Import")]
        public PictureStoreImportProgressIndicator GetImportProgress()
        {
            return PictureStoreProgress.ImportTaskProgress;
        }

        [HttpGet("Export")]
        public PictureStoreExportProgressIndicator GetExportProgress()
        {
            return PictureStoreProgress.ExportTaskProgress;
        }

    }
}
