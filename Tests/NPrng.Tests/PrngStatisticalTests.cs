using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NPrng.Generators;
using Xunit;

namespace NPrng.Tests
{
    internal sealed class DummyGenerator : AbstractPseudoRandomGenerator
    {
        private UInt64 Seed;

        public DummyGenerator(ulong seed)
        {
            Seed = seed;
        }

        public override Int64 Generate()
        {
            unchecked
            {
                var finalBit = Seed & 1;
                Seed <<= 1;
                Seed += 1 - finalBit;
                return (Int64)Seed;
            }
        }
    }

    public sealed class PrngStatisticalTestsData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var seed = GetSeed();

            yield return new object[] {
                nameof(LinearCongruentGenerator),
                (Func<IPseudoRandomGenerator>)(() => new LinearCongruentGenerator(Interlocked.Increment(ref seed)))
            };

            yield return new object[] {
                nameof(Xoshiro256StarStar),
                (Func<IPseudoRandomGenerator>)(() => new Xoshiro256StarStar(Interlocked.Increment(ref seed)))
            };

            var splittableRandom = new SplittableRandom(seed);
            yield return new object[] {
                nameof(SplittableRandom),
                (Func<IPseudoRandomGenerator>)(() => {
                    lock (splittableRandom)
                    {
                        return splittableRandom.Split();
                    }
                })
            };

            yield return new object[] {
                nameof(SplitMix64),
                (Func<IPseudoRandomGenerator>)(() => new SplitMix64(Interlocked.Increment(ref seed)))
            };

            yield return new object[] {
                nameof(ChaCha20Generator),
                (Func<IPseudoRandomGenerator>)(() => new ChaCha20Generator(Interlocked.Increment(ref seed)))
            };
        }

        private static UInt64 GetSeed()
        {
            var time = (int)DateTime.UtcNow.ToBinary();
            var rng = new Random(time);

            for (var i = 0; i < 20; i++)
            {
                rng.Next();
            }
            var seed = (UInt64)rng.Next();
            return seed;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public sealed class PrngStatisticalTests
    {
        private static readonly int Rounds = 60;
        private const int DataSize = 1000000;

        [Theory]
        [ClassData(typeof(PrngStatisticalTestsData))]
        public void TestWindowedDistribution(string name, Func<IPseudoRandomGenerator> instantiator)
        {
            Assert.NotNull(name);
            var invalid = CalculateInvalids(instantiator);
            Assert.Equal(0, invalid);
        }

        private static int CalculateInvalids(Func<IPseudoRandomGenerator> instantiator)
        {
            var invalid = 0;
            var sample = Ranges.Sample;

            Parallel.For(0, sample.Length, (counter, state) =>
            {
                var generator = instantiator();
                var modulus = sample[counter];
                var resultArray = new UInt64[modulus];
                for (var i = 0; i < DataSize; i++)
                {
                    var generatedNo = generator.GenerateInRange(0, modulus-1);
                    resultArray[generatedNo] += 1;
                }

                const double RealDataSize = (double)DataSize;
                var expectedChunkSize = RealDataSize / (double)resultArray.Length;
                var threshold = 0.001 * RealDataSize;

                foreach (var resultNo in resultArray)
                {
                    var tmpDiff = Math.Abs(resultNo - expectedChunkSize);
                    if (tmpDiff > threshold)
                    {
                        Interlocked.Increment(ref invalid);
                        return;
                    }
                }
            });

            return invalid;
        }

        [Theory]
        [ClassData(typeof(PrngStatisticalTestsData))]
        public void TestRanges(string name, Func<IPseudoRandomGenerator> instantiator)
        {
            Assert.NotNull(name);
            var invalid = 0;
            var initialModulus = 22;

            Parallel.For(0, Rounds, (counter, state) => {
                const int lower = -15;
                const int upper = 55;
                var generator = instantiator();
                var modulus = Interlocked.Increment(ref initialModulus);
                for (var i = 0; i < DataSize; i++)
                {
                    var generatedNo = generator.GenerateInRange(lower, upper);
                    if (generatedNo < lower || generatedNo > upper)
                    {
                        Interlocked.Increment(ref invalid);
                        return;
                    }
                }
            });

            Assert.Equal(0, invalid);
        }

        [Theory]
        [ClassData(typeof(PrngStatisticalTestsData))]
        public void TestDoubles(string name, Func<IPseudoRandomGenerator> instantiator)
        {
            Assert.NotNull(name);
            var invalid = 0;

            Parallel.For(0, Rounds, (counter, state) =>
            {
                var total = 0d;
                var generator = instantiator();
                for (var i = 0; i < DataSize; i++)
                {
                    var generatedNo = generator.GenerateDouble();
                    if (generatedNo < 0 || generatedNo >= 1)
                    {
                        Interlocked.Increment(ref invalid);
                        return;
                    }
                    total += generatedNo;
                }
                total /= DataSize;
                total *= 2;
                if (Math.Abs(total - 1d) > 0.01d)
                {
                    Interlocked.Increment(ref invalid);
                }
            });

            Assert.Equal(0, invalid);
        }

        [Theory]
        [ClassData(typeof(PrngStatisticalTestsData))]
        public void TestDoublesDistribution(string name, Func<IPseudoRandomGenerator> instantiator)
        {
            Assert.NotNull(name);
            var invalid = 0;

            Parallel.For(0, Rounds, (counter, state) =>
            {
                const int localDataSize = DataSize * 3;
                const int subdivision = 1009;
                var counts = new int[subdivision];
                var generator = instantiator();
                for (var i = 0; i < localDataSize; i++)
                {
                    var generatedNo = generator.GenerateDouble();
                    var no = (int)(generatedNo * subdivision);
                    counts[no]++;
                }

                const int perSubdivision = localDataSize / subdivision;

                foreach (var count in counts)
                {
                    if ((double)Math.Abs(count - perSubdivision) > perSubdivision * 0.1d)
                    {
                        Interlocked.Increment(ref invalid);
                        break;
                    }
                }
            });

            Assert.Equal(0, invalid);
        }

        [Theory]
        [ClassData(typeof(PrngStatisticalTestsData))]
        public void TestLongestConsecutive(string name, Func<IPseudoRandomGenerator> instantiator)
        {
            Assert.NotNull(name);
            var no = CalculateLongestConsecutive(instantiator());
            Assert.True(no >= 12);
        }

        [Fact]
        public void TestLongestConsecutiveFail()
        {
            var no = CalculateLongestConsecutive(new DummyGenerator(0));
            Assert.True(no < 5);
        }

        private static int CalculateLongestConsecutive(IPseudoRandomGenerator generator)
        {
            var longestConsecutive = 0;
            var tmpLongest = 0;

            for (var i = 0; i < 5000; i++)
            {
                var no = (UInt64)generator.Generate();
                if (no == 0)
                {
                    if (tmpLongest > longestConsecutive)
                    {
                        longestConsecutive = tmpLongest;
                    }
                    tmpLongest = 0;
                    continue;
                }

                while (no > 0)
                {
                    if ((no & 1) == 1)
                    {
                        tmpLongest++;
                    }
                    else
                    {
                        if (tmpLongest > longestConsecutive)
                        {
                            longestConsecutive = tmpLongest;
                        }
                        tmpLongest = 0;
                    }

                    no >>= 1;
                }
            }

            if (tmpLongest > longestConsecutive)
            {
                longestConsecutive = tmpLongest;
            }

            return longestConsecutive;
        }
    }
}
