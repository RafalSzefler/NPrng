using System;
using System.Runtime.CompilerServices;

namespace NPrng
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] WriteToByteArray(this IPseudoRandomGeneratorSerializer serializer, IPseudoRandomGenerator generator)
        {
            var bufferSize = serializer.GetExpectedBufferSize(generator);
            var array = new byte[bufferSize];
            serializer.WriteToBuffer(generator, new ArraySegment<byte>(array));
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string WriteToString(this IPseudoRandomGeneratorSerializer serializer, IPseudoRandomGenerator generator)
        {
            var array = serializer.WriteToByteArray(generator);
            return Convert.ToBase64String(array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IPseudoRandomGenerator ReadFromByteArray(this IPseudoRandomGeneratorSerializer serializer, byte[] buffer)
            => serializer.ReadFromBuffer(new ArraySegment<byte>(buffer), out var _);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IPseudoRandomGenerator ReadFromString(this IPseudoRandomGeneratorSerializer serializer, string buffer)
            => serializer.ReadFromBuffer(new ArraySegment<byte>(Convert.FromBase64String(buffer)), out var _);
    }
}
