using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using Skadi.Vectors;

namespace SkadiBenchmarks.LinearAlgebra;

public class LinearCombinationBenchmarks
{
    private const int VectorSize = 10_000;
    private Vector v;
    private Vector u;
    private Vector result;

    [GlobalSetup]
    public void Setup()
    {
        var rand = new Random(42);
        var data1 = new double[VectorSize];
        var data2 = new double[VectorSize];

        for (var i = 0; i < VectorSize; i++)
        {
            data1[i] = rand.NextDouble();
            data2[i] = rand.NextDouble();
        }

        v = new Vector(data1);
        u = new Vector(data2);
        result = new Vector(new double[VectorSize]);
    }

    [Benchmark(Baseline = true)]
    public void LinearCombination_Original()
    {
        for (var i = 0; i < v.Length; i++)
        {
            result[i] = v[i] * 2.0 + u[i] * 3.0;
        }
    }

    [Benchmark]
    public void LinearCombination_SIMD_AVX()
    {
        var length = v.Length;
        var i = 0;
        var simdWidth = Vector256<double>.Count;

        if (Avx.IsSupported)
        {
            var vCoeff = Vector256.Create(2.0);
            var uCoeff = Vector256.Create(3.0);

            for (; i <= length - simdWidth; i += simdWidth)
            {
                var vVec = Vector256.Create(v[i], v[i + 1], v[i + 2], v[i + 3]);
                var uVec = Vector256.Create(u[i], u[i + 1], u[i + 2], u[i + 3]);

                var vMul = Avx.Multiply(vVec, vCoeff);
                var uMul = Avx.Multiply(uVec, uCoeff);
                var sum = Avx.Add(vMul, uMul);

                result[i] = sum[0];
                result[i + 1] = sum[1];
                result[i + 2] = sum[2];
                result[i + 3] = sum[3];
            }
        }

        // Остаток
        for (; i < length; i++)
        {
            result[i] = v[i] * 2.0 + u[i] * 3.0;
        }
    }
}
