using System;
using System.IO;
using NPrng.Generators;
using NPrng.Serializers;
using Xunit;

namespace NPrng.Tests
{
    public sealed class LinearCongruentGeneratorSerializerTests
    {
        private readonly LinearCongruentGeneratorSerializer Serializer
            = new LinearCongruentGeneratorSerializer();

        [Theory]
        [InlineData(1, "AAAAAAAAAAE=", -288103957898298394, -1862732721773727433)]
        [InlineData(1234, "AAAAAAAABNI=", -5032146646061321613, 8252288942967176848)]
        [InlineData(2619692, "AAAAAAAn+Sw=", 4900101226711725925, -6829847203050550854)]
        public void TestSerialization(ulong seed, string expectedSerialization, long expected1, long expected2)
        {
            var generator = new LinearCongruentGenerator(seed);
            string result;
            using (var stream = new MemoryStream())
            {
                Serializer.WriteToStream(generator, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var data = stream.ToArray();
                result = Convert.ToBase64String(data);
            }

            Assert.Equal(expectedSerialization, result);
            Assert.Equal(expected1, generator.Generate());
            Assert.Equal(expected2, generator.Generate());
        }

        [Theory]
        [InlineData("AAAAAAAAAAE=", -288103957898298394, -1862732721773727433)]
        [InlineData("AAAAAAAABNI=", -5032146646061321613, 8252288942967176848)]
        [InlineData("AAAAAAAn+Sw=", 4900101226711725925, -6829847203050550854)]
        public void TestDeserialization(string data, long expected1, long expected2)
        {
            var binaryData = Convert.FromBase64String(data);
            var generator = Serializer.ReadFromBuffer(new ArraySegment<byte>(binaryData), out var count);
            Assert.Equal(8, count);
            Assert.NotNull(generator);
            Assert.Equal(expected1, generator.Generate());
            Assert.Equal(expected2, generator.Generate());
        }

        [Theory]
        [InlineData("AAAAAAAAAAE=", -288103957898298394, -1862732721773727433)]
        [InlineData("AAAAAAAABNI=", -5032146646061321613, 8252288942967176848)]
        [InlineData("AAAAAAAn+Sw=", 4900101226711725925, -6829847203050550854)]
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
            var generator = new LinearCongruentGenerator(98326);
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
