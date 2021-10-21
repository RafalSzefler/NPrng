using System;
using System.IO;

namespace NPrng
{
    public interface IPseudoRandomGeneratorSerializer
    {
        int GetExpectedBufferSize(IPseudoRandomGenerator generator);
        void WriteToStream(IPseudoRandomGenerator generator, Stream target);
        int WriteToBuffer(IPseudoRandomGenerator generator, ArraySegment<byte> buffer);
        IPseudoRandomGenerator ReadFromStream(Stream source);
        IPseudoRandomGenerator ReadFromBuffer(ArraySegment<byte> buffer, out int read);
    }
}
