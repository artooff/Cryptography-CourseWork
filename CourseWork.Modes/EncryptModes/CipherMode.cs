using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork.Modes.EncryptModes
{
    public abstract class CipherMode
    {
        public abstract byte[] IV { get; set; }

        public abstract byte[] EncryptBlocks(List<byte[]> blocksList);
        public abstract byte[] DecryptBlocks(List<byte[]> blocksList, byte[]? lastDecryptedBlock = null);
        public abstract void Reset();
        protected abstract void GenerateIV();
        
    }
}
