using System;
using System.IO;
using NPrng.Generators;

namespace NPrng.Serializers
{
    public sealed class SplittableRandomSerializer : AbstractPseudoRandomGeneratorSerializer<SplittableRandom>
    {
        private const int BufferSize = 2 * sizeof(UInt64);
        public override int GetExpectedBufferSize(IPseudoRandomGenerator generator) => BufferSize;

        public override SplittableRandom ReadFromBuffer(ArraySegment<byte> buffer, out int read)
        {
            UInt64 seed;
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out seed);
            UInt64 gamma;
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out gamma);
            read = BufferSize;
            return new SplittableRandom(seed, gamma);
        }

        public override SplittableRandom ReadFromStream(Stream source)
        {
            UInt64 seed;
            SerializationHelpers.ReadFromStream(source, out seed);
            UInt64 gamma;
            SerializationHelpers.ReadFromStream(source, out gamma);
            return new SplittableRandom(seed, gamma);
        }

        public override int WriteToBuffer(SplittableRandom generator, ArraySegment<byte> buffer)
        {
            buffer = SerializationHelpers.WriteToBuffer(generator.Seed, buffer);
            buffer = SerializationHelpers.WriteToBuffer(generator.Gamma, buffer);
            return BufferSize;
        }

        public override void WriteToStream(SplittableRandom generator, Stream target)
        {
            SerializationHelpers.WriteToStream(generator.Seed, target);
            SerializationHelpers.WriteToStream(generator.Gamma, target);
        }
    }
}
