using System;
using BenchmarkDotNet.Running;

namespace BenchmarkApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = BenchmarkRunner.Run<Benchmark>();

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
