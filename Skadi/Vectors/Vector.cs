using System.Collections;
using Skadi.Matrices.Sparse.Storages;

namespace Skadi.Vectors;

public sealed class Vector : IVector<double>
{
    public static Vector Create(int length, double defaultValue = 0)
    {
        return Create(length, _ => defaultValue);
    }
    public static Vector Create(int length, Func<int, double> factory)
    {
        var values = new double[length];
        for (int i = 0; i < length; i++)
            values[i] = factory(i);

        return new Vector(values);
    }

    public double this[int x]
    {
        get => _values[x];
        set => _values[x] = value;
    }
    public int Length => _values.Length;
    public double Norm => Math.Sqrt(ScalarProduct(this, this));

    private readonly double[] _values;

    public Vector Copy()
    {
        var values = new double[Length];
        return CopyTo(values);
    }

    public TVector CopyTo<TVector>(TVector memory) where TVector : IVector<double>
    {
        if (memory == null || memory.Length != Length)
            throw new ArgumentException();

        for (var i = 0; i < Length; i++)
            memory[i] = this[i];

        return memory;
    }

    public Vector CopyTo(double[] memory)
    {
        if (memory == null || memory.Length != Length)
            throw new ArgumentException();

        for (var i = 0; i < Length; i++)
            memory[i] = this[i];

        return new Vector(memory);
    }

    public Vector(params double[] values)
    {
        _values = values;
    }
    
    public Vector(IEnumerable<double> values)
    {
        _values = values.ToArray();
    }

    public static double ScalarProduct(IReadonlyVector<double> v, IReadonlyVector<double> u)
    {
        if (v.Length != u.Length)
            throw new ArgumentOutOfRangeException($"{nameof(v)} and {nameof(u)} must have the same length");

        var sum = 0d;

        for (var i = 0; i < v.Length; i++)
            sum += u[i] * v[i];

        return sum;
    }

    public double ScalarProduct(IReadonlyVector<double> v)
    {
        return ScalarProduct(this, v);
    }
    
    public static implicit operator Span<double>(Vector v) => new(v._values);

    public static implicit operator ReadOnlySpan<double>(Vector v) => new(v._values);
    
    public Span<double> AsSpan() => this;
    public ReadOnlySpan<double> AsReadOnlySpan() => this;
    
    public IEnumerator<IndexValue<double>> GetEnumerator()
    {
        for (var i = 0; i < Length; i++)
            yield return new IndexValue<double>(this[i], i);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}