using System;
using System.Runtime.CompilerServices;

namespace NPrng
{
    public static class NumericHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountBits(UInt64 value)
        {
            var count = (int)Math.Log(value, 2);
            return 1 + count & int.MaxValue;
        }
    }
}
