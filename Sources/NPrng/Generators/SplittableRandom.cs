using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NPrng.Generators
{
    public sealed class SplittableRandom : AbstractPseudoRandomGenerator
    {
        private static Int64 DefaultGen = InitialSeed();
        private const UInt64 GOLDEN_GAMMA = 0x9e3779b97f4a7c15L;

        private static UInt64 GetAndAdd(UInt64 value)
        {
            while (true)
            {
                var defaultGen = Volatile.Read(ref DefaultGen);
                var newValue = unchecked(defaultGen + (Int64)value);
                if (Interlocked.CompareExchange(ref DefaultGen, newValue, defaultGen) == defaultGen)
                {
                    return (UInt64)newValue;
                }
            }
        }

        private static Int64 InitialSeed() => DateTime.UtcNow.ToBinary();
        internal UInt64 Seed { get; private set; }
        internal UInt64 Gamma { get; }

        internal SplittableRandom(UInt64 seed, UInt64 gamma)
        {
            Seed = seed;
            Gamma = gamma;
        }

        public SplittableRandom(UInt64 seed)
            : this(seed, GOLDEN_GAMMA)
        { }

        public SplittableRandom()
        {
            var s = GetAndAdd(unchecked(2 * GOLDEN_GAMMA));
            this.Seed = mix64(s);
            this.Gamma = mixGamma(s + GOLDEN_GAMMA);
        }

        public override Int64 Generate() => (Int64)mix64(nextSeed());

        public SplittableRandom Split() => new SplittableRandom(
            mix64(nextSeed()),
            mixGamma(nextSeed()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt64 mix64(UInt64 z)
        {
            unchecked
            {
                z = (z ^ (z >> 33)) * 0xff51afd7ed558ccdL;
                z = (z ^ (z >> 33)) * 0xc4ceb9fe1a85ec53L;
                return z ^ (z >> 33);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UInt64 nextSeed()
        {
            unchecked
            {
                Seed += Gamma;
                return Seed;
            }
        }

        private static UInt64 mixGamma(UInt64 z)
        {
            unchecked
            {
                z = mix64variant13(z) | 1L;
                var n = NumericHelpers.CountBits(z ^ (z >> 1));
                if (n >= 24) z ^= 0xaaaaaaaaaaaaaaaaL;
            }
            return z;
        }

        private static UInt64 mix64variant13(UInt64 z)
        {
            unchecked
            {
                z = (z ^ (z >> 30)) * 0xbf58476d1ce4e5b9L;
                z = (z ^ (z >> 27)) * 0x94d049bb133111ebL;
                return z ^ (z >> 31);
            }
        }
    }
}
