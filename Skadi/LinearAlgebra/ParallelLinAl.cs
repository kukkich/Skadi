using System.Numerics;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;
using Vector = Skadi.LinearAlgebra.Vectors.Vector;

namespace Skadi.LinearAlgebra;

public static class ParallelLinAl
{
    public static Vector Multiply(ComplexMatrix matrix, ReadOnlySpan<double> vector, Vector? resultMemory = null, int threadsCount = 1)
    {
        resultMemory ??= Vector.Create(vector.Length);
        resultMemory.Nullify();

        var x = vector.ToArray();
        var y = resultMemory.AsSpan();

        var vectorLength = y.Length;
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = threadsCount
        };

        using var threadResults = new ThreadLocal<double[]>
        (
            () => new double[vectorLength], 
            true
        );
        using var threadX = new ThreadLocal<double[]>
        (
            () =>
            {
                var localCopy = new double[x.Length];
                Array.Copy(x, localCopy, x.Length);
                return localCopy;
            }, 
            false
        );

        Parallel.For(0, matrix.Size, parallelOptions, i =>
        {
            var xLocal = threadX.Value;
            var yLocal = threadResults.Value;
            
            var diagBlock = matrix[i,i];
            var xBlock = new ComplexMatrix.Block(xLocal.AsSpan(i * 2, 2));
            var ySpan = yLocal.AsSpan(i * 2, 2);

            ComplexMatrix.Block.Multiply(diagBlock, xBlock, ySpan);

            for (var j = matrix.RowIndexes[i]; j < matrix.RowIndexes[i + 1]; ++j)
            {
                var k = matrix.ColumnIndexes[j];
                var offDiagBlock = matrix[i,j];
                var xk = new ComplexMatrix.Block(xLocal.AsSpan(k * 2, 2));
                var yk = yLocal.AsSpan(k * 2, 2);

                ComplexMatrix.Block.Multiply(offDiagBlock, xk, ySpan);
                ComplexMatrix.Block.Multiply(offDiagBlock, xBlock, yk);
            }
        });

        foreach (var local in threadResults.Values)
        {
            for (var idx = 0; idx < vectorLength; idx++)
                y[idx] += local[idx];
        }

        return resultMemory;
    }

    public static double ComplexNorm(Vector a, int threadsCount = 1)
        => Math.Sqrt(ComplexScalarProduct(a, a, threadsCount).Real);
    
    public static Complex ComplexScalarProduct(Vector a, Vector b, int threadsCount = 1)
    {
        LinAl.AssertSameSize(a, b);
        LinAl.AssertEvenLenght(a);
        LinAl.AssertEvenLenght(b);

        var size = a.Count / 2;
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = threadsCount };
        var threadLocalResults = new ThreadLocal<Complex>(() => Complex.Zero, trackAllValues: true);

        Parallel.For(0, size, parallelOptions, i =>
        {
            var aComplex = new Complex(a[2 * i], -a[2 * i + 1]);
            var bComplex = new Complex(b[2 * i], b[2 * i + 1]);
            threadLocalResults.Value += aComplex * bComplex;
        });

        return threadLocalResults.Values.Aggregate<Complex, Complex>(0, (current, local) => current + local);
    }
    
    public static Complex ComplexPseudoScalarProduct(Vector a, Vector b, int threadsCount = 1)
    {
        LinAl.AssertSameSize(a, b);
        LinAl.AssertEvenLenght(a);
        LinAl.AssertEvenLenght(b);

        var size = a.Count / 2;
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = threadsCount };
        var threadLocalResults = new ThreadLocal<Complex>(() => Complex.Zero, trackAllValues: true);

        Parallel.For(0, size, parallelOptions, i =>
        {
            var aComplex = new Complex(a[2 * i], a[2 * i + 1]);
            var bComplex = new Complex(b[2 * i], b[2 * i + 1]);
            threadLocalResults.Value += aComplex * bComplex;
        });

        return threadLocalResults.Values.Aggregate<Complex, Complex>(0, (current, local) => current + local);
    }

    public static Vector Multiply(Vector a, Complex coefficient, Vector? resultMemory = null, int threadsCount = 1)
    {
        LinAl.AssertEvenLenght(a);
        LinAl.ValidateOrAllocateIfNull(a.AsReadOnlySpan(), ref resultMemory);
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = threadsCount
        };

        Parallel.For(0, a.Count / 2, parallelOptions, i =>
        {
            var value = new Complex(a[i * 2], a[i * 2 + 1]);
            var product = coefficient * value;

            resultMemory[i * 2] = product.Real;
            resultMemory[i * 2 + 1] = product.Imaginary;
        });

        return resultMemory;
    }

    public static Vector Sum(Vector a, Vector b, Vector? resultMemory = null, int threadsCount = 1)
    {
        LinAl.AssertSameSize(a, b);
        LinAl.AssertEvenLenght(a);
        LinAl.AssertEvenLenght(b);
        LinAl.ValidateOrAllocateIfNull(a.AsReadOnlySpan(), ref resultMemory);
        
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = threadsCount
        };
        
        Parallel.For
        (
            0, a.Count, 
            parallelOptions,
            i => resultMemory[i] = a[i] + b[i]
        );

        return resultMemory;
    }
    
    public static Vector Subtract(Vector a, Vector b, Vector? resultMemory = null, int threadsCount = 1)
    {
        LinAl.AssertSameSize(a, b);
        LinAl.AssertEvenLenght(a);
        LinAl.AssertEvenLenght(b);
        LinAl.ValidateOrAllocateIfNull(a.AsReadOnlySpan(), ref resultMemory);
        
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = threadsCount
        };
        
        Parallel.For
        (
            0, a.Count, 
            parallelOptions,
            i => resultMemory[i] = a[i] - b[i]
        );

        return resultMemory;
    }
}