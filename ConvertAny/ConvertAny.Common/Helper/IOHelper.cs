using System;
using System.IO;
using System.Linq;

namespace ConvertAny.Common.Helper
{
    public static class IOHelper
    {
        public static byte[] GetBytes(this string base64)
        {
            if (string.IsNullOrEmpty(base64)) return null;

            string base64Rs = base64.Split(',').LastOrDefault();

            return string.IsNullOrEmpty(base64Rs) ? null : Convert.FromBase64String(base64Rs);
        }

        public static Stream GetStream(this string base64)
        {
            byte[] bytes = base64.GetBytes();

            Stream stream = new MemoryStream(bytes);

            return stream;
        }
    }
}
