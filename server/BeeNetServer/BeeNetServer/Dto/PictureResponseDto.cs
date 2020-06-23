using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.Dto
{
    public class PictureResponseDto
    {
        public uint Id { get; set; }
        public DateTime AddTime { get; set; }
        public DateTime EditTime { get; set; }
        public string Path { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public List<PictureLabelResponseDto> PictureLabels { get; set; }
    }
    public class PictureLabelResponseDto
    {
        public string LabelName { get; set; }
        public uint Color { get; set; }
        public int Num { get; set; }
    }
}
