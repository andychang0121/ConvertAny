using ConvertAny.Web.Models;
using ConvertAny.Web.Models.Image;
using Microsoft.AspNetCore.Mvc;

namespace ConvertAny.Web.Controllers
{
    public partial class UploadController : Controller
    {
        [HttpGet]
        public IActionResult Get(string guid)
        {
            ResponseData rs = (ResponseData)TempData[guid];

            return base.File(rs.Result, rs.ContentType, rs.FileName);
        }
    }
}
