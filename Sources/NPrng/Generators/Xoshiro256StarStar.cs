using System;
using System.Runtime.CompilerServices;

namespace NPrng.Generators
{
    public sealed class Xoshiro256StarStar : AbstractPseudoRandomGenerator
    {
        internal UInt64 S0 { get; private set; }
        internal UInt64 S1 { get; private set; }
        internal UInt64 S2 { get; private set; }
        internal UInt64 S3 { get; private set; }

        internal Xoshiro256StarStar(UInt64 s0, UInt64 s1, UInt64 s2, UInt64 s3)
        {
            S0 = s0;
            S1 = s1;
            S2 = s2;
            S3 = s3;
        }

        public Xoshiro256StarStar(IPseudoRandomGenerator seeder)
        {
            S0 = (UInt64)seeder.Generate();
            S1 = (UInt64)seeder.Generate();
            S2 = (UInt64)seeder.Generate();
            S3 = (UInt64)seeder.Generate();
        }

        public Xoshiro256StarStar(UInt64 seed)
            : this(new LinearCongruentGenerator(seed))
        { }

        /// <inheritdoc/>
        public override Int64 Generate()
        {
            unchecked
            {
                var result = rol64(S1 * 5, 7) * 9;
                var t = S1 << 17;
                S2 ^= S0;
                S3 ^= S1;
                S1 ^= S2;
                S0 ^= S3;
                S2 ^= t;
                S3 ^= rol64(S3, 45);
                return (Int64)result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt64 rol64(UInt64 x, int k) => unchecked((x << k) | (x >> (64 - k)));
    }
}
