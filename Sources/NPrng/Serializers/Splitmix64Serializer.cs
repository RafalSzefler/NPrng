using System;
using System.IO;
using NPrng.Generators;

namespace NPrng.Serializers
{
    public sealed class SplitMix64Serializer : AbstractPseudoRandomGeneratorSerializer<SplitMix64>
    {
        private const int BufferSize = sizeof(UInt64);

        /// <inheritdoc/>
        public override int GetExpectedBufferSize(SplitMix64 generator) => BufferSize;

        /// <inheritdoc/>
        public override SplitMix64 ReadFromBuffer(ArraySegment<byte> buffer, out int read)
        {
            UInt64 state;
            SerializationHelpers.ReadFromBuffer(buffer, out state);
            read = BufferSize;
            return new SplitMix64(state);
        }

        /// <inheritdoc/>
        public override SplitMix64 ReadFromStream(Stream source)
        {
            UInt64 state;
            SerializationHelpers.ReadFromStream(source, out state);
            return new SplitMix64(state);
        }

        /// <inheritdoc/>
        public override int WriteToBuffer(SplitMix64 generator, ArraySegment<byte> buffer)
        {
            SerializationHelpers.WriteToBuffer(generator.CurrentState, buffer);
            return BufferSize;
        }

        /// <inheritdoc/>
        public override void WriteToStream(SplitMix64 generator, Stream target)
        {
            SerializationHelpers.WriteToStream(generator.CurrentState, target);
        }
    }
}
