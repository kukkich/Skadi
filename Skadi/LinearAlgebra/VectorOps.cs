using System.Numerics;
using Skadi.LinearAlgebra.Vectors;
using Vector = Skadi.LinearAlgebra.Vectors.Vector;

namespace Skadi.LinearAlgebra;

public static class VectorOps
{
    // ---- kernels: explicit destination, never allocate ----

    public static void LinearCombination(
        ReadOnlySpan<double> a, ReadOnlySpan<double> b,
        double aCoefficient, double bCoefficient,
        Span<double> destination)
    {
        Shape.SameLength(a.Length, b.Length);
        Shape.SameLength(a.Length, destination.Length);

        var width = System.Numerics.Vector<double>.Count;
        var i = 0;
        for (; i <= a.Length - width; i += width)
        {
            var va = new System.Numerics.Vector<double>(a.Slice(i, width));
            var vb = new System.Numerics.Vector<double>(b.Slice(i, width));
            (va * aCoefficient + vb * bCoefficient).CopyTo(destination.Slice(i, width));
        }

        for (; i < a.Length; i++)
            destination[i] = a[i] * aCoefficient + b[i] * bCoefficient;
    }

    public static void Sum(ReadOnlySpan<double> a, ReadOnlySpan<double> b, Span<double> destination) =>
        LinearCombination(a, b, 1d, 1d, destination);

    public static void Subtract(ReadOnlySpan<double> a, ReadOnlySpan<double> b, Span<double> destination) =>
        LinearCombination(a, b, 1d, -1d, destination);

    public static void Scale(double coefficient, ReadOnlySpan<double> a, Span<double> destination)
    {
        Shape.SameLength(a.Length, destination.Length);

        for (var i = 0; i < a.Length; i++)
            destination[i] = a[i] * coefficient;
    }

    /// <returns>v * u^T</returns>
    public static TResult MultiplyAsTransparent<T1, T2, TResult>(ReadOnlySpan<T1> v, ReadOnlySpan<T2> u)
        where TResult : IAdditiveIdentity<TResult, TResult>, IAdditionOperators<TResult, TResult, TResult>
        where T1 : IMultiplyOperators<T1, T2, TResult>
    {
        Shape.SameLength(v.Length, u.Length);

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
        Shape.SameLength(v.Length, u.Length);

        var result = TResult.AdditiveIdentity;
        for (var i = 0; i < v.Length; i++)
            result += u[i] * v[i];
        return result;
    }

    // ---- convenience: allocate destination if omitted ----

    public static Vector Sum(IReadonlyVector<double> a, IReadonlyVector<double> b, Vector? destination = null) =>
        LinearCombination(a, b, 1d, 1d, destination);

    public static Vector Subtract(IReadonlyVector<double> a, IReadonlyVector<double> b, Vector? destination = null) =>
        LinearCombination(a, b, 1d, -1d, destination);

    public static Vector LinearCombination(
        IReadonlyVector<double> a, IReadonlyVector<double> b,
        double aCoefficient, double bCoefficient,
        Vector? destination = null)
    {
        destination = EnsureDestination(a.Count, destination);
        LinearCombination(AsSpan(a), AsSpan(b), aCoefficient, bCoefficient, destination);
        return destination;
    }

    public static Vector Scale(double coefficient, IReadonlyVector<double> a, Vector? destination = null)
    {
        destination = EnsureDestination(a.Count, destination);
        Scale(coefficient, AsSpan(a), destination);
        return destination;
    }

    public static Vector EnsureDestination(int length, Vector? destination)
    {
        if (destination is null)
            return Vector.Create(length);

        Shape.SameLength(length, destination.Count);
        return destination;
    }

    public static void EnsureDestination(ReadOnlySpan<double> source, ref Vector? destination)
    {
        destination = EnsureDestination(source.Length, destination);
    }

    internal static ReadOnlySpan<double> AsSpan(IReadonlyVector<double> v) =>
        v is Vector vector ? vector.AsReadOnlySpan() : v.ToArray();
}
