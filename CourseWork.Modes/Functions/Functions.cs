using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork.Modes.Functions
{
    public class Functions
    {
        public static Byte[] Trim(Byte[] source, Int32 sourceStartIndex, Int32 destinationLength, Int32 destinationStartIndex)
        {
            var destination = new Byte[destinationLength];
            Array.Copy(sourceArray: source,
                sourceIndex: sourceStartIndex,
                destinationArray: destination,
                destinationIndex: destinationStartIndex,
                length: destinationLength);

            return destination;
        }
        public static Byte[] Xor(Byte[] a, Byte[] b)
        {
            Byte[] res = new Byte[a.Length];

            for (var i = 0; i < a.Length; i++)
            {
                res[i] = (Byte)(a[i] ^ b[i]);
            }
            return res;
        }

        public static Byte[] IncrementCounterByOne(Byte[] initCounterValue, Int32 blockSize)
        {
            Byte[] tmp = (Byte[])initCounterValue.Clone();
            for (var i = blockSize; i > 0; i--)
            {
                tmp[i - 1]++;
                if (tmp[i - 1] != 0)
                {
                    break;
                }
            }

            return tmp;
        }


        public static Byte[] GetDelta(Byte[] iv, Int32 blockSize)
        {
            var deltaArr = new Byte[blockSize];
            Array.Copy(iv, blockSize, deltaArr, 0, blockSize);
            return deltaArr;
        }

        public static Byte[] GetInitial(Byte[] iv, Int32 blockSize)
        {
            var initial = new Byte[blockSize];
            Array.Copy(iv, 0, initial, 0, blockSize);
            return initial;
        }

        public static List<byte[]> GetBlocksListFromArray(byte[] inputBuffer, int blockSize /*= algorhytm.blockSize*/)
        {
            var iterationsCount = inputBuffer.Length / blockSize;
            int inputBufferPosition = 0;

            var outputList = new List<byte[]>();

            for (int i = 0; i < iterationsCount; i++)
            {
                var block = new byte[blockSize];
                Array.Copy(inputBuffer, inputBufferPosition++ * blockSize, block, 0, blockSize);
                outputList.Add(block);


            }

            return outputList;
        }
    }
}
