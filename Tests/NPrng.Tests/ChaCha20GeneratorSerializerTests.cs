using System;
using System.IO;
using NPrng.Generators;
using NPrng.Serializers;
using Xunit;

namespace NPrng.Tests
{
    public sealed class ChaCha20GeneratorSerializerTests
    {
        private readonly ChaCha20GeneratorSerializer Serializer
            = new ChaCha20GeneratorSerializer();

        [Theory]
        [InlineData(1, "YXB4ZTMgZG55Yi0yayBldAAAAAEAAAAAAAAAAQAAAAAAAAABAAAAAAAAAAEAAAAAAAAAAAsWe+ZvLjE3ThKWRA==", -8183708576023727317, -6446727672260194149)]
        [InlineData(1234, "YXB4ZTMgZG55Yi0yayBldAAABNIAAAAAAAAE0gAAAAAAAATSAAAAAAAABNIAAAAAAAAAAG/XcnM1cT6QElybOQ==", -5940430971784544814, -4986938435889292935)]
        [InlineData(2619692, "YXB4ZTMgZG55Yi0yayBldAAn+SwAAAAAACf5LAAAAAAAJ/ksAAAAAAAn+SwAAAAAAAAAAJWbt2Wn+gm6HaNruw==", 3986920306532631898, 6791565248134816761)]
        public void TestSerialization(ulong seed, string expectedSerialization, long expected1, long expected2)
        {
            var generator = new ChaCha20Generator(seed);
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
        [InlineData("YXB4ZTMgZG55Yi0yayBldAAAAAEAAAAAAAAAAQAAAAAAAAABAAAAAAAAAAEAAAAAAAAAAAsWe+ZvLjE3ThKWRA==", -8183708576023727317, -6446727672260194149)]
        [InlineData("YXB4ZTMgZG55Yi0yayBldAAABNIAAAAAAAAE0gAAAAAAAATSAAAAAAAABNIAAAAAAAAAAG/XcnM1cT6QElybOQ==", -5940430971784544814, -4986938435889292935)]
        [InlineData("YXB4ZTMgZG55Yi0yayBldAAn+SwAAAAAACf5LAAAAAAAJ/ksAAAAAAAn+SwAAAAAAAAAAJWbt2Wn+gm6HaNruw==", 3986920306532631898, 6791565248134816761)]
        public void TestDeserialization(string data, long expected1, long expected2)
        {
            var binaryData = Convert.FromBase64String(data);
            var generator = Serializer.ReadFromBuffer(new ArraySegment<byte>(binaryData), out var count);
            Assert.Equal(64, count);
            Assert.NotNull(generator);
            Assert.Equal(expected1, generator.Generate());
            Assert.Equal(expected2, generator.Generate());
        }

        [Theory]
        [InlineData("YXB4ZTMgZG55Yi0yayBldAAAAAEAAAAAAAAAAQAAAAAAAAABAAAAAAAAAAEAAAAAAAAAAAsWe+ZvLjE3ThKWRA==", -8183708576023727317, -6446727672260194149)]
        [InlineData("YXB4ZTMgZG55Yi0yayBldAAABNIAAAAAAAAE0gAAAAAAAATSAAAAAAAABNIAAAAAAAAAAG/XcnM1cT6QElybOQ==", -5940430971784544814, -4986938435889292935)]
        [InlineData("YXB4ZTMgZG55Yi0yayBldAAn+SwAAAAAACf5LAAAAAAAJ/ksAAAAAAAn+SwAAAAAAAAAAJWbt2Wn+gm6HaNruw==", 3986920306532631898, 6791565248134816761)]
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
            var generator = new ChaCha20Generator(98326);
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
