//using CourseWork.Modes.EncryptModes;
//using CourseWork.Modes.FilesOperating;
//using CourseWork.RC6;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CourseWork.Modes
//{
//    public class Cipher
//    {
//        private RC6.RC6 _algorhytm;
//        private CipherMode _cipherMode;
//        private Padder _padder;
//        private int _blockSize;
//        private int _processorCount;

//        public Action<byte[], bool>? BlocksSetEncrypted;
        
//        public byte[] IV
//        {
//            get => _cipherMode.IV;
//            set => _cipherMode.IV = value;
//        }

//        public byte[] Key
//        {
//            get => _algorhytm.MainKey;
//            set { _algorhytm.MainKey = value; }
//        }

//        public void GenerateKey(int size, byte[] key = null)
//        {
//            _algorhytm.GenerateKey(128, key);
//        }

//        public Cipher(EncryptionMode encryptionMode)
//        {
//            _algorhytm = new RC6.RC6();
//            var factory = new ModeFactory();
//            _cipherMode = factory.CreateCipherMode(encryptionMode, _algorhytm);
//            _blockSize = _algorhytm.GetBlockSize();
//            _padder = new Padder(_blockSize);
//            _processorCount = Environment.ProcessorCount;
//        }

//        public void Encrypt(string filePath)
//        {
//            var fileReader = new FileReader(filePath, _blockSize);
//            var iterationsCount = fileReader.BlocksNumber % _processorCount == 0
//                ? fileReader.BlocksNumber / _processorCount
//                : fileReader.BlocksNumber / _processorCount + 1;

//            var outputBuffer = new byte[iterationsCount][];
//            var isLastBlock = false;
//            for(int i = 0; i < iterationsCount; i++)
//            {
//                var readedBlocks = fileReader.GetNextBlocks(_processorCount);
//                if(i == iterationsCount - 1)
//                {
//                    if(readedBlocks.Last().Length == _blockSize)
//                    {
//                        readedBlocks.Add(Enumerable.Repeat((Byte)_blockSize, _blockSize).ToArray());
//                    }
//                    else
//                    {
//                        readedBlocks[readedBlocks.Count - 1] = _padder.PadBuffer(readedBlocks.Last());
//                    }
//                    isLastBlock = true;
//                }
//                var encryptedBlocks = _cipherMode.EncryptBlocks(readedBlocks);
//                BlocksSetEncrypted?.Invoke(encryptedBlocks, isLastBlock);
//                //outputBuffer[i] = encryptedBlocks;
//            }
//            _cipherMode.Reset();
//            //return outputBuffer.SelectMany(x => x).ToArray();
//        }

//        public byte[] Decrypt(byte[] inputBuffer)
//        {
//            //первый блок наебывается
//            //разбить массив на processorCount блоков по 16 байт
//            var inputBlocks = GetBlocksList(inputBuffer);
//            var iterationsCount = inputBlocks.Count % _processorCount == 0 
//                ? inputBlocks.Count / _processorCount
//                : inputBlocks.Count / _processorCount + 1;
//            var outputBuffer = new byte[iterationsCount][];

//            for (int i = 0; i < iterationsCount; i++)
//            {
//                var decryptedBlocks = _cipherMode.DecryptBlocks(inputBlocks.GetRange(
//                    i * _processorCount,  Math.Min(_processorCount ,inputBlocks.Count - i *_processorCount)));
//                outputBuffer[i] = decryptedBlocks;

//                if (i == outputBuffer.Length - 1)
//                {
//                    outputBuffer[outputBuffer.Length - 1] = _padder.RemovePadding(outputBuffer.Last());
//                }
//            }

//            return outputBuffer.SelectMany(x => x).ToArray();
//        }

//        private List<byte[]> GetBlocksList(byte[] inputBuffer, int blockSize /*= algorhytm.blockSize*/)
//        {
//            var stepSize = _algorhytm.GetBlockSize();
//            var iterationsCount = inputBuffer.Length / stepSize;
//            int inputBufferPosition = 0;

//            var outputList = new List<byte[]>();

//            for(int i = 0; i < iterationsCount; i++)
//            {
//                var block = new byte[stepSize];
//                Array.Copy(inputBuffer, inputBufferPosition++ * stepSize, block, 0, stepSize);
//                outputList.Add(block);

                
//            }

//            return outputList;
//        }

//    }
//}
