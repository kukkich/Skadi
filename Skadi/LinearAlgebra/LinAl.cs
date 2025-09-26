using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Skadi.LinearAlgebra.Matrices;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;
using Vector = Skadi.LinearAlgebra.Vectors.Vector;

namespace Skadi.LinearAlgebra;

public static class LinAl
{
    public static Vector Sum(IReadonlyVector<double> v, IReadonlyVector<double> u, Vector? resultMemory = null)
    {
        return LinearCombination(v, u, 1.0, 1.0, resultMemory);
    }
    public static Vector Subtract(IReadonlyVector<double> v, IReadonlyVector<double> u, Vector? resultMemory = null)
    {
        return LinearCombination(v, u, 1.0, -1.0, resultMemory);
    }
    
    public static Vector LinearCombination(
        IReadonlyVector<double> v, IReadonlyVector<double> u,
        double vCoefficient, double uCoefficient,
        Vector? resultMemory = null
    )
    {
        AssertSameSize(v, u);
        ValidateOrAllocateIfNull(v, ref resultMemory!);
        var length = v.Length;

        var i = 0;
        var simdWidth = Vector256<double>.Count;
        var vCoeffVec = Vector256.Create(vCoefficient);
        var uCoeffVec = Vector256.Create(uCoefficient);

        for (; i <= length - simdWidth; i += simdWidth)
        {
            var vVec = Vector256.Create(v[i], v[i + 1], v[i + 2], v[i + 3]);
            var uVec = Vector256.Create(u[i], u[i + 1], u[i + 2], u[i + 3]);

            var vMul = Avx.Multiply(vVec, vCoeffVec);
            var uMul = Avx.Multiply(uVec, uCoeffVec);

            var sum = Avx.Add(vMul, uMul);

            resultMemory[i] = sum[0];
            resultMemory[i + 1] = sum[1];
            resultMemory[i + 2] = sum[2];
            resultMemory[i + 3] = sum[3];
        }

        for (; i < length; i++)
        {
            resultMemory[i] = v[i] * vCoefficient + u[i] * uCoefficient;
        }

        return resultMemory;
    }
    
    public static Vector Multiply(double coefficient, IReadonlyVector<double> v, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        for (var i = 0; i < v.Length; i++)
            resultMemory[i] = coefficient * v[i];

        return resultMemory;
    }
    public static Matrix Multiply(double coefficient, IReadOnlyMatrix a, Matrix? resultMemory = null)
    {
        resultMemory = ValidateOrAllocateIfNull(a, resultMemory);

        for (var i = 0; i < a.Rows; i++)
            for (var j = 0; j < a.Columns; j++)
                resultMemory[i, j] = a[i, j] * coefficient;

        return resultMemory;
    }
    public static MatrixSpan Multiply(double coefficient, ReadOnlyMatrixSpan a, MatrixSpan resultMemory)
    {
        AsserSameSize(a, resultMemory);

        for (var i = 0; i < a.Size; i++)
        for (var j = 0; j < a.Size; j++)
            resultMemory[i, j] = a[i, j] * coefficient;

        return resultMemory;
    }

    public static Vector Multiply(CSRMatrix a, ReadOnlySpan<double> v, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(v, ref resultMemory!);
        var rowPointers = a.RowPointers;
        var columnIndexes = a.ColumnIndexes;
        var values = a.Values;
        
        var simdWidth = Vector256<double>.Count;
        for (var row = 0; row < a.Size; row++)
        {
            var rowStart = rowPointers[row];
            var rowEnd = rowPointers[row + 1];
            var length = rowEnd - rowStart;

            var sumVec = Vector256<double>.Zero;
            var i = 0;
            for (; i <= length - simdWidth; i += simdWidth)
            {
                var idx = rowStart + i;
            
                var valVec = Vector256.Create
                (
                    values[idx], 
                    values[idx + 1], 
                    values[idx + 2], 
                    values[idx + 3]
                );
            
                var vecV = Vector256.Create
                (
                    v[columnIndexes[idx]], 
                    v[columnIndexes[idx + 1]], 
                    v[columnIndexes[idx + 2]], 
                    v[columnIndexes[idx + 3]]
                );
            
                sumVec = Avx.Add(sumVec, Avx.Multiply(valVec, vecV));
            }
            var sum = HorizontalSum(sumVec);
        
            for (; i < length; i++)
            {
                var idx = rowStart + i;
                sum += values[idx] * v[columnIndexes[idx]];
            }

            resultMemory[row] = sum;
        }

        return resultMemory;
    }
    
