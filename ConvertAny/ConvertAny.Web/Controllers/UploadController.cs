using ConvertAny.Web.Helper;
using ConvertAny.Web.Models.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ConvertAny.Common.Enum;

namespace ConvertAny.Web.Controllers
{


    //https://www.codeproject.com/Articles/2941/Resizing-a-Photographic-image-with-GDI-for-NET
    public class UploadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Post(IFormCollection formdata)
        {
            Dictionary<string, ImageOutput> dict = new Dictionary<string, ImageOutput>
            {
                {"PCHOME",new ImageOutput {
                    Width = 800,
                    MaxHeight = 120,
                    DPI = 100
                }},
                {"Momo",new ImageOutput {
                    Width = 1000,
                    MaxHeight = 300,
                    DPI = 200
                }},
                {"Shopee",new ImageOutput {
                    Width = 600,
                    MaxHeight = 600
                }}
            };

            IFormFileCollection files = formdata.Files;

            foreach (IFormFile formFile in files)
            {
                if (formFile.Length > 0)
                {
                    bool isPortait = formdata["isPortait"] == "true";

                    string ext = formFile.ContentType.Split("/").LastOrDefault();

                    Stream fileStream = formFile.OpenReadStream();

                    using (Bitmap image = new Bitmap(Image.FromStream(fileStream)))
                    {
                        double ratio = isPortait ? 800 / (double)image.Height : 800 / (double)image.Width;

                        ImageDirection direction = isPortait ? ImageDirection.Portait : ImageDirection.LandScape;

                        Image newImage = image.Resize(ratio, direction);

                        string realFileName = GetFileName("PCHOME", newImage.Width, newImage.Height, ext);

                        string fileName = $"C:\\temp\\{realFileName}";

                        newImage.Save(fileName);

                        var guid = Guid.NewGuid().ToString();

                        var f1 = newImage.ImageToByteArray(image.RawFormat);

                        ResponseData rs = new ResponseData
                        {
                            Result = f1,
                            ContentType = formFile.ContentType,
                            FileName = fileName
                        };

                        TempData[guid] = rs;

                        return Ok();
                        //byte[] rs = ImageToByteArray(newImage);
                        //return base.File(rs, "image/jpeg", fileName);
                    }
                }
            }

            return Ok(new { string.Empty });
        }

        [HttpGet]
        public IActionResult Get(string guid)
        {
            ResponseData rs = (ResponseData)TempData[guid];

            return base.File(rs.Result, rs.ContentType, rs.FileName);
        }

        private static string GetFileName(string prefix, int w, int h, string extName)
            => $"{prefix}-{w}-{h}.{extName}";
    }
}
