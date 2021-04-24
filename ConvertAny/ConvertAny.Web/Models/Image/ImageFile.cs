using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConvertAny.Common.Enum;

namespace ConvertAny.Web.Models.Image
{
    public struct ImageFile
    {
        private bool _isPortait { get; set; }
        private string _ext { get; set; }
        private string _fileName { get; set; }
        private Stream _stream { get; set; }

        public ImageFile(Stream stream, string fileName, bool isPortait)
        {
            _isPortait = isPortait;
            _ext = fileName.Split('.').LastOrDefault();
            _fileName = fileName.Split('.').FirstOrDefault();
            _stream = stream;
            _stream.Position = 0;
        }

        public Stream Stream => _stream;

        public bool IsPortait => _isPortait;

        public ImageDirection Direction => _isPortait ? ImageDirection.Portait : ImageDirection.LandScape;

        public string FileName => _fileName;

        public string Ext => _ext;
    }
}
