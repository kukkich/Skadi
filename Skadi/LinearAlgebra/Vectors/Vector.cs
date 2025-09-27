using System.Collections;

namespace Skadi.LinearAlgebra.Vectors;

public sealed class Vector(params double[] values) : IReadonlyVector<double>
{
    public static Vector Create(int length, double defaultValue = 0)
    {
        return Create(length, _ => defaultValue);
    }
    public static Vector Create(int length, Func<int, double> factory)
    {
        var values = new double[length];
        for (var i = 0; i < length; i++)
            values[i] = factory(i);

        return new Vector(values);
    }

    public double this[int x]
    {
        get => _values[x];
        set => _values[x] = value;
    }
    public int Count => _values.Length;
    public double Norm => Math.Sqrt(ScalarProduct(this, this));

    private readonly double[] _values = values;

    public Vector Copy()
    {
        var values = new double[Count];
        return CopyTo(values);
    }

    public Vector CopyTo(Vector memory)
    {
        if (memory == null || memory.Count != Count)
            throw new ArgumentException();

        for (var i = 0; i < Count; i++)
            memory[i] = this[i];

        return memory;
    }

    public Vector CopyTo(double[] memory)
    {
        if (memory == null || memory.Length != Count)
            throw new ArgumentException();

        for (var i = 0; i < Count; i++)
            memory[i] = this[i];

        return new Vector(memory);
    }

    public Vector(IEnumerable<double> values) : this(values.ToArray())
    {
    }

    public static double ScalarProduct(IReadonlyVector<double> v, IReadonlyVector<double> u)
    {
        if (v.Count != u.Count)
            throw new ArgumentOutOfRangeException($"{nameof(v)} and {nameof(u)} must have the same length");

        return v.Select((t, i) => u[i] * t).Sum();
    }

    public double ScalarProduct(IReadonlyVector<double> v)
    {
        return ScalarProduct(this, v);
    }
    
    public static implicit operator Span<double>(Vector v) => new(v._values);

    public static implicit operator ReadOnlySpan<double>(Vector v) => new(v._values);
    
    public Span<double> AsSpan() => this;
    public ReadOnlySpan<double> AsReadOnlySpan() => this;
    
    public IEnumerator<double> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}