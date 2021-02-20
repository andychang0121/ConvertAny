using ConvertAny.Service.Models;
using ConvertAny.Service.Process;
using System;
using System.IO;
using System.Threading.Tasks;
using ConvertAny.Service.Interface;

namespace ConvertAny.Service.Repository
{
    public class ConvertProcess : IConvertProcess
    {
        private readonly IImageService _imageService;
        private readonly IZipService _zipService;

        public ConvertProcess(IImageService imageService, IZipService zipService)
        {
            _imageService = imageService;
            _zipService = zipService;
        }

        public Task<byte[]> ConvertProcessAsync(Stream stream, ProcessData processData)
        {
            throw new NotImplementedException();
        }
    }
}
