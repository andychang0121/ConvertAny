using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConvertAny.Web.Helper
{
    public static class IOHelper
    {
        public static async Task<string> GetBase64(this byte[] bytes)
            => await Task.Run(() => Convert.ToBase64String(bytes));

        public static byte[] GetBytes(this string base64)
        {
            if (string.IsNullOrEmpty(base64)) return null;

            string base64Rs = base64.Split(',').LastOrDefault();

            return string.IsNullOrEmpty(base64Rs) ? null : Convert.FromBase64String(base64Rs);
        }

        //public static Stream GetStream(this string base64)
        //{
        //    byte[] bytes = base64.GetBytes();

        //    Stream stream = new MemoryStream(bytes);

        //    return stream;
        //}

        public static async Task<string> GetBase64(this object obj)
        {
            if (obj == null) return string.Empty;

            return await ((byte[])obj).GetBase64();
        }

    }
}