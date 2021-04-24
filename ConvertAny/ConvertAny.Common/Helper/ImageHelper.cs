using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ConvertAny.Common.Helper
{
    public class ImageHelper
    {
        public static byte[] ImageToBytes(Image image, ImageFormat imageFormat)
        {
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bitmap = new Bitmap(image))
                {
                    bitmap.Save(ms, imageFormat);
                    ms.Position = 0;
                    bytes = new byte[ms.Length];
                    ms.Read(bytes, 0, Convert.ToInt32(ms.Length));
                    ms.Flush();
                }
            }
            return bytes;
        }

        public static double GetRatio(bool isPortait, int assignWidth, int width, int height)
            => assignWidth == width ?
                1 :
                isPortait ? assignWidth / (double)height : assignWidth / (double)width;
    }
}
