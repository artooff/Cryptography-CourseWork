using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork.ElGamal
{
    public static class SimplifyTests
    {
        public static bool MillerRabin(BigInteger value, double minProbability)
        {
            BigInteger d = value - 1;
            int degree = 0;
            if (value == 1)
                return false;
            while (d % 2 == 0)
            {
                d /= 2;
                degree += 1;
            }
            Random rnd = new Random();
            for (int i = 0; 1.0 - Math.Pow(4, -i) <= minProbability; i++)
            {
                BigInteger a = RandomInteger(2, value - 1);
                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1)
                    continue;
                
                for (int r = 1; r < degree; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);
                    if (x == 1)
                        return false;
                    if (x == value - 1)
                        break;
                }

                if (x != value - 1)
                    return false;
            }

            return true;
        }

        public static bool Fermat(BigInteger value, double minProbability)
        {
            Random rnd = new Random();
            if (value == 1)
                return false;
            for (int i = 0; 1.0 - Math.Pow(2, -i) <= minProbability; i++)
            {
                BigInteger a = RandomInteger(2, value - 1);
                if (QuickPow(a, value - 1, value) != 1) return false; // составное
            }

            return true;
        }

        public static bool MakeSimplicityTest(BigInteger value, double minProbability)
        {
            Random rnd = new Random();
            if (value == 1)
                return false;
            for (int i = 0; 1.0 - Math.Pow(2, -i) <= minProbability; i++)
            {
                BigInteger a = RandomInteger(2, value - 1);
                if (BigInteger.GreatestCommonDivisor(a, value) > 1)
                {
                    return false;
                }
                if (QuickPow(a, (value - 1) / 2, value) != Jacobi(a, value))
                {
                    return false; // составное
                }
            }

            return true;
        }

        public static BigInteger RandomInteger(BigInteger below, BigInteger above)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = above.ToByteArray();
            BigInteger R;
            do
            {
                rng.GetBytes(bytes);
                R = new BigInteger(bytes);
            } while (!(R >= below && R <= above));

            return R;
        }

        public static BigInteger QuickPow(BigInteger b, BigInteger degree, BigInteger mod)
        {
            BigInteger result = 1;
            BigInteger bitesForMask = degree;
            while (bitesForMask > 0)
            {
                if ((bitesForMask & 0b01) == 1)
                {
                    result = (result * b) % mod;
                }
                bitesForMask >>= 1;

                b *= b % mod;
            }
            return result;
        }

        public static BigInteger Jacobi(BigInteger a, BigInteger n)
        {
            int value;

            if (a == 1) return 1;

            value = (((n - 1) / 2) % 2 == 0 ? 1 : -1);
            if (a < 0) return Jacobi(-a, n) * value;

            value = (((n * n - 1) / 8) % 2 == 0 ? 1 : -1);
            if (a % 2 == 0) return Jacobi(a / 2, n) * value;

            value = (((a - 1) * (n - 1) / 4) % 2 == 0 ? 1 : -1);
            return value * Jacobi(n % a, a);
        }
    }
}
