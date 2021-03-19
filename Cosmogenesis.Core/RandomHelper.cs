using System;
using System.Security.Cryptography;
using System.Threading;

namespace Cosmogenesis.Core
{
    public static class RandomHelper
    {
        static readonly ThreadLocal<RandomNumberGenerator> Randoms = new(RandomNumberGenerator.Create);

        public static byte[] GetRandomBytes(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var buf = new byte[count];
            Randoms.Value.GetBytes(buf);
            return buf;
        }

        public static void GetRandomBytes(byte[] buf) => GetRandomBytes((Span<byte>)(buf ?? throw new ArgumentNullException(nameof(buf))));

        public static void GetRandomBytes(Span<byte> buf) => Randoms.Value.GetBytes(buf);

        public static long GetRandomInt64()
        {
            Span<byte> buf = stackalloc byte[8];
            GetRandomBytes(buf);
            return BitConverter.ToInt64(buf);
        }

        public static int GetRandomInt32()
        {
            Span<byte> buf = stackalloc byte[4];
            GetRandomBytes(buf);
            return BitConverter.ToInt32(buf);
        }

        public static long GetRandomPositiveInt64()
        {
            var num = GetRandomInt64();
            if (num > 0)
            {
                return num;
            }
            num = -num;
            if (num > 0)
            {
                return num;
            }
            return GetRandomPositiveInt64();
        }

        public static int GetRandomPositiveInt32()
        {
            var num = GetRandomInt32();
            if (num > 0)
            {
                return num;
            }
            num = -num;
            if (num > 0)
            {
                return num;
            }
            return GetRandomPositiveInt32();
        }
    }
}
