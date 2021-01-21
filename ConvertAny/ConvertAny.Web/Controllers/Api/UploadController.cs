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

            bool[] isPortaits = ((string)form["isPortaits"])
                .GetStringArray()
                .GetStingArrayToBoolean()
                ?.ToArray();

            IFormFileCollection files = form.Files;

            IEnumerable<ZipData> entryFiles = GetFileResult(files, isPortaits, dict);

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

        [HttpGet]
        [Route("Get")]
        public IActionResult Get()
        {
            ResponseData rs = JsonConvert.DeserializeObject<ResponseData>(TempData);

            byte[] bytes = rs.Result;

            string fileName = $"{Guid.NewGuid()}.zip";

            return File(bytes, _zipContentType, fileName);
        }

        private static double GetRatio(bool isPortait, int limitPx, int width, int height)
        => isPortait ? limitPx / (double)height : 800 / (double)width;

        private static ValueTuple<string, byte[]> GetFileResult(IFormFile formFile, bool isPortait, int limitPx)
        {
            string ext = formFile.FileName.Split(".").LastOrDefault();

            string originalFileName = formFile.FileName.Split(".").FirstOrDefault();

            Stream fileStream = formFile.OpenReadStream();

            using (Bitmap image = new Bitmap(Image.FromStream(fileStream)))
            {
                double ratio = GetRatio(isPortait, limitPx, image.Width, image.Height);

                ImageDirection direction = isPortait ? ImageDirection.Portait : ImageDirection.LandScape;

                Bitmap newImage = image.Resize(ratio, direction);

                string fileName = GetFileName(originalFileName, newImage.Width, newImage.Height, ext);

                byte[] bytes = newImage.ImageToByteArray(image.RawFormat);

                return (fileName, bytes);
            }
        }

        private static IEnumerable<ZipData> GetFileResult(IFormFileCollection files,
            IReadOnlyList<bool> isPortaits, Dictionary<string, ImageOutput> ecProfile)
        {
            List<ZipData> entryFiles = new List<ZipData>();

            foreach (KeyValuePair<string, ImageOutput> ec in ecProfile)
            {
                string ecName = ec.Key;
                ImageOutput ecSetting = ec.Value;

                for (int i = 0; i < files.Count; i++)
                {
                    bool isPortait = isPortaits?[i] ?? false;

                    IFormFile formFile = files[i];

                    (string fileName, byte[] bytes) = GetFileResult(formFile, isPortait, ecSetting.Width);

                    fileName = $"{ecName}-{fileName}";

                    ZipData zipData = new ZipData
                    {
                        FileName = fileName,
                        Bytes = bytes,
                        FolderName = string.Empty
                    };

                    entryFiles.Add(zipData);
                }
            }

            return entryFiles;
        }

        private static string GetFileName(string prefix, int w, int h, string extName)
            => $"{prefix}-{w}x{h}.{extName}";
    }
}
