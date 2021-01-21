using ConvertAny.Web.Helper;
using ConvertAny.Web.Models;
using ConvertAny.Web.Models.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ConvertAny.Web.Controllers.Api
{
    [Route("api/Upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        public const string _zipContentType = "application/zip";

        [TempData]
        public string TempData { get; set; }

        [HttpPost]
        [Route("Post")]
        public IActionResult Post(IFormCollection form)
        {
            Dictionary<string, ImageOutput> dict = new Dictionary<string, ImageOutput>
            {
                {"PCHOME",new ImageOutput {
                    MaxWidth = 800,
                    Height = 120,
                    DPI = 100
                }},
                {"Momo",new ImageOutput {
                    MaxWidth = 1000,
                    Height = 300,
                    DPI = 200
                }},
                {"Shopee",new ImageOutput {
                    MaxWidth = 600,
                    Height = 600
                }}
            };

            bool[] isPortaits = ((string)form["isPortaits"])
                .GetStringArray()
                .GetStingArrayToBoolean()
                ?.ToArray();

            IFormFileCollection files = form.Files;

            Dictionary<string, byte[]> entryFiles = GetFileResult(files, isPortaits);

            byte[] zipRs = ZipHelper.ZipData(entryFiles);

            ResponseData rs = new ResponseData
            {
                Result = zipRs
            };

            TempData = JsonConvert.SerializeObject(rs);

            return Ok(new ResponseResult
            {
                IsOk = true,
                Message = "Generate ok!"
            });
        }

        private static ValueTuple<string, byte[]> GetFileResult(IFormFile formFile, bool isPortait)
        {
            string ext = formFile.FileName.Split(".").LastOrDefault();

            string originalFileName = formFile.FileName.Split(".").FirstOrDefault();

            Stream fileStream = formFile.OpenReadStream();

            using (Bitmap image = new Bitmap(Image.FromStream(fileStream)))
            {
                double ratio = isPortait ? 800 / (double)image.Height : 800 / (double)image.Width;

                ImageDirection direction = isPortait ? ImageDirection.Portait : ImageDirection.LandScape;

                Bitmap newImage = image.Resize(ratio, direction);

                string fileName = GetFileName(originalFileName, newImage.Width, newImage.Height, ext);

                byte[] bytes = newImage.ImageToByteArray(image.RawFormat);

                return (fileName, bytes);
            }
        }

        private static Dictionary<string, byte[]> GetFileResult(IFormFileCollection files, IReadOnlyList<bool> isPortaits)
        {
            Dictionary<string, byte[]> entryFiles = new Dictionary<string, byte[]>();

            for (int i = 0; i < files.Count; i++)
            {
                bool isPortait = isPortaits?[i] ?? false;

                IFormFile formFile = files[i];

                (string item1, byte[] bytes) = GetFileResult(formFile, isPortait);

                entryFiles.Add(item1, bytes);

                //if (formFile.Length > 0)
                //{
                //    string ext = formFile.FileName.Split(".").LastOrDefault();

                //    string originalFileName = formFile.FileName.Split(".").FirstOrDefault();

                //    Stream fileStream = formFile.OpenReadStream();

                //    using (Bitmap image = new Bitmap(Image.FromStream(fileStream)))
                //    {
                //        double ratio = isPortait ? 800 / (double)image.Height : 800 / (double)image.Width;

                //        ImageDirection direction = isPortait ? ImageDirection.Portait : ImageDirection.LandScape;

                //        Bitmap newImage = image.Resize(ratio, direction);

                //        string fileName = GetFileName(originalFileName, newImage.Width, newImage.Height, ext);

                //        byte[] bytes = newImage.ImageToByteArray(image.RawFormat);

                //        entryFiles.Add(fileName, bytes);
                //    }
                //}
            }

            return entryFiles;
        }


        [HttpGet]
        [Route("Get")]
        public IActionResult Get()
        {
            ResponseData rs = JsonConvert.DeserializeObject<ResponseData>(TempData);

            byte[] bytes = rs.Result;

            string fileName = $"{Guid.NewGuid()}.zip";

            return File(bytes, _zipContentType, fileName);
        }

        private static string GetFileName(string prefix, int w, int h, string extName)
            => $"{prefix}-{w}x{h}.{extName}";
    }
}
