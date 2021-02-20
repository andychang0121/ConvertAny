using ConvertAny.Web.Models.Image;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ConvertAny.Common.Enum;

namespace ConvertAny.Web.Helper
{
    public static class ImageHelper
    {
        public static byte[] ImageToByteArray(this Image image, ImageFormat imageFormat)
        {
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                //建立副本
                using (Bitmap bitmap = new Bitmap(image))
                {
                    //儲存圖片到 MemoryStream 物件，並且指定儲存影像之格式
                    bitmap.Save(ms, ImageFormat.Jpeg);
                    //設定資料流位置
                    ms.Position = 0;
                    //設定 buffer 長度
                    bytes = new byte[ms.Length];
                    //將資料寫入 buffer
                    ms.Read(bytes, 0, Convert.ToInt32(ms.Length));
                    //將所有緩衝區的資料寫入資料流
                    ms.Flush();
                }
            }
            return bytes;
        }

        public static Bitmap Resize(this Image image, double ratio, ImageDirection direction)
        {
            if (direction == ImageDirection.Portait)
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipX);
            }

            ImageProfile rs = new ImageProfile(image);

            return Resize(image, rs.Width, rs.Height, ratio);
        }

        public static Bitmap Resize(this Image originalImage, int originalWidth, int originalHeight, double ratio)
        {
            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);
            // -----
            Bitmap thumbnail = new Bitmap(newWidth, newHeight);

            thumbnail.SetResolution(120, 120);

            Graphics graphic = Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            graphic.Clear(Color.Transparent);
            graphic.DrawImage(originalImage, 0, 0, newWidth, newHeight);

            return thumbnail;
        }
    }
}
