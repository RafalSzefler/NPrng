using System;
using System.IO;

namespace NPrng
{
    public interface IPseudoRandomGeneratorSerializer
    {
        /// <summary>Returns the minimal size of the buffer needed to serialize the generator.</summary>
        int GetExpectedBufferSize(IPseudoRandomGenerator generator);

        /// <summary>Writes the generator to Stream.</summary>
        void WriteToStream(IPseudoRandomGenerator generator, Stream target);

        /// <summary>Writes the generator to ArraySegment.</summary>
        int WriteToBuffer(IPseudoRandomGenerator generator, ArraySegment<byte> buffer);

        /// <summary>Reads the generator from Stream.</summary>
        IPseudoRandomGenerator ReadFromStream(Stream source);

        /// <summary>Reads the generator from ArraySegment. The out int read argument indicates how many bytes were read.</summary>
        IPseudoRandomGenerator ReadFromBuffer(ArraySegment<byte> buffer, out int read);
    }
}
