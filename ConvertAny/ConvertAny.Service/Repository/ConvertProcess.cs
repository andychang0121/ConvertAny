using ConvertAny.Common.Helper;
using ConvertAny.Common.Models;
using ConvertAny.Common.Models.Image;
using ConvertAny.Service.Interface;
using ConvertAny.Service.Models;
using ConvertAny.Service.Process;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        public async Task<byte[]> ConvertProcessAsync(Stream stream, ProcessData processData)
        {
            IEnumerable<ZipData> zipDatas = await ConvertProcessAsync(
                stream, processData.FileName, processData.IsPortait, processData.OutputDict);

            byte[] bytes = await Task.Run(() => _zipService.GetZipData(zipDatas));

            return bytes;
        }

        private async Task<IEnumerable<ZipData>> ConvertProcessAsync(Stream stream, string fileName, bool isPortait, Dictionary<string, ImageOutput> outputProfile)
        {
            List<ZipData> zipDatas = new List<ZipData>();

            foreach (KeyValuePair<string, ImageOutput> ec in outputProfile)
            {
                string ecName = ec.Key;
                ImageOutput ecSetting = ec.Value;

                byte[] bytes = await _imageService.GetImageBytesAsync(stream, fileName, isPortait, ecSetting.Width,
                    ecSetting.MaxHeight, ecName);

                string zipFileName = ZipHelper.GetZipFileName(fileName, ecSetting.Width, ecSetting.MaxHeight, ecName);

                ZipData zipData = new ZipData(bytes, zipFileName, string.Empty);

                zipDatas.Add(zipData);
            }

            return zipDatas;
        }
    }
}
