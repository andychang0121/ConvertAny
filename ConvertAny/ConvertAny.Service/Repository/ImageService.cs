using ConvertAny.Common.Helper;
using ConvertAny.Common.Models.Image;
using ConvertAny.Service.Interface;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ConvertAny.Service.Repository
{
    public class ImageService : IImageService
    {
        public async Task<byte[]> GetImageBytesAsync(Stream stream, string fileName, bool isPortait, int width, int height, string ecName)
        {
            byte[] bytes = await ResizeAsync(stream, isPortait, width, height, ImageFormat.Jpeg);

            return bytes;
        }

        private async Task<byte[]> ResizeAsync(Stream stream, bool isPortait, int reWidth, int reHeight, ImageFormat imageFormat)
        {
            using (Bitmap image = new Bitmap(stream))
            {
                double ratio = ImageHelper.GetRatio(isPortait, reWidth, image.Width, image.Height);

                Bitmap newImage = await ResizeAsync(image, ratio, isPortait, reWidth, reHeight);

                byte[] bytes = ImageHelper.ImageToBytes(newImage, imageFormat);

                return bytes;
            }
        }

        private async Task<Bitmap> ResizeAsync(Image image, double ratio, bool isPortait, int width, int height)
        {
            if (isPortait)
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipX);
            }

            Bitmap bitmapRs = await ResizeAsync(image, ratio, width, height);

            return bitmapRs;
        }

        /// <summary>
        /// Todo:
        /// </summary>
        /// <param name="image"></param>
        /// <param name="ratio"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xDpi"></param>
        /// <param name="yDpi"></param>
        /// <returns></returns>
        private async Task<Bitmap> ResizeAsync(Image image, double ratio, int width, int height, float xDpi = 72, float yDpi = 72)
        {
            return await Task.Run(() =>
            {
                ImageProfile originalImageProfile = new ImageProfile(image);

                // now we can get the new height and width
                int newWidth = Convert.ToInt32(originalImageProfile.Width * ratio);
                int newHeight = Convert.ToInt32(originalImageProfile.Height * ratio);

                if ((int)ratio == 1)
                {
                    newWidth = width;
                    newHeight = height;
                }

                // -----
                Bitmap thumbnail = new Bitmap(newWidth, newHeight);

                thumbnail.SetResolution(xDpi, yDpi);

                Graphics graphic = Graphics.FromImage(thumbnail);

                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;

                graphic.Clear(Color.Transparent);
                graphic.DrawImage(image, 0, 0, newWidth, newHeight);
                graphic.Dispose();

                return thumbnail;
            });
        }
    }
}