    public static Vector Multiply(SparseMatrix a, IReadonlyVector<double> v, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        var rowsIndexes = a.RowsIndexes;
        var columnsIndexes = a.ColumnsIndexes;
        var di = a.Diagonal;
        var lowerValues = a.LowerValues;
        var upperValues = a.UpperValues;

        for (var i = 0; i < a.RowsCount; i++)
        {
            resultMemory[i] += di[i] * v[i];

            for (var j = rowsIndexes[i]; j < rowsIndexes[i + 1]; j++)
            {
                resultMemory[i] += lowerValues[j] * v[columnsIndexes[j]];
                resultMemory[columnsIndexes[j]] += upperValues[j] * v[i];
            }
        }

        return resultMemory;
    }
    public static Vector Multiply(SymmetricRowSparseMatrix matrix, IReadonlyVector<double> x, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(x, ref resultMemory!);
        AssertSameSize(matrix, x);

        for (var i = 0; i < x.Length; i++)
            resultMemory[i] = x[i] * matrix.Diagonal[i];

        for (var i = 0; i < x.Length; i++)
        {
            foreach (var indexValue in matrix[i])
            {
                resultMemory[i] += indexValue.Value * x[indexValue.ColumnIndex];
                resultMemory[indexValue.ColumnIndex] += indexValue.Value * x[i];
            }
        }

        return resultMemory;
    }
    public static Vector Multiply(IReadOnlyMatrix a, IReadonlyVector<double> v, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        for (var i = 0; i < v.Length; i++)
            for (var j = 0; j < v.Length; j++)
                resultMemory[i] += a[i, j] * v[j];

        return resultMemory;
    }
    public static Vector Multiply(Matrix a, IReadonlyVector<double> v, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        for (var i = 0; i < v.Length; i++)
        for (var j = 0; j < v.Length; j++)
            resultMemory[i] += a[i, j] * v[j];

        return resultMemory;
    }
    public static Span<double> Multiply(ReadOnlyMatrixSpan a, ReadOnlySpan<double> v, Span<double> resultMemory)
    {
        AssertSameSize(a, v);
        AssertSameSize(v, (ReadOnlySpan<double>)resultMemory);
        
        for (var i = 0; i < v.Length; i++)
        for (var j = 0; j < v.Length; j++)
            resultMemory[i] += a[i, j] * v[j];

        return resultMemory;
    }
    
    /// <returns>v * u^T</returns>
    public static TResult MultiplyAsTransparent<T1, T2, TResult>(ReadOnlySpan<T1> v, ReadOnlySpan<T2> u)
        where TResult : IAdditiveIdentity<TResult, TResult>, IAdditionOperators<TResult, TResult, TResult>
        where T1 : IMultiplyOperators<T1, T2, TResult>
    {
        AssertSameSize(v, u);
        
        var result = TResult.AdditiveIdentity;
        for (var i = 0; i < v.Length; i++)
            result += v[i] * u[i]!;
        return result;
    }
    /// <returns>v * u^T</returns>
    public static TResult MultiplyAsTransparent<TResult>(ReadOnlySpan<double> v, ReadOnlySpan<TResult> u)
        where TResult : IAdditiveIdentity<TResult, TResult>, IAdditionOperators<TResult, TResult, TResult>, 
        IMultiplyOperators<TResult, double, TResult>
    {
        AssertSameSize(v, u);
        
        var result = TResult.AdditiveIdentity;
        for (var i = 0; i < v.Length; i++)
            result += u[i] * v[i];
        return result;
    }
    public static Span<double> Multiply(IReadOnlyMatrix a, ReadOnlySpan<double> v, Span<double> resultMemory)
    {
        AssertSameSize(a, v);
        AssertSameSize(v, (ReadOnlySpan<double>)resultMemory);
            
        for (var i = 0; i < v.Length; i++)
        for (var j = 0; j < v.Length; j++)
            resultMemory[i] += a[i, j] * v[j];

        return resultMemory;
    }
    
    public static Matrix Multiply(IReadOnlyMatrix a, IReadOnlyMatrix b, Matrix? resultMemory = null)
    {
        AssertSameSize(a, b);
        AssertDifferentObjects(a, resultMemory);
        AssertDifferentObjects(b, resultMemory);
        resultMemory = ValidateOrAllocateIfNullForMultiplying(a, b, resultMemory);

        for (var aRow = 0; aRow < a.Rows; aRow++)
        {
            for (var bColumn = 0; bColumn < b.Columns; bColumn++)
            {
                double sum = 0;
                for (var i = 0; i < a.Columns; i++)
                    sum += a[aRow, i] * b[i, bColumn];

                resultMemory[aRow, bColumn] = sum;
            }
        }

        return resultMemory;
    }

