using ConvertAny.Web.Models.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using ConvertAny.Web.Helper;
using ConvertAny.Web.Models;
using Newtonsoft.Json;

namespace ConvertAny.Web.Controllers.Api
{
    [Route("api/Upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [TempData]
        public string TempData { get; set; }

        [HttpPost]
        [Route("Post")]
        public IActionResult Post(IFormCollection form)
        {
            Dictionary<string, ImageOutput> dict = new Dictionary<string, ImageOutput>
            {
                {"PCHOME",new ImageOutput {
                    Width = 800,
                    Height = 120,
                    DPI = 100
                }},
                {"Momo",new ImageOutput {
                    Width = 1000,
                    Height = 300,
                    DPI = 200
                }},
                {"Shopee",new ImageOutput {
                    Width = 600,
                    Height = 600
                }}
            };

            IFormFileCollection files = form.Files;

            foreach (IFormFile formFile in files)
            {
                if (formFile.Length > 0)
                {
                    bool isPortait = form["isPortait"] == "true";

                    string ext = formFile.FileName.Split(".").LastOrDefault();

                    Stream fileStream = formFile.OpenReadStream();

                    using (Bitmap image = new Bitmap(Image.FromStream(fileStream)))
                    {
                        double ratio = isPortait ? 800 / (double)image.Height : 800 / (double)image.Width;

                        ImageDirection direction = isPortait ? ImageDirection.Portait : ImageDirection.LandScape;

                        Bitmap newImage = image.Resize(ratio, direction);

                        string fileName = GetFileName("PCHOME", newImage.Width, newImage.Height, ext);

                        byte[] bytes = newImage.ImageToByteArray(image.RawFormat);

                        ResponseData rs = new ResponseData
                        {
                            Result = bytes,
                            ContentType = formFile.ContentType,
                            FileName = fileName
                        };

                        TempData = JsonConvert.SerializeObject(rs);

                        return Ok(new ResponseResult
                        {
                            IsOk = true,
                            Message = $"{fileName} Generate ok!"
                        });
                    }
                }
            }

            return Ok(new ResponseResult
            {
                IsOk = true,
                Message = "Generate ok!"
            });
        }

        [HttpGet]
        [Route("Get")]
        public IActionResult Get()
        {
            ResponseData rs = JsonConvert.DeserializeObject<ResponseData>(TempData);

            byte[] bytes = rs.Result;

            var fileName = $"{Guid.NewGuid()}-{rs.FileName}";

            return File(bytes, MediaTypeNames.Application.Octet, fileName);
        }

        private static string GetFileName(string prefix, int w, int h, string extName)
            => $"{prefix}-{w}-{h}.{extName}";


        private void SaveImage(Bitmap bitmap, string ext)
        {
            string realFileName = GetFileName("PCHOME", bitmap.Width, bitmap.Height, ext);

            string fileName = $"C:\\temp\\{realFileName}";

            bitmap.Save(fileName);
        }
    }

}
