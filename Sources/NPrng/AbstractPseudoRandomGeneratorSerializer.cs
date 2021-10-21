using System;
using System.IO;

namespace NPrng
{
    public abstract class AbstractPseudoRandomGeneratorSerializer<TPseudoRandomGenerator> : IPseudoRandomGeneratorSerializer
        where TPseudoRandomGenerator : IPseudoRandomGenerator
    {
        public abstract void WriteToStream(TPseudoRandomGenerator generator, Stream target);
        public abstract int WriteToBuffer(TPseudoRandomGenerator generator, ArraySegment<byte> buffer);
        public abstract int GetExpectedBufferSize(IPseudoRandomGenerator generator);
        public abstract TPseudoRandomGenerator ReadFromStream(Stream source);
        public abstract TPseudoRandomGenerator ReadFromBuffer(ArraySegment<byte> buffer, out int read);

        void IPseudoRandomGeneratorSerializer.WriteToStream(IPseudoRandomGenerator generator, Stream target)
        {
            if (generator is TPseudoRandomGenerator tGenerator)
            {
                WriteToStream(tGenerator, target);
                return;
            }
            throw new ArgumentException($"{GetType()} can only serialize instances of {typeof(TPseudoRandomGenerator)}");
        }

        int IPseudoRandomGeneratorSerializer.WriteToBuffer(IPseudoRandomGenerator generator, ArraySegment<byte> buffer)
        {
            if (generator is TPseudoRandomGenerator tGenerator)
            {
                return WriteToBuffer(tGenerator, buffer);
            }
            throw new ArgumentException($"{GetType()} can only serialize instances of {typeof(TPseudoRandomGenerator)}");
        }

        IPseudoRandomGenerator IPseudoRandomGeneratorSerializer.ReadFromStream(Stream source)
            => this.ReadFromStream(source);

        IPseudoRandomGenerator IPseudoRandomGeneratorSerializer.ReadFromBuffer(ArraySegment<byte> buffer, out int read)
            => this.ReadFromBuffer(buffer, out read);
    }
}
