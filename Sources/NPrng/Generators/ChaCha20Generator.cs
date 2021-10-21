using System;
using System.Runtime.CompilerServices;

namespace NPrng.Generators
{
    public sealed class ChaCha20Generator : AbstractPseudoRandomGenerator
    {
        private const int Rounds = 20;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt32 ROTL(UInt32 a, int b) => unchecked((a << b) | (a >> (32 - b)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void QR(ref UInt32 a, ref UInt32 b, ref UInt32 c, ref UInt32 d)
        {
            unchecked
            {
                a += b; 
                d ^= a;
                d = ROTL(d, 16);
                c += d;
                b ^= c;
                b = ROTL(b,12);
                a += b;
                d ^= a;
                d = ROTL(d, 8);
                c += d;
                b ^= c;
                b = ROTL(b, 7);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ChaChaBlock(UInt32[] input, out UInt32[] output)
        {
            const int blockSize = 16;

            unchecked
            {
                var x = new UInt32[blockSize];
                Array.Copy(input, 0, x, 0, blockSize);

                for (var i = 0; i < (Rounds/2); i++)
                {
                    // Odd round
                    QR(ref x[0], ref x[4], ref x[ 8], ref x[12]);
                    QR(ref x[1], ref x[5], ref x[ 9], ref x[13]);
                    QR(ref x[2], ref x[6], ref x[10], ref x[14]);
                    QR(ref x[3], ref x[7], ref x[11], ref x[15]);

                    // Even round
                    QR(ref x[0], ref x[5], ref x[10], ref x[15]);
                    QR(ref x[1], ref x[6], ref x[11], ref x[12]);
                    QR(ref x[2], ref x[7], ref x[ 8], ref x[13]);
                    QR(ref x[3], ref x[4], ref x[ 9], ref x[14]);
                }

                for (var i = 0; i < blockSize; i++)
                {
                    x[i] += input[i];
                }
            
                output = x;
            }
        }

        internal UInt32[] Key { get; private set; }
        private UInt32[] Cache;

        internal ChaCha20Generator(UInt32[] key)
        {
            Key = key;
            Cache = Array.Empty<UInt32>();
            
            // Key[12] serves two purposes: as a ChaCha block counter
            // and as a pointer to Cache.
            var counter = Key[12];
            if (counter % 8 != 0)
            {
                Key[12] = counter / 8;
                ChaChaBlock(Key, out Cache);
                Key[12] = counter;
            }
        }

        public ChaCha20Generator(UInt64 seed)
        {
            Key = GenerateInitialKey(seed);
            Cache = Array.Empty<UInt32>();
        }

        private static UInt32[] GenerateInitialKey(UInt64 seed)
        {
            unchecked
            {
                if (seed == 0)
                {
                    seed = 0x2654633dc37cc394;
                }
                var key = new UInt32[16];

                // Fill constants
                key[0] = 0x61707865;
                key[1] = 0x3320646e;
                key[2] = 0x79622d32;
                key[3] = 0x6b206574;

                // Fill key
                for (var i = 0; i < 4; i++)
                {
                    key[4+2*i] = (UInt32)(seed & 0xffffffff);
                    key[5+2*i] = (UInt32)((seed >> 32) & 0xffffffff);
                }

                // Initialize block/cache counter to 0
                key[12] = 0;

                // Fill nonce
                FillNonce(new ArraySegment<UInt32>(key, 13, 3), seed);
                return key;
            }
        }

        private static void FillNonce(ArraySegment<uint> arraySegment, UInt64 seed)
        {
            unchecked
            {
                var rng = new LinearCongruentGenerator(seed);
                var array = arraySegment.Array;
                var count = arraySegment.Offset + arraySegment.Count;
                for (var i = arraySegment.Offset; i < count; i++)
                {
                    array[i] = (UInt32)rng.Generate();
                }
            }
        }

        public override Int64 Generate()
        {
            unchecked
            {
                var mod = Key[12] % 8;
                if (mod == 0)
                {
                    Key[12] /= 8;
                    ChaChaBlock(Key, out Cache);
                    Key[12] *= 8;
                }
                Key[12]++;

                var first = Cache[2*mod];
                var second = Cache[2*mod + 1];
                var result = ((UInt64)first << 32) + (UInt64)second;
                if (Key[12] >= (1 << 16) + 7)
                {
                    Key = GenerateInitialKey(result);
                    Cache = Array.Empty<UInt32>();
                }
                return (Int64)result;
            }
        }
    }
}
