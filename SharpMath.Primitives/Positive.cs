using System.Numerics;

namespace SharpMath.Primitives;

public readonly struct Positive<T> where T : INumber<T>
{
    public T Value { get; }

    public Positive(T value)
    {
        if (T.IsNegative(value) || T.IsZero(value))
        {
            throw new ArgumentException("Value must be positive", nameof(value));
        }

        Value = value;
    }

    public static implicit operator T(Positive<T> positive)
    {
        return positive.Value;
    }

    public static implicit operator Positive<T>(T value)
    {
        return new Positive<T>(value);
    }

    public override string? ToString() => Value.ToString();
}