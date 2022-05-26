using System;

namespace NPrng.Generators
{
    public sealed class LinearCongruentGenerator : AbstractPseudoRandomGenerator
    {
        /// <summary>
        /// Based on "Computationally Easy, Spectrally Good Multipliers for Congruential Pseudorandom Number Generators"
        /// paper by Guy Steele and Sebastiano Vigna
        /// </summary>
        private const UInt64 Multiplier = 0xfc0072fa0b15f4fd;

        /// <summary>
        /// Based on "The Art of Computer Programming, Volume 2: Seminumerical Algorithms" by Donald E. Knuth
        /// chapter §3.2.1.2, Theorem A.
        /// </summary>
        private const UInt64 LinearConstant = 34537;  // Prime number not dividing the Multiplier

        internal UInt64 CurrentState { get; private set; }

        public LinearCongruentGenerator(UInt64 seed)
        {
            if (seed == 0)
            {
                throw new ArgumentException($"{nameof(seed)} cannot be 0");
            }
            CurrentState = seed;
        }

        /// <inheritdoc/>
        public override Int64 Generate()
        {
            var currentState = CurrentState;
            unchecked
            {
                currentState = Multiplier * currentState + LinearConstant;
            }
            CurrentState = currentState;
            return (Int64)currentState;
        }
    }
}
