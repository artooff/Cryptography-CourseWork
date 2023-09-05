using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork.Modes.EncryptModes
{
    public class CipherModeCBC : CipherMode
    {
        private RC6.RC6 _algorhytm;
        private byte[] _iv;
        private int _blockSize;
        private byte[]? _lastProceseedBlock;
        private byte[]? _previousEncryptedBlock;

        public override byte[] IV
        {
            get => _iv;
            set => _iv = value;
        }

        public CipherModeCBC(RC6.RC6 algorhytm)
        {
            _algorhytm = algorhytm;
            _blockSize = _algorhytm.GetBlockSize();
            GenerateIV();
        }

        public override byte[] EncryptBlocks(List<byte[]> blocksList)
        {
            var blockSize = _algorhytm.GetBlockSize();
            var outputBuffer = new Byte[blocksList.Count * blockSize];

            var outputPosition = 0;

            foreach (var block in blocksList)
            {
                if (_lastProceseedBlock == null)
                    _lastProceseedBlock = _iv;
                _lastProceseedBlock = _algorhytm.Encrypt(Functions.Functions.Xor(_lastProceseedBlock, block));
                Array.Copy(_lastProceseedBlock, 0, outputBuffer, outputPosition++ * blockSize, blockSize);
            }

            return outputBuffer;
        }

        public override byte[] DecryptBlocks(List<byte[]> blocksList, byte[]? lastEncryptedBlock = null)
        {
            if (lastEncryptedBlock == null)
                _previousEncryptedBlock = _iv;
            else _previousEncryptedBlock = lastEncryptedBlock;
            blocksList.Insert(0, _previousEncryptedBlock);
            var outputBuffer = Enumerable.Repeat(default(Byte[]), blocksList.Count - 1).ToList();

            Parallel.For(0, outputBuffer.Count, index =>

                outputBuffer[index] = Functions.Functions.Xor(blocksList[index], _algorhytm.Decrypt(blocksList[index + 1]))
            );

            _previousEncryptedBlock = blocksList.Last();
            return outputBuffer.SelectMany(x => x).ToArray();

            //var outputPosition = 0;
            //foreach (var block in blocksList)
            //{
            //    if (_previousEncryptedBlock == null)
            //        _previousEncryptedBlock = _iv;
            //    var encryptedBlock = Functions.Functions.Xor(_previousEncryptedBlock, _algorhytm.Decrypt(block));
            //    _previousEncryptedBlock = block;
            //    Array.Copy(encryptedBlock, 0, outputBuffer, outputPosition++ * blockSize, blockSize);
            //}
        }

        protected override void GenerateIV()
        {
            _iv = new byte[_blockSize];
            Random rnd = new();
            rnd.NextBytes(_iv);
            Console.WriteLine(_iv);
        }

        public override void Reset()
        {
            _lastProceseedBlock = null;
            _previousEncryptedBlock = null;
        }
    }
}
