using System;
using System.IO;

namespace NPrng
{
    public abstract class AbstractPseudoRandomGeneratorSerializer<TPseudoRandomGenerator> : IPseudoRandomGeneratorSerializer
        where TPseudoRandomGenerator : IPseudoRandomGenerator
    {
        /// <summary>Returns the minimal size of the buffer needed to serialize the generator.</summary>
        public abstract void WriteToStream(TPseudoRandomGenerator generator, Stream target);

        /// <summary>Writes the generator to Stream.</summary>
        public abstract int WriteToBuffer(TPseudoRandomGenerator generator, ArraySegment<byte> buffer);

        /// <summary>Writes the generator to ArraySegment.</summary>
        public abstract int GetExpectedBufferSize(TPseudoRandomGenerator generator);

        /// <summary>Reads the generator from Stream.</summary>
        public abstract TPseudoRandomGenerator ReadFromStream(Stream source);

        /// <summary>Reads the generator from ArraySegment. The out int read argument indicates how many bytes were read.</summary>
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

        int IPseudoRandomGeneratorSerializer.GetExpectedBufferSize(IPseudoRandomGenerator generator)
        {
            if (generator is TPseudoRandomGenerator tGenerator)
            {
                return GetExpectedBufferSize(tGenerator);
            }
            throw new ArgumentException($"{GetType()} can only serialize instances of {typeof(TPseudoRandomGenerator)}");
        }
    }
}
