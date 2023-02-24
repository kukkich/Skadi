namespace SharpMath;

public class Vector
{
    public static Vector Create(int length, double defaultValue = 0)
    {
        return Create(length, _ => defaultValue);
    }
    public static Vector Create(int length, Func<int, double> filling)
    {
        var values = new double[length];
        for (int i = 0; i < length; i++)
            values[i] = filling(i);

        return new Vector(values);
    }

    public virtual double this[int x]
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
        for (int i = 0; i < Length; i++)
            values[i] = this[i];

        return new Vector(values);
    }

    public Vector(params double[] values)
    {
        _values = values;
    }

    public static double ScalarProduct(Vector v, Vector u)
    {
        if (v.Length != u.Length)
            throw new ArgumentOutOfRangeException($"{nameof(v)} and {nameof(u)} must have the same length");

        var sum = 0d;

        for (var i = 0; i < v.Length; i++)
            sum += u[i] * v[i];

        return sum;
    }

    public double ScalarProduct(Vector v)
    {
        return ScalarProduct(this, v);
    }
}