using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork.Modes.FilesOperating
{
    public class FileReader : IDisposable
    {
        private FileStream _fileStream;
        private int _blockSize;
        private int _blocksCounted;
        private int _blocksNumber;

        public int BlocksCounted => _blocksCounted;
        public int BlocksNumber => _blocksNumber;
        public long FileLength => _fileStream.Length;

        public FileReader(String path, int blockSize)
        {
            _blocksCounted = 0;
            _fileStream = File.OpenRead(path);
            _blockSize = blockSize;
            _blocksNumber = (int)(_fileStream.Length % blockSize == 0
                ? _fileStream.Length / _blockSize
                : _fileStream.Length / _blockSize + 1);
        }

        ~FileReader()
        {
            _fileStream?.Dispose();
        }



        public List<Byte[]> GetNextBlocks(int blocksCountToRead)
        {
            var blocksList = new List<Byte[]>();
            var bufferSize = _fileStream.Length - _fileStream.Position < _blockSize * blocksCountToRead
                ? _fileStream.Length - _fileStream.Position
                : _blockSize * blocksCountToRead;

            if (bufferSize != 0)
            {
                var buffer = new Byte[bufferSize];
                var iterations = 0;
                _fileStream.Read(buffer, offset: 0, count: buffer.Length);

                for (var index = 0; index < blocksCountToRead; index++)
                {
                    if (index * _blockSize < bufferSize)
                    {
                        iterations++;
                    }
                    else
                    {
                        break;
                    }
                }

                for (var index = 0; index < iterations; index++)
                {
                    var length = (index + 1) * _blockSize < bufferSize
                        ? _blockSize
                        : bufferSize - index * _blockSize;

                    blocksList.Add(new Byte[length]);
                    Array.Copy(sourceArray: buffer,
                        sourceIndex: _blockSize * index,
                        destinationArray: blocksList[index],
                        destinationIndex: 0,
                        length: blocksList[index].Length);
                    _blocksCounted++;
                }
            }

            return blocksList;
        }

        public void Dispose()
        {
            _fileStream.Close();
            _fileStream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
