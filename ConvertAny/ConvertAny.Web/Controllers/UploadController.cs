using Microsoft.AspNetCore.Mvc;

namespace ConvertAny.Web.Controllers
{
    public class UploadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Post()
        {
            return View();
        }
    }
}
