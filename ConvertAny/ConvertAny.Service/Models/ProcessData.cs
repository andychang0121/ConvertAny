using ConvertAny.Common.Models.Image;
using System.Collections.Generic;

namespace ConvertAny.Service.Models
{
    public struct ProcessData
    {
        private string _fileName { get; set; }

        private int _size { get; set; }

        private string _type { get; set; }

        private int _width { get; set; }

        private int _height { get; set; }

        private bool _isPortait { get; set; }

        private Dictionary<string, ImageOutput> _outputDict { get; set; }

        public ProcessData(string fileName, int size, string type, bool isPortait, int width, int height, Dictionary<string, ImageOutput> outputDict)
        {
            _fileName = fileName;
            _size = size;
            _type = type;
            _width = width;
            _height = height;
            _isPortait = isPortait;
            _outputDict = outputDict;
        }

        public string FileName => _fileName;

        public int Size => _size;

        public string Type => _type;

        public int Width => _width;

        public int Height => _height;

        public bool IsPortait => _isPortait;

        public Dictionary<string, ImageOutput> OutputDict => _outputDict;
    }
}
