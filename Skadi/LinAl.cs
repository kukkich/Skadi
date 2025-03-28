﻿using System.Numerics;
using System.Runtime.CompilerServices;
using Skadi.Matrices;
using Skadi.Matrices.Sparse;
using Skadi.Vectors;
using Vector = Skadi.Vectors.Vector;

namespace Skadi;

public static class LinAl
{
    // TODO replace Vector with IVector and IReadonlyVector where possible
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

        for (var i = 0; i < v.Length; i++)
            resultMemory[i] = v[i] * vCoefficient + u[i] * uCoefficient;

        return resultMemory;
    }

    public static Vector Multiply(double coefficient, IReadonlyVector<double> v, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        for (var i = 0; i < v.Length; i++)
            resultMemory[i] = coefficient * v[i];

        return resultMemory;
    }
    public static Matrix Multiply(double coefficient, MatrixBase a, Matrix? resultMemory = null)
    {
        resultMemory = ValidateOrAllocateIfNull(a, resultMemory).AsMutable();

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

    public static ImmutableMatrix Multiply(double coefficient, ImmutableMatrix a)
    {
        return new ImmutableMatrix(a, a.Coefficient * coefficient);
    }
    public static ImmutableMatrix Multiply(ImmutableMatrix a, double coefficient)
    {
        return new ImmutableMatrix(a, a.Coefficient * coefficient);
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
    public static Vector Multiply(MatrixBase a, IReadonlyVector<double> v, Vector? resultMemory = null)
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
    public static Span<double> Multiply(MatrixBase a, ReadOnlySpan<double> v, Span<double> resultMemory)
    {
        AssertSameSize(a, v);
        AssertSameSize(v, (ReadOnlySpan<double>)resultMemory);
            
        for (var i = 0; i < v.Length; i++)
        for (var j = 0; j < v.Length; j++)
            resultMemory[i] += a[i, j] * v[j];

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
    public static Matrix Multiply(MatrixBase a, MatrixBase b, Matrix? resultMemory = null)
    {
        AssertSameSize(a, b);
        AssertDifferentObjects(a, resultMemory);
        AssertDifferentObjects(b, resultMemory);
        resultMemory = ValidateOrAllocateIfNullForMultiplying(a, b, resultMemory).AsMutable();

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

    public static MatrixBase Sum(MatrixBase a, MatrixBase b, Matrix? resultMemory = null)
    {
        AssertCanBeMultiplied(a, b);
        resultMemory = ValidateOrAllocateIfNull(a, resultMemory).AsMutable();

        for (var i = 0; i < a.Rows; i++)
            for (var j = 0; j < a.Rows; j++)
                resultMemory[i, j] = a[i, j] + b[i, j];

        return resultMemory;
    }
    public static MatrixSpan Sum(MatrixBase a, MatrixBase b, MatrixSpan resultMemory)
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
    private static MatrixBase ValidateOrAllocateIfNull(MatrixBase a, MatrixBase? b)
    {
        if (b is null)
            b = new Matrix(new double[a.Rows, a.Columns]);
        else AssertSameSize(a, b);

        return b;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AssertSameSize(MatrixBase a, MatrixBase b)
    {
        if (a.Rows != b.Rows)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "must have the same rows");
        if (a.Columns != b.Columns)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "must have the same columns");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static MatrixBase ValidateOrAllocateIfNullForMultiplying(MatrixBase a, MatrixBase b, MatrixBase? c)
    {
        AssertCanBeMultiplied(a, b);
        if (c is null)
            c = new Matrix(new double[a.Rows, b.Columns]);
        else if (a.Rows != c.Rows || b.Columns != c.Columns)
            throw new ArgumentOutOfRangeException(nameof(c), "сan't be the result of a multiplication");

        return c;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AssertCanBeMultiplied(MatrixBase a, MatrixBase b)
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
    private static void AssertSameSize(MatrixBase a, ReadOnlySpan<double> v)
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
    private static void AsserSameSize(MatrixBase a, ReadOnlyMatrixSpan b)
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
}