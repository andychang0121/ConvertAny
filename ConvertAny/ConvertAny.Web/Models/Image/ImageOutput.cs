using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConvertAny.Web.Models.Image
{
    public class ImageOutput
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public int Resolution { get; set; }
        public int DPI { get; set; }
    }
}
