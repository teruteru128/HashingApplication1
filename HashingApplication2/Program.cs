using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashingApplication2
{
    internal class Program
    {
        private const string Path0 = "D:\\keys\\public\\publicKeys0.bin";
        private const string Path1 = "D:\\keys\\public\\publicKeys1.bin";

        static void Main(string[] args)
        {
            byte[] bytes = new byte[25165824 * 65];
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Path0)))
            {
                int r = reader.Read(bytes, 0, 16777216 * 65);
                Console.WriteLine($"{Path0}: {r}バイト読み込みました");
            }
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Path1)))
            {
                int r = reader.Read(bytes, 16777216 * 65, 8388608 * 65);
                Console.WriteLine($"{Path1}: {r}バイト読み込みました");
            }
            using (SHA512 sha512 = SHA512.Create())
            using (RIPEMD160 ripe = RIPEMD160.Create())
            {
                int i = 0;
                int inputOffset;
                var randomNumberGenerator = RandomNumberGenerator.Create();
                while (true)
                {
                    i = GetInt32(randomNumberGenerator, 25165824);
                    inputOffset = i * 65;
                    for (int j = 0; j < 1635778560; j += 65)
                    {
                        sha512.TransformBlock(bytes, inputOffset, 65, null, 0);
                        sha512.TransformFinalBlock(bytes, j, 65);
                        if (BitConverter.ToUInt64(ripe.Hash, 0) < 0x10000UL)
                        {
                            Console.WriteLine($"{BitConverter.ToUInt64(ripe.Hash, 0):x016}, {i}, {j / 65}");
                            break;
                        }
                    }
                }
            }
        }
        private static int NextInt32(RandomNumberGenerator random, int bits)
        {
            byte[] bytes = new byte[4];
            random.GetBytes(bytes, 0, 4);
            return (int)(BitConverter.ToUInt32(bytes, 0) >> (32 - bits));
        }
        private static int GetInt32(RandomNumberGenerator random, int n)
        {
            int m = n - 1;
            if ((m & n) == 0)
            {
                ulong x = (ulong)n * (ulong)NextInt32(random, n);
                return (int)((long)x >> 31);
            }
            int bits, val;
            do
            {
                bits = NextInt32(random, 31);
                val = bits % n;
            } while (bits - val + m < 0);
            return val;
        }
    }
}
