using ConvertAny.Common.Models.Image;
using ConvertAny.Service.Process;
using ConvertAny.Web.Helper;
using ConvertAny.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ConvertAny.Web.Controllers
{
    //https://www.codeproject.com/Articles/2941/Resizing-a-Photographic-image-with-GDI-for-NET
    public partial class UploadController : Controller
    {
        private readonly Dictionary<string, ImageOutput> _ecDict =
            new Dictionary<string, ImageOutput>
            {
                {"PCHOME-尺寸1",new ImageOutput {
                    Width = 800,
                    MaxHeight = 120,
                    DPI = 72
                }},
                {"PCHOME-尺寸2",new ImageOutput {
                    Width = 360,
                    MaxHeight = 360,
                    DPI = 72
                }},
                {"PCHOME-尺寸3",new ImageOutput {
                    Width = 475,
                    MaxHeight = 270,
                    DPI = 72
                }},
                {"PCHOME-尺寸4",new ImageOutput {
                    Width = 414,
                    MaxHeight = 270,
                    DPI = 72
                }},
                {"PCHOME-尺寸5",new ImageOutput {
                    Width = 314,
                    MaxHeight = 282,
                    DPI = 72
                }},
                {"Momo",new ImageOutput {
                    Width = 1000,
                    MaxHeight = 300,
                    DPI = 200
                }},
                {"Momo-館內輪播看板",new ImageOutput {
                    Width = 818,
                    MaxHeight = 370,
                    DPI = 200
                }},
                {"Momo-輪播看板",new ImageOutput {
                    Width = 960,
                    MaxHeight = 480,
                    DPI = 200
                }},
                {"Shopee-輪播看板",new ImageOutput {
                    Width = 2000,
                    MinHeight = 100,
                    MaxHeight = 2200
                }},
                {"Shopee-簡易看板",new ImageOutput {
                    Width = 600,
                    MaxHeight = 600
                }},
                {"Shopee-簡易圖片-圖片點擊區",new ImageOutput {
                    Width = 1200,
                    MinHeight = 100,
                    MaxHeight = 2200
                }}
            };

        public const string _zipContentType = "application/x-zip-compressed";
        public const string _downloadExtName = "zip";

        private readonly IConvertProcess _convertProcess;

        [TempData]
        private string SessionData { get; set; }

        public UploadController(IConvertProcess convertProcess)
        {
            _convertProcess = convertProcess;
        }

        public IActionResult Index() => View();

        public IActionResult SetTempData(ResponseData data)
        {
            string key = Guid.NewGuid().ToString();

            TempData[key] = data.Serialize();

            ResponseResult<string> rs = new ResponseResult<string>
            {
                IsOk = true,
                Data = key
            };
            return Json(rs);
        }

        private static string GetFileName(string prefix, int w, int h, string extName)
            => $"{prefix}-{w}-{h}.{extName}";
    }
}
