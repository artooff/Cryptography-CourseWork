using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork.Modes
{
    public class Padder
    {
        private int _blockSize;

        public Padder(int blockSize)
        {
            _blockSize = blockSize;
        }

        private Byte[] PadBuffer(Byte[] buf, Int32 padFrom, Int32 padTo)
        {
            if (padTo < buf.Length || padTo - padFrom > Byte.MaxValue)
            {
                return buf;
            }

            var b = new Byte[padTo];
            Buffer.BlockCopy(buf, 0, b, 0, padFrom);

            for (var count = padFrom; count < padTo; count++)
            {
                b[count] = (Byte)(padTo - padFrom);
            }
            return b;
        }

        public Byte[] PadBuffer(Byte[] buf)
        {
            var extraBlock = (buf.Length % _blockSize == 0) ? 0 : 1;
            return PadBuffer(buf, buf.Length, ((buf.Length / _blockSize) + extraBlock) * _blockSize);
        }

        public Byte[] RemovePadding(Byte[] blocks)
        {
            var extraBlocks = blocks[^1];
            var result = new Byte[blocks.Length - extraBlocks];
            Array.Copy(blocks, result, result.Length);

            return result;
        }
    }
}
