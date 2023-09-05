using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork.Modes.EncryptModes
{
    public enum EncryptionMode
    {
        ECB,
        CBC,
        CFB,
        OFB,
        CTR,
        RD,
        RDH
    }

    public class ModeFactory
    {
        public CipherMode CreateCipherMode(EncryptionMode mode, RC6.RC6 algorhytm)
        {
            return mode switch
            {
                EncryptionMode.CBC => new CipherModeCBC(algorhytm)
            };
        }
    }

}
