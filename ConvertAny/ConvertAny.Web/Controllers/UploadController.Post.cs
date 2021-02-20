using ConvertAny.Common.Enum;
using ConvertAny.Common.Helper;
using ConvertAny.Common.Models.Image;
using ConvertAny.Service.Models;
using ConvertAny.Web.Helper;
using ConvertAny.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConvertAny.Web.Controllers
{
    public partial class UploadController : Controller
    {
        [HttpPost]
        [AllowAnonymous]
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostDataAsync(RequestData requestData)
        {
            if (!requestData.ValidRequestData()) return Json(new ResponseResult
            {
                IsOk = false
            });

            ResponseData response = await ConvertImageAsync(requestData);

            IActionResult rs = await Task.Run(() => SetTempData(response));

            return rs;
        }

        public async Task<ResponseData> ConvertImageAsync(RequestData requestData)
        {
            Dictionary<string, ImageOutput> dict = _ecDict;

            if (requestData.IsCustomSize && requestData.CustomWidth.HasValue && requestData.CustomHeight.HasValue)
            {
                dict = new Dictionary<string, ImageOutput>
                {
                    {"自定義尺寸",new ImageOutput {
                        Width = requestData.CustomWidth.Value,
                        MaxHeight = requestData.CustomHeight.Value,
                        DPI = 72
                    }},
                };
            }

            Stream stream = requestData.Base64.GetStream();

            bool isPortait = requestData.Height > requestData.Width;

            ProcessData processData =
                new ProcessData(requestData.FileName, requestData.Size, requestData.Type, isPortait, requestData.Width, requestData.Height, dict);

            ResponseData rs = await SizingAsync(stream, processData);

            return rs;
        }

        private async Task<ResponseData> SizingAsync(Stream stream, ProcessData processData)
        {
            byte[] zipRs = await _convertProcess.ConvertProcessAsync(stream, processData);

            ResponseData response = new ResponseData
            {
                FileName = processData.FileName,
                ContentType = processData.Type,
                Result = zipRs
            };

            return response;
        }
    }
}
