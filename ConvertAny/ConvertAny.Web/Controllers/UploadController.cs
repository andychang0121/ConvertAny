using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ConvertAny.Web.Controllers
{
    //https://www.codeproject.com/Articles/2941/Resizing-a-Photographic-image-with-GDI-for-NET
    public class UploadController : Controller
    {
        public enum ImageDirection
        {
            Portait = 0,
            LandScape = 1
        }

        public struct ImageProfile
        {
            private int _height { get; set; }
            private int _width { get; set; }
            private ImageFormat _format { get; set; }
            private ImageDirection _imageDirection { get; set; }

            public ImageProfile(Image image)
            {
                _height = image?.Height ?? 0;
                _width = image?.Width ?? 0;
                _format = image?.RawFormat;
                _imageDirection = _height >= _width ? ImageDirection.Portait : ImageDirection.LandScape;
            }

            public int Height => _height;
            public int Width => _width;
            public ImageFormat Format => _format;
            public ImageDirection ImageDirection => _imageDirection;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormCollection formdata)
        {
            IFormFileCollection files = formdata.Files;

            foreach (IFormFile formFile in files)
            {
                if (formFile.Length > 0)
                {
                    Stream fileStream = formFile.OpenReadStream();

                    using (Bitmap image = new Bitmap(Image.FromStream(fileStream)))
                    {
                        ImageProfile imageProfile = new ImageProfile(image);

                    }

                    var name = Guid.NewGuid().ToString();

                    var filePath = "C:\\temp\\" + name + ".jpg";

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }


            return View();
        }
    }
}
