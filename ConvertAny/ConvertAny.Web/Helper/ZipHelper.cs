using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ConvertAny.Web.Helper
{
    public class ZipData
    {
        public string FileName { get; set; }

        public byte[] Bytes { get; set; }

        public string FolderName { get; set; }
    }

    public class ZipHelper
    {
        public static byte[] ZipData(IEnumerable<ZipData> data)
        {
            using (MemoryStream zipStream = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update))
                {
                    foreach (ZipData zip in data)
                    {
                        string zipFileName = $"{zip.FileName}";

                        ZipArchiveEntry entry = zipArchive.CreateEntry(zipFileName);

                        using (Stream entryStream = entry.Open())
                        {
                            byte[] buff = zip.Bytes;
                            entryStream.Write(buff, 0, buff.Length);
                        }
                    }
                }
                return zipStream.ToArray();
            }
        }

        public static Dictionary<string, byte[]> UnzipData(byte[] zip)
        {
            var dict = new Dictionary<string, byte[]>();
            using (var msZip = new MemoryStream(zip))
            {
                using (var archive = new ZipArchive(msZip, ZipArchiveMode.Read))
                {
                    archive.Entries.ToList().ForEach(entry =>
                    {
                        //e.FullName可取得完整路徑
                        if (string.IsNullOrEmpty(entry.Name)) return;
                        using (var entryStream = entry.Open())
                        {
                            using (var msEntry = new MemoryStream())
                            {
                                entryStream.CopyTo(msEntry);
                                dict.Add(entry.Name, msEntry.ToArray());
                            }
                        }
                    });
                }
            }
            return dict;
        }
    }
}