    public static IReadOnlyMatrix Sum(IReadOnlyMatrix a, IReadOnlyMatrix b, Matrix? resultMemory = null)
    {
        AssertCanBeMultiplied(a, b);
        resultMemory = ValidateOrAllocateIfNull(a, resultMemory);

        for (var i = 0; i < a.Rows; i++)
            for (var j = 0; j < a.Rows; j++)
                resultMemory[i, j] = a[i, j] + b[i, j];

        return resultMemory;
    }
    public static MatrixSpan Sum(IReadOnlyMatrix a, IReadOnlyMatrix b, MatrixSpan resultMemory)
    {
        AssertCanBeMultiplied(a, b);
        AsserSameSize(a, resultMemory);
        
        for (var i = 0; i < a.Rows; i++)
        for (var j = 0; j < a.Rows; j++)
            resultMemory[i, j] = a[i, j] + b[i, j];

        return resultMemory;
    }
    public static MatrixSpan Sum(ReadOnlyMatrixSpan a, ReadOnlyMatrixSpan b, MatrixSpan resultMemory)
    {
        AsserSameSize(a, b);

        for (var i = 0; i < a.Size; i++)
        for (var j = 0; j < a.Size; j++)
            resultMemory[i, j] = a[i, j] + b[i, j];

        return resultMemory;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Matrix ValidateOrAllocateIfNull(IReadOnlyMatrix a, Matrix? b)
    {
        if (b is null)
            b = new Matrix(new double[a.Rows, a.Columns]);
        else AssertSameSize(a, b);

        return b;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AssertSameSize(IReadOnlyMatrix a, IReadOnlyMatrix b)
    {
        if (a.Rows != b.Rows)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "must have the same rows");
        if (a.Columns != b.Columns)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "must have the same columns");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Matrix ValidateOrAllocateIfNullForMultiplying(IReadOnlyMatrix a, IReadOnlyMatrix b, Matrix? c)
    {
        AssertCanBeMultiplied(a, b);
        if (c is null)
            c = new Matrix(new double[a.Rows, b.Columns]);
        else if (a.Rows != c.Rows || b.Columns != c.Columns)
            throw new ArgumentOutOfRangeException(nameof(c), "сan't be the result of a multiplication");

        return c;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AssertCanBeMultiplied(IReadOnlyMatrix a, IReadOnlyMatrix b)
    {
        if (a.Columns != b.Rows)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "can't be multiplied");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ValidateOrAllocateIfNull(IReadonlyVector<double> v, ref Vector? u)
    {
        if (u is null)
            u = Vector.Create(v.Length);
        else AssertSameSize(v, u);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ValidateOrAllocateIfNull(ReadOnlySpan<double> v, ref Vector? u)
    {
        if (u is null)
            u = Vector.Create(v.Length);
        else AssertSameSize(v, (ReadOnlySpan<double>)u);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSameSize<T>(IReadonlyVector<T> v, IReadonlyVector<T> u)
    {
        if (v.Length != u.Length)
            throw new ArgumentOutOfRangeException($"{nameof(v)} and {nameof(u)}", "must have the same length");
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSameSize(SymmetricRowSparseMatrix matrix, IReadonlyVector<double> x)
    {
        if (matrix.Size != x.Length)
        {
            throw new ArgumentOutOfRangeException($"{nameof(matrix.Size)} and {nameof(x)}", "must have the same length");
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSameSize(IReadOnlyMatrix a, ReadOnlySpan<double> v)
    {
        if (a.Columns != v.Length || a.Rows != v.Length)
        {
            throw new ArgumentException();
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSameSize(MatrixSpan a, ReadOnlySpan<double> v)
    {
        if (a.Size != v.Length)
        {
            throw new ArgumentException();
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSameSize(ReadOnlyMatrixSpan a, ReadOnlySpan<double> v)
    {
        if (a.Size != v.Length)
        {
            throw new ArgumentException();
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSameSize<T1, T2>(ReadOnlySpan<T1> v, ReadOnlySpan<T2> u)
    {
        if (v.Length != u.Length)
        {
            throw new ArgumentException("Both vectors must have the same length.");
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AsserSameSize(IReadOnlyMatrix a, ReadOnlyMatrixSpan b)
    {
        if (a.Rows != b.Size || a.Columns != b.Size)
        {
            throw new ArgumentException();
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AsserSameSize(ReadOnlyMatrixSpan a, ReadOnlyMatrixSpan b)
    {
        if (a.Size != b.Size)
        {
            throw new ArgumentException();
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertDifferentObjects(object a, object? resultMemory)
    {
        if (a == resultMemory) 
            throw new ArgumentOutOfRangeException($"{nameof(resultMemory)}", "can't be equal to one of the arguments");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double HorizontalSum(Vector256<double> v)
    {
        // Складываем верхнюю и нижнюю половины вектора
        var sum128 = Sse2.Add(v.GetLower(), v.GetUpper());
    
        // Складываем верхнюю и нижнюю половины 128-битного вектора
        var sum64 = Sse2.Add(sum128, Sse2.Shuffle(sum128, sum128, 0x1));
    
        // общая сумма
        return sum64.GetElement(0);
    }
}