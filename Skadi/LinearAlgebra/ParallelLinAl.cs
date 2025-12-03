using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.LinearAlgebra;

public static class ParallelLinAl
{
    public static Vector Multiply(ComplexMatrix matrix, ReadOnlySpan<double> vector, Vector? resultMemory = null, int threadsCount = 1)
    {
        resultMemory ??= Vector.Create(vector.Length);
        resultMemory.Nullify();
        
        if (matrix.Size == -1)
        {
            // return Vector.None;
            throw new InvalidOperationException("Zero size");
        }

        var x = vector.ToArray();
        var y = resultMemory.AsSpan();

        var vectorLength = y.Length;
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = threadsCount
        };

        using var threadLocalResults = new ThreadLocal<double[]>(() => new double[vectorLength], trackAllValues: true);
        using var threadLocalX = new ThreadLocal<double[]>(() =>
        {
            var localCopy = new double[x.Length];
            Array.Copy(x, localCopy, x.Length);
            return localCopy;
        }, trackAllValues: false);

        Parallel.For(0, matrix.Size, parallelOptions, i =>
        {
            var xLocal = threadLocalX.Value;
            var yLocal = threadLocalResults.Value;
            
            var diagBlock = matrix[i,i];
            var xBlock = new ComplexMatrix.Block(xLocal.AsSpan(i * 2, 2));
            var yBlock = yLocal.AsSpan(i * 2, 2);

            ComplexMatrix.Block.Multiply(diagBlock, xBlock, yBlock);

            for (var j = matrix.RowIndex[i]; j < matrix.RowIndex[i + 1]; ++j)
            {
                var k = matrix.ColumnIndex[j];
                var offDiagBlock = matrix[i,j];
                var xk = new ComplexMatrix.Block(xLocal.AsSpan(k * 2, 2));
                var yk = yLocal.AsSpan(k * 2, 2);

                ComplexMatrix.Block.Multiply(offDiagBlock, xk, yBlock);
                ComplexMatrix.Block.Multiply(offDiagBlock, xBlock, yk);
            }
        });

        foreach (var local in threadLocalResults.Values)
        {
            for (var idx = 0; idx < vectorLength; idx++)
                y[idx] += local[idx];
        }

        return resultMemory;
    }
}