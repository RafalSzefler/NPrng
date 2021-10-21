using System;
using System.IO;
using NPrng.Generators;
using NPrng.Serializers;
using Xunit;

namespace NPrng.Tests
{
    public sealed class SplittableRandomSerializerTests
    {
        private readonly SplittableRandomSerializer Serializer
            = new SplittableRandomSerializer();

        [Theory]
        [InlineData(1, "AAAAAAAAAAGeN3m5f0p8FQ==", -1874130600990937387, -1706819287460639110)]
        [InlineData(1234, "AAAAAAAABNKeN3m5f0p8FQ==", -460151231511528161, -8217047239040255225)]
        public void TestSerialization(ulong seed, string expectedSerialization, long expected1, long expected2)
        {
            var generator = new SplittableRandom(seed);
            string result;
            using (var stream = new MemoryStream())
            {
                Serializer.WriteToStream(generator, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var data = stream.ToArray();
                result = Convert.ToBase64String(data);
            }

            Assert.Equal(expectedSerialization, result);
            var result1 = generator.Generate();
            var result2 = generator.Generate();
            Assert.Equal(expected1, result1);
            Assert.Equal(expected2, result2);
        }

        [Theory]
        [InlineData("AAAAAAAAAAGeN3m5f0p8FQ==", -1874130600990937387, -1706819287460639110)]
        [InlineData("AAAAAAAABNKeN3m5f0p8FQ==", -460151231511528161, -8217047239040255225)]
        public void TestDeserialization(string data, long expected1, long expected2)
        {
            var binaryData = Convert.FromBase64String(data);
            var generator = Serializer.ReadFromBuffer(new ArraySegment<byte>(binaryData), out var count);
            Assert.Equal(16, count);
            Assert.NotNull(generator);
            Assert.Equal(expected1, generator.Generate());
            Assert.Equal(expected2, generator.Generate());
        }

        [Theory]
        [InlineData("AAAAAAAAAAGeN3m5f0p8FQ==", -1874130600990937387, -1706819287460639110)]
        [InlineData("AAAAAAAABNKeN3m5f0p8FQ==", -460151231511528161, -8217047239040255225)]
        public void TestDeserializationStream(string data, long expected1, long expected2)
        {
            var binaryData = Convert.FromBase64String(data);
            IPseudoRandomGenerator generator;
            using (var stream = new MemoryStream(binaryData))
            {
                generator = Serializer.ReadFromStream(stream);                
            }
            Assert.NotNull(generator);
            Assert.Equal(expected1, generator.Generate());
            Assert.Equal(expected2, generator.Generate());
        }

        [Fact]
        public void TestSerializationAfterGeneration()
        {
            var generator = new SplittableRandom(98326);
            generator.Generate();
            generator.Generate();
            var serialized = Serializer.WriteToString(generator);
            Assert.NotNull(serialized);
            var newGenerator = Serializer.ReadFromString(serialized);
            Assert.False(object.ReferenceEquals(generator, newGenerator));

            for (var i = 0; i < 100; i++)
            {
                Assert.Equal(generator.Generate(), newGenerator.Generate());
            }
        }
    }
}
