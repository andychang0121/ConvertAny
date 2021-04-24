using ConvertAny.Common.Models;
using ConvertAny.Service.Interface;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ConvertAny.Service.Repository
{
    public class ZipService : IZipService
    {
        public byte[] GetZipData(IEnumerable<ZipData> zipDatas)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(ms, ZipArchiveMode.Update))
                {
                    foreach (ZipData zip in zipDatas)
                    {
                        string zipFileName = $"{zip.FileName}";

                        ZipArchiveEntry entry = zipArchive.CreateEntry(zipFileName);

                        using (Stream stream = entry.Open())
                        {
                            byte[] buff = zip.Bytes;
                            stream.Write(buff, 0, buff.Length);
                        }
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
