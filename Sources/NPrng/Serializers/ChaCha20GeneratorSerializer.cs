using System;
using System.IO;
using NPrng.Generators;

namespace NPrng.Serializers
{
    public sealed class ChaCha20GeneratorSerializer : AbstractPseudoRandomGeneratorSerializer<ChaCha20Generator>
    {
        private const int BufferSize = 16 * sizeof(UInt32);
        public override int GetExpectedBufferSize(IPseudoRandomGenerator generator) => BufferSize;

        public override ChaCha20Generator ReadFromBuffer(ArraySegment<byte> buffer, out int read)
        {
            var key = new UInt32[16];
            for (var i = 0; i < 8; i++)
            {
                buffer = SerializationHelpers.ReadFromBuffer(buffer, out var value);
                key[2*i+1] = (UInt32)(value & 0xffffffff);
                key[2*i] = (UInt32)((value >> 32) & 0xffffffff);
            }
            read = BufferSize;
            return new ChaCha20Generator(key);
        }

        public override ChaCha20Generator ReadFromStream(Stream source)
        {
            var key = new UInt32[16];
            for (var i = 0; i < 8; i++)
            {
                SerializationHelpers.ReadFromStream(source, out var value);
                key[2*i+1] = (UInt32)(value & 0xffffffff);
                key[2*i] = (UInt32)((value >> 32) & 0xffffffff);
            }
            return new ChaCha20Generator(key);
        }

        public override int WriteToBuffer(ChaCha20Generator generator, ArraySegment<byte> buffer)
        {
            var key = generator.Key;
            for (var i = 0; i < 8; i++)
            {
                var no = ((UInt64)key[2*i] << 32) + ((UInt64)key[2*i+1]);
                buffer = SerializationHelpers.WriteToBuffer(no, buffer);
            }
            return BufferSize;
        }

        public override void WriteToStream(ChaCha20Generator generator, Stream target)
        {
            var key = generator.Key;
            for (var i = 0; i < 8; i++)
            {
                var no = ((UInt64)key[2*i] << 32) + ((UInt64)key[2*i+1]);
                SerializationHelpers.WriteToStream(no, target);
            }
        }
    }
}
