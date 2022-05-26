using System;
using System.Runtime.CompilerServices;

namespace NPrng
{
    public abstract class AbstractPseudoRandomGenerator : IPseudoRandomGenerator
    {
        /// <inheritdoc/>
        public abstract Int64 Generate();

        /// <inheritdoc/>
        public double GenerateDouble()
        {
            const Int64 absMask = (Int64)(~((1UL << 63) + (1UL << 62)));
            var generated = Generate() & absMask;
            return (double)generated / (double)(absMask+1);
        }

        /// <inheritdoc/>
        public virtual Int64 GenerateInRange(Int64 lower, Int64 upper)
        {
            if (lower > upper)
            {
                throw new ArgumentException($"{nameof(lower)} cannot be greater than {nameof(upper)}.");
            }
            else if (lower == upper)
            {
                return lower;
            }

            return lower + InternalGenerateInRange(upper - lower);
        }

        /// <inheritdoc/>
        public virtual Int64 GenerateLessOrEqualTo(Int64 range)
        {
            if (range < 0)
            {
                throw new ArgumentException($"{nameof(range)} has to be nonnegative.");
            }
            else if (range == 0)
            {
                return 0;
            }

            return InternalGenerateInRange(range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Int64 InternalGenerateInRange(Int64 range)
        {
            var uRange = (UInt64)range;
            var bitCount = NumericHelpers.CountBits(uRange);
            var mask = (UInt64)((1 << bitCount) - 1);

            UInt64 result;
            do
            {
                result = (UInt64)Generate() & mask;
            }
            while (result > uRange);

            return (Int64)result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int64 FastAbs(Int64 value)
        {
            var shifted = value >> 63;
            return (value + shifted) ^ shifted;
        }
    }
}
