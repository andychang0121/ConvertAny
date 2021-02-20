using ConvertAny.Common.Enum;
using System.Drawing.Imaging;

namespace ConvertAny.Common.Models.Image
{
    public struct ImageProfile
    {
        private int _height { get; set; }
        private int _width { get; set; }
        private ImageFormat _format { get; set; }
        private ImageDirection _imageDirection { get; set; }

        public ImageProfile(System.Drawing.Image image)
        {
            _height = image?.Height ?? 0;
            _width = image?.Width ?? 0;
            _format = image?.RawFormat;
            _imageDirection = _height >= _width ? ImageDirection.Portait : ImageDirection.LandScape;
        }

        public int Height => _height;
        public int Width => _width;
        public ImageFormat Format => _format;
        public ImageDirection ImageDirection => _imageDirection;
    }
}
