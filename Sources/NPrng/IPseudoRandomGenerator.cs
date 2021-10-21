using System;

namespace NPrng
{
    public interface IPseudoRandomGenerator
    {
        Int64 Generate();
        Int64 GenerateLessOrEqualTo(Int64 range);
        Int64 GenerateInRange(Int64 lower, Int64 upper);
        double GenerateDouble();
    }
}
