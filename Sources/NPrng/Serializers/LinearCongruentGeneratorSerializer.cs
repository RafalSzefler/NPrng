using System;
using System.IO;
using NPrng.Generators;

namespace NPrng.Serializers
{
    public sealed class LinearCongruentGeneratorSerializer : AbstractPseudoRandomGeneratorSerializer<LinearCongruentGenerator>
    {
        private const int BufferSize = sizeof(UInt64);

        /// <inheritdoc/>
        public override int GetExpectedBufferSize(LinearCongruentGenerator generator) => sizeof(UInt64);

        /// <inheritdoc/>
        public override LinearCongruentGenerator ReadFromBuffer(ArraySegment<byte> buffer, out int read)
        {
            UInt64 state;
            SerializationHelpers.ReadFromBuffer(buffer, out state);
            read = BufferSize;
            return new LinearCongruentGenerator(state);
        }

        /// <inheritdoc/>
        public override LinearCongruentGenerator ReadFromStream(Stream source)
        {
            UInt64 state;
            SerializationHelpers.ReadFromStream(source, out state);
            return new LinearCongruentGenerator(state);
        }

        /// <inheritdoc/>
        public override int WriteToBuffer(LinearCongruentGenerator generator, ArraySegment<byte> buffer)
        {
            SerializationHelpers.WriteToBuffer(generator.CurrentState, buffer);
            return BufferSize;
        }

        /// <inheritdoc/>
        public override void WriteToStream(LinearCongruentGenerator generator, Stream target)
        {
            SerializationHelpers.WriteToStream(generator.CurrentState, target);
        }
    }
}
