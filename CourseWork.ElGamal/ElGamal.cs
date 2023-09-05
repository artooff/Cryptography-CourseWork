using System.Numerics;

namespace CourseWork.ElGamal
{
    public enum SimplifyTestModes
    {
        Fermat,
        MillerRabin,
        SolovayStrassen
    }
    public struct PublicKey
    {
        public BigInteger P;
        public BigInteger G;
        public BigInteger Y;

        public PublicKey(BigInteger p, BigInteger g, BigInteger y)
        {
            P = p;
            G = g;
            Y = y;
        }

        public override string ToString()
        {
            return new string(String.Format("{0}\n{1}\n{2}", P.ToString(), G.ToString(), Y.ToString()));
        }
    }

    public struct EncryptedMessage
    {
        public BigInteger A;
        public BigInteger B;
        public EncryptedMessage(BigInteger a, BigInteger b)
        {
            this.A = a;
            this.B = b;
        }

        public override string ToString()
        {
            return new string(String.Format("{0}\n{1}", A.ToString(), B.ToString()));
        }
    }

    public class ElGamal
    {
        private PublicKey _publicKey = new PublicKey();
        private BigInteger _privateKey;
        private SimplifyTestModes _testMode = SimplifyTestModes.MillerRabin;

        public PublicKey PublicKey
        {
            get => _publicKey;
            set => _publicKey = new PublicKey(value.P, value.G, value.Y);
        }

        public void KenerateGeys(int byteSize)
        {
            Random rnd = new Random();
            _publicKey.P = GetPrimeNumber(byteSize);
            byte[] gBytes = new byte[rnd.Next(byteSize / 2, byteSize)];
            rnd.NextBytes(gBytes);
            _publicKey.G = new BigInteger(gBytes);
            _privateKey = GetPrimeNumber(byteSize - 1);
            _publicKey.Y = BigInteger.ModPow(_publicKey.G, _privateKey, _publicKey.P);
        }

        private BigInteger GetPrimeNumber(int size)
        {
            BigInteger newBigInt;
            var random = new Random();
            byte[] buffer = new byte[size];
            while (true)
            {
                do
                {
                    random.NextBytes(buffer);
                    newBigInt = new BigInteger(buffer);
                } while (newBigInt < 2);

                switch (_testMode)
                {
                    case SimplifyTestModes.Fermat:
                        {
                            if (SimplifyTests.Fermat(newBigInt, 0.7)) return newBigInt;
                            break;
                        }
                    case SimplifyTestModes.MillerRabin:
                        {
                            if (SimplifyTests.MillerRabin(newBigInt, 0.7)) return newBigInt;
                            break;
                        }
                    case SimplifyTestModes.SolovayStrassen:
                        {

                            if (SimplifyTests.Fermat(newBigInt, 0.7)) return newBigInt;
                            break;
                        }
                }
            }
        }

        public EncryptedMessage Encrypt(byte[] data)
        {
            Random rnd = new Random();
            BigInteger message = new BigInteger(data);
            BigInteger k = GetPrimeNumber(data.Length - 1);
            EncryptedMessage res = new EncryptedMessage();
            res.A = BigInteger.ModPow(_publicKey.G, k, _publicKey.P);
            res.B = BigInteger.ModPow(_publicKey.Y, k, _publicKey.P) * message % _publicKey.P;
            return res;
        }

        public byte[] Decrypt(EncryptedMessage data)
        {
            BigInteger res = data.B * BigInteger.ModPow(data.A, _publicKey.P - 1 - _privateKey, _publicKey.P) % _publicKey.P;
            return res.ToByteArray();
        }
    }
}