NPrng
=====

The project contains several implementations of popular pseudo random number generator algorithms. Additionally it contains serializers to save and restore state of every generator in the library.

**Dependencies:** netstandard 2.1

Generators
----------

Generators live in `NPrng.Generators` namespace and at the moment the library supports the following pseudo random generators:

1. [SplittableRandom](https://docs.oracle.com/javase/8/docs/api/java/util/SplittableRandom.html) - port of Java's SplittableRandom class. Quite fast and generates statistically great results. In addition has `.Split()` method that generates a new but independent instance of `SplittableRandom` from the old one. Great whenever deterministic randomness is needed in parallel processing (e.g. Monte Carlo style algorithms). The generator should be used as the default one.
2. [LinearCongruentGenerator](https://en.wikipedia.org/wiki/Linear_congruential_generator) - popular prng algorithm, widely used around. The implementation here uses well chosen multiplier so that the distribution is good enough (based on spectral analysis). Fast and good enough for non-secure purposes.
3. [Xoshiro256StarStar](https://en.wikipedia.org/wiki/Xorshift#xoshiro256**) - a variant of shift-register generators. One of the fastest generators, with good enough distribution.
4. [SplitMix64](https://rosettacode.org/wiki/Pseudo-random_numbers/Splitmix64) - the default prng in Java. Very fast but statistically quite poor. It is suited for seeding other algorithms.
5. [ChaCha20Generator](https://en.wikipedia.org/wiki/Salsa20#ChaCha_variant) - cryptographically secure prng, thus great statistically and extremely hard to predict output based on previous outputs. This is a great algorithm whenever security is necessary. Note that this generator is slower than previous generators, somewhere between 10-20x. However the word "slower" doesn't mean "slow". It is still fast enough if not generating numbers in a tight loop.

Each of the generators implements `IPseudoRandomGenerator` interface and can be used as follows:

```c#
var seed = (ulong)DateTime.UtcNow.ToBinary();
var generator = new LinearCongruentGenerator(seed);
var number = generator.GenerateInRange(0, 333);
```

**IMPORTANT:** all generators are designed to generate 64bit numbers. If you want to generate 32bit integer you have to trim the result.

**WARNING:** None of the generators is thread safe. In order to use them in multithreaded environment there are several things you can do:

1. Write a wrapper that locks the generator
2. Make each thread use a separate instance, for example via static `ThreadLocal<IPseudoRandomGenerator>` member
3. Use `SplittableRandom` and its `.Split()` method and pass new instances to threads directly

Serializers
-----------

Each generator additionally has its own serializer class. These classes implement `IPseudoRandomGeneratorSerializer` interface and are used to store and load current generator state. They live in `NPrng.Serializers` namespace. Example:

```c#
var seed = (ulong)DateTime.UtcNow.ToBinary();
var generator = new LinearCongruentGenerator(seed);
generator.Generate();
generator.Generate();

var serializer = new LinearCongruentGeneratorSerializer();
var serializedState = serializer.WriteToString(generator);
Console.WriteLine(serializedState);  // Base64 encoded binary data
var newGenerator = serializer.ReadFromString(serializedState);

// Test correctedness
var originalNo = generator.Generate();
var newNo = newGenerator.Generate();
Console.WriteLine(originalNo == newNo);  // Should be true
```

The expected size of serialized binary data is the following:

1. SplittableRandom: 16 bytes
2. LinearCongruentGenerator: 8 bytes
3. Xoshiro256StarStar: 32 bytes
4. Splitmix64: 8 bytes
5. ChaCha20Generator: 64 bytes

(De)serialization into a string utilizes base64 encoding.

Tests
=====

**Dependencies:** net 6.0

All generators have a simple statistical test based on prime numbers. You can run them by simply doing

```
> dotnet test
```

in `Tests\NPrng.Tests` folder.

Note however that `PrngStatisticalTests.cs` can take quite sometime, even though it is parallelized. It takes around ~14s for all of them on my 12 core Ryzen9 cpu. A single threaded version of that test took around ~40s.
