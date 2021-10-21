using System;
using System.IO;
using NPrng.Generators;
using NPrng.Serializers;
using Xunit;

namespace NPrng.Tests
{
    public sealed class Xoshiro256StarStarSerializerTests
    {
        private readonly Xoshiro256StarStarSerializer Serializer
            = new Xoshiro256StarStarSerializer();

        [Theory]
        [InlineData(1, "/ABy+gsWe+bmJj51by4xN1vDUKROEpZE+smHLh8r2B0=", 6664573482289026999, -6703103142870705724)]
        [InlineData(1234, "uio5SW/XcnNyhgUSNXE+kHyF1okSXJs5ZVce5ZglQj4=", -4075166458575869682, -5826677797489693870)]
        public void TestSerialization(ulong seed, string expectedSerialization, long expected1, long expected2)
        {
            var lcg = new Xoshiro256StarStar(seed);
            string result;
            using (var stream = new MemoryStream())
            {
                Serializer.WriteToStream(lcg, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var data = stream.ToArray();
                result = Convert.ToBase64String(data);
            }

            Assert.Equal(expectedSerialization, result);
            var generatedNo = lcg.Generate();
            Assert.Equal(expected1, generatedNo);
            generatedNo = lcg.Generate();
            Assert.Equal(expected2, generatedNo);
        }

        [Theory]
        [InlineData("/ABy+gsWe+bmJj51by4xN1vDUKROEpZE+smHLh8r2B0=", 6664573482289026999, -6703103142870705724)]
        [InlineData("uio5SW/XcnNyhgUSNXE+kHyF1okSXJs5ZVce5ZglQj4=", -4075166458575869682, -5826677797489693870)]
        public void TestDeserialization(string data, long expected1, long expected2)
        {
            var binaryData = Convert.FromBase64String(data);
            var generator = Serializer.ReadFromBuffer(new ArraySegment<byte>(binaryData), out var count);
            Assert.Equal(32, count);
            Assert.NotNull(generator);
            Assert.Equal(expected1, generator.Generate());
            Assert.Equal(expected2, generator.Generate());
        }

        [Theory]
        [InlineData("/ABy+gsWe+bmJj51by4xN1vDUKROEpZE+smHLh8r2B0=", 6664573482289026999, -6703103142870705724)]
        [InlineData("uio5SW/XcnNyhgUSNXE+kHyF1okSXJs5ZVce5ZglQj4=", -4075166458575869682, -5826677797489693870)]
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
            var generator = new Xoshiro256StarStar(98326);
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
