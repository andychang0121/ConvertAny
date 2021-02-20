using ConvertAny.Service.Process;
using Microsoft.AspNetCore.Mvc;

namespace ConvertAny.Web.Controllers
{
    //https://www.codeproject.com/Articles/2941/Resizing-a-Photographic-image-with-GDI-for-NET
    public partial class UploadController : Controller
    {
        private const string _contentType = "";

        private const string _downloadExt = "";

        public const string _zipContentType = "application/zip";

        private readonly IConvertProcess _convertProcess;

        [TempData]
        private string SessionData { get; set; }

        public UploadController(IConvertProcess convertProcess)
        {
            _convertProcess = convertProcess;
        }

        public IActionResult Index() => View();

        private static string GetFileName(string prefix, int w, int h, string extName)
            => $"{prefix}-{w}-{h}.{extName}";
    }
}
