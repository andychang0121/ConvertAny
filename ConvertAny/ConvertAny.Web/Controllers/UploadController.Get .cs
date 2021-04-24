using ConvertAny.Web.Helper;
using ConvertAny.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ConvertAny.Web.Controllers
{
    public partial class UploadController
    {
        [HttpGet]
        public async Task<ActionResult> Get(string tempdataKey)
            => await Task.Run(() => GetTempData(tempdataKey));

        private ActionResult GetTempData(string encodeKey)
        {
            string tempRs = (string)TempData[encodeKey];

            ResponseData jsonRs = tempRs.Deserialize<ResponseData>();

            byte[] bytes = (byte[])jsonRs.Result;

            string originalFileName = jsonRs.FileName?.Split('.').FirstOrDefault() ?? encodeKey;

            string fileName = $"{originalFileName}.{_downloadExtName}";

            FileContentResult rs = File(bytes, _zipContentType, fileName);

            return rs;
        }
    }
}
