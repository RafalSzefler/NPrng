using System;
using System.IO;
using NPrng.Generators;

namespace NPrng.Serializers
{
    public sealed class Xoshiro256StarStarSerializer : AbstractPseudoRandomGeneratorSerializer<Xoshiro256StarStar>
    {
        private const int BufferSize = 4 * sizeof(UInt64);
        public override int GetExpectedBufferSize(IPseudoRandomGenerator generator) => BufferSize;

        public override Xoshiro256StarStar ReadFromBuffer(ArraySegment<byte> buffer, out int read)
        {
            UInt64 s0;
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out s0);
            UInt64 s1;
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out s1);
            UInt64 s2;
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out s2);
            UInt64 s3;
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out s3);
            read = BufferSize;
            return new Xoshiro256StarStar(s0, s1, s2, s3);
        }

        public override Xoshiro256StarStar ReadFromStream(Stream source)
        {
            UInt64 s0;
            SerializationHelpers.ReadFromStream(source, out s0);
            UInt64 s1;
            SerializationHelpers.ReadFromStream(source, out s1);
            UInt64 s2;
            SerializationHelpers.ReadFromStream(source, out s2);
            UInt64 s3;
            SerializationHelpers.ReadFromStream(source, out s3);
            return new Xoshiro256StarStar(s0, s1, s2, s3);
        }

        public override int WriteToBuffer(Xoshiro256StarStar generator, ArraySegment<byte> buffer)
        {
            buffer = SerializationHelpers.WriteToBuffer(generator.S0, buffer);
            buffer = SerializationHelpers.WriteToBuffer(generator.S1, buffer);
            buffer = SerializationHelpers.WriteToBuffer(generator.S2, buffer);
            buffer = SerializationHelpers.WriteToBuffer(generator.S3, buffer);
            return BufferSize;
        }

        public override void WriteToStream(Xoshiro256StarStar generator, Stream target)
        {
            SerializationHelpers.WriteToStream(generator.S0, target);
            SerializationHelpers.WriteToStream(generator.S1, target);
            SerializationHelpers.WriteToStream(generator.S2, target);
            SerializationHelpers.WriteToStream(generator.S3, target);
        }
    }
}
