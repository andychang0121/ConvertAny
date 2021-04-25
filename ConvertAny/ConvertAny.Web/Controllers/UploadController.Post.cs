using ConvertAny.Common.Helper;
using ConvertAny.Common.Models.Image;
using ConvertAny.Service.Models;
using ConvertAny.Web.Helper;
using ConvertAny.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConvertAny.Web.Controllers
{
    public partial class UploadController
    {
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody] RequestData requestData)
        {
            if (!requestData.ValidRequestData()) return Json(new ResponseResult
            {
                IsOk = false
            });

            ResponseData response = await ConvertImageAsync(requestData);

            ResponseData rs = new ResponseData
            {
                Result = await response.Result.GetBase64(),
                FileName = response.FileName.SetDownloadFileName("zip"),
                ContentType = response.ContentType
            };

            return new JsonResult(new { Data = rs });
        }

        private async Task<ResponseData> ConvertImageAsync(RequestData requestData)
        {
            Dictionary<string, ImageOutput> dict = _ecDict;

            if (requestData.IsCustomSize)
            {
                dict = new Dictionary<string, ImageOutput>
                {
                    {"自定義尺寸",new ImageOutput {
                        Width = int.Parse(requestData.CustomWidth),
                        MaxHeight = int.Parse(requestData.CustomHeight),
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
