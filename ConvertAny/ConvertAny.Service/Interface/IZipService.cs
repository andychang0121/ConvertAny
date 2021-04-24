using System.Collections.Generic;
using ConvertAny.Common.Models;

namespace ConvertAny.Service.Interface
{
    public interface IZipService
    {
        byte[] GetZipData(IEnumerable<ZipData> data);
    }
}
