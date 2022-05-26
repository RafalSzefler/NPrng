using System;

namespace NPrng.Generators
{
    public sealed class SplitMix64 : AbstractPseudoRandomGenerator
    {
        internal UInt64 CurrentState { get; private set; }

        public SplitMix64(UInt64 currentState)
        {
            CurrentState = currentState;
        }

        /// <inheritdoc/>
        public override Int64 Generate()
        {
            unchecked
            {
                CurrentState += 0x9E3779B97f4A7C15;
                var result = CurrentState;
                result = (result ^ (result >> 30)) * 0xBF58476D1CE4E5B9;
                result = (result ^ (result >> 27)) * 0x94D049BB133111EB;
                result = result ^ (result >> 31);
                return (Int64)result;
            }
        }
    }
}
