using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NPrng
{
    internal static class SerializationHelpers
    {
        private static readonly ThreadLocal<byte[]> ReadBuffer
            = new ThreadLocal<byte[]>(() => new byte[sizeof(Int64)]);

        private static readonly ThreadLocal<byte[]> WriteBuffer
            = new ThreadLocal<byte[]>(() => new byte[sizeof(Int64)]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> WriteToBuffer(UInt64 value, ArraySegment<byte> buffer)
        {
            var array = buffer.Array;
            var offset = buffer.Offset;

            for (var i = sizeof(Int64) - 1; i >= 0; i--)
            {
                array[offset + i] = (byte)(value & 0xFF);
                value >>= 8;
            }

            return new ArraySegment<byte>(array, offset + sizeof(UInt64), buffer.Count - sizeof(UInt64));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteToStream(UInt64 value, Stream stream)
        {
            var buffer = WriteBuffer.Value;
            WriteToBuffer(value, new ArraySegment<byte>(buffer));
            stream.Write(buffer, 0, sizeof(UInt64));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> ReadFromBuffer(ArraySegment<byte> buffer, out UInt64 value)
        {
            var array = buffer.Array;
            var offset = buffer.Offset;

            value = 0;
            for (var i = 0; i < sizeof(UInt64); i++)
            {
                value <<= 8;
                value += array[offset + i];
            }

            return new ArraySegment<byte>(array, offset + sizeof(UInt64), buffer.Count - sizeof(UInt64));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadFromStream(Stream stream, out UInt64 value)
        {
            var buffer = ReadBuffer.Value;
            var missing = sizeof(UInt64);
            var offset = 0;
            while (missing > 0)
            {
                var justRead = stream.Read(buffer, offset, missing-offset);
                missing -= justRead;
                offset += justRead;
            }
            ReadFromBuffer(new ArraySegment<byte>(buffer), out value);
        }
    }
}
