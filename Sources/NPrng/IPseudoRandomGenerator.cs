using System;

namespace NPrng
{
    public interface IPseudoRandomGenerator
    {
        /// <summary>Generates a pseudo random 64-bit integer.</summary>
        Int64 Generate();

        /// <summary>Generates a pseudo random non-negative 64-bit integer up to range.</summary>
        Int64 GenerateLessOrEqualTo(Int64 range);

        /// <summary>Generates a pseudo random 64-bit integer in [lower, upper] range.</summary>
        Int64 GenerateInRange(Int64 lower, Int64 upper);

        /// <summary>Generates a pseudo random 64-bit double in [0, 1) range.</summary>
        double GenerateDouble();
    }
}
