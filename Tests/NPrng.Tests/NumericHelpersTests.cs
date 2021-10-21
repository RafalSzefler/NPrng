using System;
using Xunit;

namespace NPrng.Tests
{
    public sealed class NumericHelpersTests
    {
        [Theory]
        [InlineData(0b0, 1)]
        [InlineData(0b1, 1)]
        [InlineData(0b10, 2)]
        [InlineData(0b11, 2)]
        [InlineData(0b100, 3)]
        [InlineData(0b101, 3)]
        [InlineData(0b110, 3)]
        [InlineData(0b111, 3)]
        [InlineData(0b1000, 4)]
        [InlineData(0b1001, 4)]
        [InlineData(0b1010, 4)]
        [InlineData(0b1100, 4)]
        [InlineData(0b110101100, 9)]
        [InlineData(1 << 14, 15)]
        [InlineData(1 << 17, 18)]
        public void TestBitCount(UInt64 value, int expected)
        {
            Assert.Equal(expected, NumericHelpers.CountBits(value));
        }
    }
}
