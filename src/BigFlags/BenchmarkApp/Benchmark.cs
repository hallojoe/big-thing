using BenchmarkDotNet.Attributes;
using Core;

namespace BenchmarkApp
{
    public class Benchmark
    {
        public Enum32 Value32 => Enum32.Value30 | Enum32.Value31;
        public Enum64 Value64 => Enum64.Value50 | Enum64.Value61;
        public BigFlags ValueBig => BigFlags.Value10 | BigFlags.Value15;
        public BigFlags ValueBigHigh => BigFlags.Value100 | BigFlags.Value1000;

        [Benchmark]
        public bool Test32() => Value32.HasFlag(Enum32.Value31);
        [Benchmark]
        public bool Test64() => Value64.HasFlag(Enum64.Value50);
        [Benchmark]
        public bool TestBig() => ValueBig.HasFlag(BigFlags.Value10);

        [Benchmark]
        public bool TestBigHigh() => ValueBigHigh.HasFlag(BigFlags.Value1000);

    }
}
