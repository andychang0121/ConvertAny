using System.IO;
using System.Threading.Tasks;
using ConvertAny.Service.Models;

namespace ConvertAny.Service.Process
{
    public interface IConvertProcess
    {
        Task<byte[]> ConvertProcessAsync(Stream stream, ProcessData processData);
    }
}
